using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public abstract class VidPlayerCore {
    private readonly Vector2 size;
    private bool muted;
    private readonly bool keepAspectRatio;
    private readonly bool looping;
    internal bool hires;
    private readonly float volumeMult;
    private readonly MTexture fallback;
    private VidPlayerManager.VidPlayerEntry? vidEntry;
    private bool hasWoken = false;
    
    // For handyness
    internal VideoPlayer? videoPlayer => vidEntry?.videoPlayer;
    
    public bool Done => vidEntry == null || hasWoken && (videoPlayer?.State ?? (MediaState)(-1)) == MediaState.Stopped;
    public bool Muted {
        get => muted;
        set => muted = value;
    }
    
    protected abstract bool Paused { get; }
    
    protected abstract Vector2 Position { get; }

    public VidPlayerCore(Vector2 entitySize, 
        string videoTarget, 
        bool entityIsMuted, 
        bool entityKeepAspectRatio, 
        bool entityLooping, 
        bool entityHires, 
        float entityVolumeMult) {
        
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
        
        try {
            vidEntry = VidPlayerManager.GetPlayerFor(videoTarget);
        } catch (FileNotFoundException fex) {
            Logger.LogDetailed(fex);
            hires = false;
        }
    }

    public void Init() {
        if (checkDisposed()) return;
        videoPlayer!.IsLooped = looping;
        videoPlayer.IsMuted = muted;
        videoPlayer.Volume = 0; // Audio volume will be determined on first update instead
        videoPlayer.Play(vidEntry.video);
        hasWoken = true;
    }

    public void Update() {
        if (checkDisposed()) return;
        float normVolume = Settings.Instance.MusicVolume / 10f /* Max volume */ * volumeMult;
        normVolume = Math.Clamp(normVolume, 0f, 1f);
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (normVolume != videoPlayer!.Volume)
            videoPlayer.Volume = normVolume;
        if (Paused) {
            videoPlayer.Pause();
        } else {
            videoPlayer.Resume();
        }
    }

    public void Render() {
        int scalingFactor = hires ? 6 : 1;
        if (checkDisposed()) {
            fallback.Draw(Position * scalingFactor, Vector2.Zero, Color.White, MathF.Min(size.X / fallback.Width, size.Y / fallback.Height) * scalingFactor);
            return;
        }
        Texture2D currTexture = videoPlayer!.GetTexture();
        Rectangle dstRect;
        if (!keepAspectRatio) {
            dstRect = new Rectangle((int)Position.X * scalingFactor, (int)Position.Y * scalingFactor, (int)size.X * scalingFactor, (int)size.Y * scalingFactor);
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
            dstRect = new Rectangle((int)Position.X * scalingFactor, (int)Position.Y * scalingFactor, (int)finalSizeX * scalingFactor, (int)finalSizeY * scalingFactor);
        }
        Draw.SpriteBatch.Draw(currTexture, dstRect, Color.White);
    }

    public bool checkDisposed() {
        if (vidEntry == null) return true;
        if (vidEntry.videoPlayer.IsDisposed) {
            vidEntry = null;
            hires = false;
            return true;
        }
        return false;
    }

    public void Mark() {
        if (checkDisposed()) return;
        vidEntry!.MarkForCollection();
    }
    
    protected virtual void SaveState(Level newLevel) {
        if (checkDisposed()) return;
        videoPlayer!.Pause();
    }

    protected virtual void LoadState(Level newLevel) {
        if (checkDisposed()) return;
        vidEntry!.SaveFromCollection();
        videoPlayer!.Pause();
    }
    
    // About savestate support, we never clone this entity, we just pause it when it is removed on scene, it will autoplay on next Update
    // we also never release the VideoPlayer explicitly, gc will do the job for us
    // this is due to not having any clean way of knowing when an entity is in a savestate, other than ref counting, which is what the gc
    // does for us
    private static object? RSLASaveLoadAction;
    private static readonly Func<Type, bool> rsoPred = t => t.IsAssignableTo(typeof(VidPlayerCore));
    public static bool LoadingState { get; private set; }
    
    public static void RegisterSRTInterop() {
        if (SRTModImports.AddReturnSameObjectProcessor is { } rsop) {
            rsop(rsoPred);
        }
        if (SRTModImports.RegisterSaveLoadAction is { } rsla) {
            RSLASaveLoadAction = rsla(Save, Load, null, null, BeforeLoadState, null);
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
        foreach (VidPlayerCore vidPlayerCore in GetAllCores(level)) {
            vidPlayerCore.SaveState(level);
        }
    }
    
    private static void Load(Dictionary<Type, Dictionary<string, object>> ctx, Level level) {
        LoadingState = false;
        foreach (VidPlayerCore vidPlayerCore in GetAllCores(level)) {
            vidPlayerCore.LoadState(level);
        }
    }

    private static void BeforeLoadState(Level level) {
        LoadingState = true;
    }

    private static IEnumerable<VidPlayerCore> GetAllCores(Level level) {
        foreach (Entity vidPlayerEntity in level.Tracker.GetEntities<VidPlayerEntity>()) {
            yield return ((VidPlayerEntity) vidPlayerEntity).Core;
        }

        BackdropRenderer[] backdrops = [level.Background, level.Foreground];
        foreach (BackdropRenderer renderer in backdrops) {
            foreach (Backdrop backdrop in renderer.Backdrops) {
                if (backdrop is VidPlayerStyleground styleground) {
                    yield return styleground.Core;
                }
            }
        }
    }
    
}