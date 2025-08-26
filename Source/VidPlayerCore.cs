using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using VideoPlayer = Celeste.Mod.VidPlayer.FNA_Reimpl.VideoPlayer2;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public abstract class VidPlayerCore {
    private readonly Vector2 fixedEntitySize;
    private bool muted;
    private readonly bool keepAspectRatio;
    private readonly bool looping;
    private bool hires;
    private float volumeMult;
    private float globalAlpha;
    private readonly MTexture fallback;
    private VidPlayerManager.VidPlayerEntry? vidEntry;
    private bool hasWoken = false;
    private readonly bool centered;
    
    // For handyness
    internal VideoPlayer? videoPlayer => vidEntry?.videoPlayer;
    private float CurrScaleFactor => 6 * (CurrentLevel?.Zoom ?? 1);
    public bool Hires => hires;
    
    public bool Done => vidEntry == null || hasWoken && (videoPlayer?.State ?? (MediaState)(-1)) == MediaState.Stopped;
    public bool Muted {
        get => muted;
        set => muted = value;
    }

    public float VolumeMult {
        get => volumeMult;
        set => volumeMult = value;
    }

    public float GlobalAlpha {
        get => globalAlpha;
        set => globalAlpha = value;
    }
    
    protected abstract bool Paused { get; }
    
    protected abstract Vector2 Position { get; }
    
    protected abstract Level? CurrentLevel { get; }

    protected VidPlayerCore(Vector2 entitySize, 
        string videoTarget, 
        bool entityIsMuted, 
        bool entityKeepAspectRatio, 
        bool entityLooping, 
        bool entityHires, 
        float entityVolumeMult,
        float entityGlobalAlpha,
        bool centeredKeepRatio) {
        
        fixedEntitySize = entitySize;
        muted = entityIsMuted;
        keepAspectRatio = entityKeepAspectRatio;
        looping = entityLooping;
        hires = entityHires;
        volumeMult = entityVolumeMult;
        globalAlpha = entityGlobalAlpha;
        centered = centeredKeepRatio;

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
        if (CheckDisposed()) return;
        videoPlayer!.Stop();
        videoPlayer!.IsLooped = looping;
        videoPlayer.IsMuted = muted;
        videoPlayer.Volume = 0; // Audio volume will be determined on first update instead
        videoPlayer.Play(vidEntry!.video, vidEntry.usedHandle);
        videoPlayer.Pause();
        hasWoken = true;
    }

    public void Update() {
        if (CheckDisposed()) return;
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
        Vector2 size = GetEntitySize();
        float scalingFactor = hires ? CurrScaleFactor : 1;
        if (CheckDisposed()) {
            fallback.Draw(Position * scalingFactor, Vector2.Zero, Color.White * globalAlpha, MathF.Min(size.X / fallback.Width, size.Y / fallback.Height) * scalingFactor);
            return;
        }
        Texture2D currTexture = videoPlayer!.GetTexture();
        Rectangle dstRect;
        if (!keepAspectRatio) {
            dstRect = new Rectangle((int)(Position.X * scalingFactor), (int)(Position.Y * scalingFactor), (int)(size.X * scalingFactor), (int)(size.Y * scalingFactor));
        } else {
            float ratio = vidEntry!.video.Width / (float)vidEntry.video.Height;
            float finalPosX = Position.X;
            float finalPosY = Position.Y;
            float finalSizeX;
            float finalSizeY;
            if (size.X / size.Y > ratio) {
                finalSizeX = size.Y * ratio;
                finalSizeY = size.Y;
                if (centered)
                    finalPosX = Position.X + size.X/2 - finalSizeX/2;
            } else {
                finalSizeX = size.X;
                finalSizeY = size.X / ratio;
                if (centered)
                    finalPosY = Position.Y + size.Y/2 - finalSizeY/2;
            }
            dstRect = new Rectangle((int)(finalPosX * scalingFactor), (int)(finalPosY * scalingFactor), (int)(finalSizeX * scalingFactor), (int)(finalSizeY * scalingFactor));
        }
        Draw.SpriteBatch.Draw(currTexture, dstRect, Color.White * globalAlpha);
    }

    public bool CheckDisposed() {
        if (vidEntry == null) return true;
        if (vidEntry.videoPlayer.IsDisposed) {
            vidEntry = null;
            hires = false;
            return true;
        }
        return false;
    }

    public void Mark() {
        if (CheckDisposed()) return;
        videoPlayer!.Pause();
        vidEntry!.MarkForCollection();
    }

    public void Reset() {
        if (CheckDisposed()) return;
        videoPlayer!.Stop();
        Init();
    }

    protected virtual Vector2 GetEntitySize() {
        return fixedEntitySize;
    }
    
    protected virtual void SaveState(Level newLevel) {
        if (CheckDisposed()) return;
        videoPlayer!.Pause();
    }

    protected virtual void LoadState(Level newLevel) {
        if (CheckDisposed()) return;
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
                if (backdrop is VidPlayerStyleground styleground && styleground.Core != null) {
                    yield return styleground.Core;
                }
            }
        }
    }
    
}