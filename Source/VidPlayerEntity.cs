// #define DEBUG_GC

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Monocle;

namespace Celeste.Mod.VidPlayer;

[Tracked]
[CustomEntity("VidPlayer/VidPlayerEntity")]
public class VidPlayerEntity : Entity {
    private readonly Vector2 size;
    private bool muted;
    private readonly bool keepAspectRatio;
    private readonly bool looping;
    private readonly bool hires;
    private readonly float volumeMult;
    private readonly MTexture fallback;
    private readonly VidPlayerManager.VidPlayerEntry? vidEntry;
    private bool hasWoken = false;
    
    
    // For handyness
    private VideoPlayer? videoPlayer => vidEntry?.videoPlayer;
    
#if DEBUG_GC
    public static int bornCounter = 0;
    public static uint ids = 0;
    public uint id;
#endif
    
    public bool Done => vidEntry == null || hasWoken && (videoPlayer?.State ?? (MediaState)(-1)) == MediaState.Stopped;
    public bool Muted {
        get => muted;
        set => muted = value;
    }

    public VidPlayerEntity(EntityData data, Vector2 offset) 
        : this(data.Position, 
            new Vector2(data.Width, data.Height), 
            data.String("video"), 
            data.Bool("muted"), 
            data.Bool("keepAspectRatio"), 
            data.Bool("looping"), 
            data.Bool("hires"), 
            data.Float("volumeMult"), 
            offset) {
    }
    
    public VidPlayerEntity(Vector2 position, 
        Vector2 entitySize, 
        string videoTarget, 
        bool entityIsMuted, 
        bool entityKeepAspectRatio, 
        bool entityLooping, 
        bool entityHires, 
        float entityVolumeMult, 
        Vector2 offset) : base(position + offset) {
#if DEBUG_GC
        bornCounter++;
        id = ids;
        ids++;
#endif
        Tag = Tags.PauseUpdate | Tags.TransitionUpdate;
        Depth = Depths.Top;
        size = entitySize;
        muted = entityIsMuted;
        keepAspectRatio = entityKeepAspectRatio;
        looping = entityLooping;
        hires = entityHires;
        volumeMult = entityVolumeMult;

        // Switch to this once it hits main
        // if (SRTModImports.IgnoreSaveState is { } cb) {
        //     cb(this, true);
        // }
        
        
        fallback = GFX.Game["__fallback"];
#if DEBUG // Just for quick debug rendering
        Collider = new Hitbox(size.X, size.Y);
#endif
        try {
            vidEntry = VidPlayerManager.GetPlayerFor(videoTarget);
        } catch (FileNotFoundException fex) {
            Logger.LogDetailed(fex);
            hires = false;
        }
        
        if (hires)
            Tag |= TagsExt.SubHUD;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        if (vidEntry == null) return;
        videoPlayer!.IsLooped = looping;
        videoPlayer.IsMuted = muted;
        videoPlayer.Play(vidEntry.video);
        base.Awake(scene);
        hasWoken = true;
    }

    public override void Update() {
        base.Update();
        if (Input.GrabCheck) {
            RemoveSelf();
            return;
        }
        
        if (videoPlayer == null) return;
        float normVolume = Settings.Instance.MusicVolume / 10f /* Max volume */ * volumeMult;
        normVolume = Math.Clamp(normVolume, 0f, 1f);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (normVolume != videoPlayer.Volume)
            videoPlayer.Volume = normVolume;
        if (Scene.Paused || SceneAs<Level>().Transitioning) {
            videoPlayer.Pause();
        } else {
            videoPlayer.Resume();
        }
    }

    public override void Render() {
        base.Render();
        int scalingFactor = hires ? 6 : 1;
        Vector2 camPos = hires ? SceneAs<Level>().Camera.Position : Vector2.Zero;
        if (vidEntry == null) {
            fallback.Draw((Position - camPos) * scalingFactor, Vector2.Zero, Color.White, MathF.Min(size.X / fallback.Width, size.Y / fallback.Height) * scalingFactor);
            return;
        }
        Texture2D currTexture = videoPlayer!.GetTexture();
        Rectangle dstRect;
        if (!keepAspectRatio) {
            dstRect = new Rectangle((int)(Position.X - camPos.X) * scalingFactor, (int)(Position.Y - camPos.Y) * scalingFactor, (int)size.X * scalingFactor, (int)size.Y * scalingFactor);
        } else {
            float ratio = vidEntry.video.Width / (float)vidEntry.video.Height;
            float finalSizeX;
            float finalSizeY;
            if (size.X / size.Y > ratio) {
                finalSizeX = size.Y * ratio;
                finalSizeY = size.Y;
            } else {
                finalSizeX = size.X;
                finalSizeY = size.X / ratio;
            }
            dstRect = new Rectangle((int)(Position.X - camPos.X) * scalingFactor, (int)(Position.Y - camPos.Y) * scalingFactor, (int)finalSizeX * scalingFactor, (int)finalSizeY * scalingFactor);
        }
        Draw.SpriteBatch.Draw(currTexture, dstRect, Color.White);
    }
    
    public override void Removed(Scene scene) {
        videoPlayer?.Pause();
        base.Removed(scene);
    }
    
    private void SaveState() {
        videoPlayer?.Pause();
    }

    private void LoadState(Level newLevel) {
        Scene = newLevel;
        videoPlayer?.Pause();
    }

#if DEBUG_GC
    ~VidPlayerEntity() {
        bornCounter--;
    }
#endif

    // About savestate support, we never clone this entity, we just pause it when it is removed on scene, it will autoplay on next Update
    // we also never release the VideoPlayer explicitly, gc will do the job for us
    // this is due to not having any clean way of knowing when an entity is in a savestate, other than ref counting, which is what the gc
    // does for us
    private static object? RSLASaveLoadAction;
    private static readonly Func<Type, bool> rsoPred = t => t == typeof(VidPlayerEntity);
    
    public static void RegisterSRTInterop() {
        if (SRTModImports.AddReturnSameObjectProcessor is { } rsop) {
            rsop(rsoPred);
        }
        if (SRTModImports.RegisterSaveLoadAction is { } rsla) {
            RSLASaveLoadAction = rsla(Save, Load, null, null, null, null);
        }
    }

    public static void UnregisterSRTInterop() {
        if (SRTModImports.Unregister is { } unregister) {
            // unregister(RSTSaveLoadAction);
            unregister(RSLASaveLoadAction!);
        }
        if (SRTModImports.RemoveReturnSameObjectProcessor is { } rrsop) {
            rrsop(rsoPred);
        }
    }

    private static void Save(Dictionary<Type, Dictionary<string, object>> ctx, Level level) {
        foreach (Entity vidPlayerEntity in level.Tracker.GetEntities<VidPlayerEntity>()) {
            ((VidPlayerEntity) vidPlayerEntity).SaveState();
        }
    }
    
    private static void Load(Dictionary<Type, Dictionary<string, object>> ctx, Level level) {
        foreach (Entity vidPlayerEntity in level.Tracker.GetEntities<VidPlayerEntity>()) {
            ((VidPlayerEntity) vidPlayerEntity).LoadState(level);
        }
    }

    
    // Exposed to Lua
    public static class Lua {
        /// <summary>
        /// Spawns a looping video entity, playing the given video file.
        /// </summary>
        /// <param name="videoTarget">The video file path.</param>
        /// <param name="x">X position relative to the room we are currently in.</param>
        /// <param name="y">Y position relative to the room we are currently in.</param>
        /// <param name="width">Width of the video, in in-game pixels.</param>
        /// <param name="height">Height of the video, in in-game pixels.</param>
        /// <param name="muted">Whether audio plays.</param>
        /// <param name="hires">Whether the video will be downscaled to celeste's gameplay resolution.</param>
        /// <returns>A handle you can use in Lua. See LuaHandle down below.</returns>
        public static LuaHandle SpawnLoop(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true) {
            if (Engine.Scene is not Level level) {
                Logger.Error(nameof(VidPlayer), "Tried to spawn video player on non-level scene!");
                return null!;
            }
            VidPlayerEntity vidPlayerEntity = new(new Vector2(x, y), new Vector2(width, height), videoTarget, muted, true, true, hires, 1f, level.LevelOffset);
            level.Add(vidPlayerEntity);
            return new LuaHandle(vidPlayerEntity);
        }

        /// <summary>
        /// Spawns a video entity that will play a video until it ends, removing itself afterwards.
        /// </summary>
        /// <param name="videoTarget">The video file path.</param>
        /// <param name="x">X position relative to the room we are currently in.</param>
        /// <param name="y">Y position relative to the room we are currently in.</param>
        /// <param name="width">Width of the video, in in-game pixels.</param>
        /// <param name="height">Height of the video, in in-game pixels.</param>
        /// <param name="muted">Whether audio plays.</param>
        /// <param name="hires">Whether the video will be downscaled to celeste's gameplay resolution.</param>
        /// <returns>An IEnumerator you can `coroutine.yield` in lua to execute it.</returns>
        public static IEnumerator SpawnOneShot(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true) {
            if (Engine.Scene is not Level level) {
                Logger.Error(nameof(VidPlayer), "Tried to spawn video player on non-level scene!");
                yield break;
            }

            LuaHandle handle = SpawnFullParameterized(videoTarget, x, y, width, height, muted, hires, true, false, 1f);

            do {
                yield return null;
            } while (!handle.Done); // Wait at least a frame
            // Otherwise the RemoveSelf may fail

            handle.RemoveSelf();
        }

        /// <summary>
        /// Spawns a video entity with all the parameters available.
        /// </summary>
        /// <param name="videoTarget">The video file path.</param>
        /// <param name="x">X position relative to the room we are currently in.</param>
        /// <param name="y">Y position relative to the room we are currently in.</param>
        /// <param name="width">Width of the video, in in-game pixels.</param>
        /// <param name="height">Height of the video, in in-game pixels.</param>
        /// <param name="muted">Whether audio plays.</param>
        /// <param name="hires">Whether the video will be downscaled to celeste's gameplay resolution.</param>
        /// <param name="keepAspectRatio">Whether to stretch the video to fit the given width/height or to keep the ratio width/height ratio from the video file.</param>
        /// <param name="looping">Whether playback loops forever or stops after the video is over.</param>
        /// <param name="volumeMult">Real value to multiply the volume with, ranging from 0 to 1.</param>
        /// <returns>A handle you can use in Lua. See LuaHandle down below.</returns>
        public static LuaHandle SpawnFullParameterized(
            string videoTarget,
            int x,
            int y,
            int width,
            int height,
            bool muted,
            bool hires,
            bool keepAspectRatio,
            bool looping,
            float volumeMult) {
            if (Engine.Scene is not Level level) {
                Logger.Error(nameof(VidPlayer), "Tried to spawn video player on non-level scene!");
                return null!;
            }
            VidPlayerEntity vidPlayerEntity = new(new Vector2(x, y), new Vector2(width, height), videoTarget, muted, keepAspectRatio, looping, hires, volumeMult, level.LevelOffset);
            level.Add(vidPlayerEntity);

            return new LuaHandle(vidPlayerEntity);
        }

        // It is extremely easy to leak references to c# objects in lua by accidentally not declaring something as local,
        // thus to prevent such mistakes we need a wrapper, the vid player entity will be alive as long as it is in the scene
        // thus this should be reliable

        /// <summary>
        /// A handle usable in lua to manipulate VidPlayerEntities
        /// Call `RemoveSelf` on this to remove the entity. This handle won't be functional after the call.
        /// </summary>
        public class LuaHandle {
            private readonly WeakReference<VidPlayerEntity> _ref;
            private bool disposed;
            
            /// <summary>
            /// Whether the entity is or should be visible.
            /// </summary>
            public bool Visible {
                get => getRef()?.Visible ?? true;
                set {
                    VidPlayerEntity? @ref = getRef();
                    if (@ref != null) @ref.Visible = value;
                }
            }

            /// <summary>
            /// Whether audio is muted or playing.
            /// </summary>
            public bool Muted {
                get => getRef()?.Muted ?? false;
                set {
                    VidPlayerEntity? @ref = getRef();
                    if (@ref != null) @ref.Muted = value;
                }
            }

            /// <summary>
            /// Whether the video has finished, or always false if the video loops.
            /// </summary>
            public bool Done => getRef()?.Done ?? true;

            public LuaHandle(VidPlayerEntity vidPlayerEntity) {
                _ref = new WeakReference<VidPlayerEntity>(vidPlayerEntity);
            }

            public void RemoveSelf() {
                getRef()?.RemoveSelf();
                disposed = true;
            }

            private VidPlayerEntity? getRef() {
                if (disposed) return null;
                if (!_ref.TryGetTarget(out VidPlayerEntity? vidPlayerEntity)) {
                    Logger.Error(nameof(VidPlayerModule), "VidPlayerEntity died prematurely!");
                }
                return vidPlayerEntity;
            }
        }
    }
}
