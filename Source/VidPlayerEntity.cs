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
public sealed class VidPlayerEntity : Entity {
    private readonly VidPlayerCoreEntity core;
    public VidPlayerCore Core => core;

    public bool Done => core.Done;

    public bool ForcePause;
    public bool Muted {
        get => core.Muted;
        set => core.Muted = value;
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
        core = new VidPlayerCoreEntity(this, entitySize, videoTarget, entityIsMuted, entityKeepAspectRatio, entityLooping, entityHires, entityVolumeMult);
        Tag = Tags.PauseUpdate | Tags.TransitionUpdate;
        Depth = Depths.Top;

        // Switch to this once it hits main
        // if (SRTModImports.IgnoreSaveState is { } cb) {
        //     cb(this, true);
        // }

#if DEBUG // Just for quick debug rendering
        Collider = new Hitbox(entitySize.X, entitySize.Y);
#endif
        if (core.hires)
            Tag |= TagsExt.SubHUD;
    }

    public override void Awake(Scene scene) {
        base.Awake(scene);
        core.Init();
    }

    public override void Update() {
        base.Update();
        core.Update();
    }

    public override void Render() {
        base.Render();
        core.Render();
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        core.Mark();
    }

    public override void SceneEnd(Scene scene) {
        base.SceneEnd(scene);
        core.Mark();
    }

    private class VidPlayerCoreEntity : VidPlayerCore {
        private readonly VidPlayerEntity owner;

        public VidPlayerCoreEntity(VidPlayerEntity owner, Vector2 entitySize, string videoTarget, bool entityIsMuted, bool entityKeepAspectRatio, bool entityLooping, bool entityHires, float entityVolumeMult) : base(entitySize, videoTarget, entityIsMuted, entityKeepAspectRatio, entityLooping, entityHires, entityVolumeMult) {
            this.owner = owner;
        }

        protected override bool Paused => owner.Scene.Paused || owner.SceneAs<Level>().Transitioning || owner.ForcePause;
        protected override Vector2 Position => hires ? owner.Position - owner.SceneAs<Level>().Camera.Position : owner.Position;

        protected override void LoadState(Level newLevel) {
            base.LoadState(newLevel);
            owner.Scene = newLevel;
        }
    }

}

// Exposed to Lua
public class VidPlayerEntityLua {
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
    /// <returns>A handle you can use in Lua. See LuaHandle for more info.</returns>
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
    /// <returns>A handle you can use in Lua. See LuaHandle for more info.</returns>
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
    /// You can also control the player with the properties and methods below.
    /// </summary>
    public class LuaHandle {
        private readonly WeakReference<VidPlayerEntity> _ref;
        private bool disposed;
        
        /// <summary>
        /// Whether the entity is or should be visible. Does not stop audio from playing. Use Pause for that.
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
        /// Pauses or resumes the video player.
        /// </summary>
        public bool Paused {
            get => getRef()?.ForcePause ?? false;
            set {
                VidPlayerEntity? @ref = getRef();
                if (@ref != null) @ref.ForcePause = value;
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

        /// <summary>
        /// Resets the video player to the start of the video.
        /// </summary>
        public void Reset() {
            VidPlayerEntity? @ref = getRef();
            if (@ref == null) return;
            @ref.Core?.Reset();
        }
    }
}
