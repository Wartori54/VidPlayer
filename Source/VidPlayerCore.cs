using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using VideoPlayer = Celeste.Mod.VidPlayer.FNA_Reimpl.VideoPlayer2;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public abstract class VidPlayerCore {
    private static Effect? _chromaKeyShader = null;
    private static Effect ChromaKeyShader {
        get {
            if (_chromaKeyShader != null) return _chromaKeyShader;

            if (Engine.Graphics == null || Engine.Graphics.GraphicsDevice == null) {
                throw new InvalidOperationException("Tried to obtain the chromaKeyShader too early!");
            }

            if (!Everest.Content.TryGet("Effects/ChromaKey.cso", out ModAsset chromaKeyAsset)) {
                throw new InvalidOperationException("No ChromaKey.cso file found!");
            }
            
            return _chromaKeyShader = new Effect(Engine.Graphics.GraphicsDevice, chromaKeyAsset.Data);
        }
    }


    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct CoreConfig {
        public readonly Vector2 fixedEntitySize;
        public bool muted;
        public readonly bool keepAspectRatio;
        public readonly bool looping;
        public bool hires;
        public float volumeMult;
        public float globalAlpha;
        public readonly bool centered;
        public readonly Color? chromaKey;
        public readonly float chromaBaseThr;
        public readonly float chromaAlphaCorr;
        public readonly float chromaSpill;
        
        public CoreConfig(Vector2 fixedEntitySize, bool muted, bool keepAspectRatio, bool looping, bool hires, float volumeMult, float globalAlpha, bool centered, Color? chromaKey, float chromaBaseThr, float chromaAlphaCorr, float chromaSpill) {
            this.fixedEntitySize = fixedEntitySize;
            this.muted = muted;
            this.keepAspectRatio = keepAspectRatio;
            this.looping = looping;
            this.hires = hires;
            this.volumeMult = volumeMult;
            this.globalAlpha = globalAlpha;
            this.centered = centered;
            this.chromaKey = chromaKey;
            this.chromaBaseThr = chromaBaseThr;
            this.chromaAlphaCorr = chromaAlphaCorr;
            this.chromaSpill = chromaSpill;
        }
    }
    
    private bool hasWoken = false;
    private VidPlayerManager.VidPlayerEntry? vidEntry;
    private readonly MTexture fallback;
    protected CoreConfig config;
    private readonly bool loadingFailure;

    // For handyness
    internal VideoPlayer? videoPlayer => vidEntry?.videoPlayer;
    private float CurrScaleFactor => 6 * (CurrentLevel?.Zoom ?? 1);
    public bool Hires => config.hires;
    
    public bool Done => vidEntry == null || hasWoken && (videoPlayer?.State ?? (MediaState)(-1)) == MediaState.Stopped;
    public bool Muted {
        get => config.muted;
        set => config.muted = value;
    }

    public float VolumeMult {
        get => config.volumeMult;
        set => config.volumeMult = value;
    }

    public float GlobalAlpha {
        get => config.globalAlpha;
        set => config.globalAlpha = value;
    }
    
    protected abstract bool Paused { get; }
    
    protected abstract Vector2 Position { get; }
    
    protected abstract Level? CurrentLevel { get; }

    protected VidPlayerCore(string videoTarget, CoreConfig entityConfig) {
        config = entityConfig;

        // Switch to this once it hits main
        // if (SRTModImports.IgnoreSaveState is { } cb) {
        //     cb(this, true);
        // }
        
        fallback = GFX.Game["__fallback"];
        
        try {
            vidEntry = VidPlayerManager.GetPlayerFor(videoTarget);
        } catch (FileNotFoundException fex) {
            Logger.LogDetailed(fex);
            config.hires = false;
            loadingFailure = true;
        }
    }

    public void Init() {
        if (CheckDisposed()) return;
        videoPlayer!.Stop();
        videoPlayer!.IsLooped = config.looping;
        videoPlayer.IsMuted = config.muted;
        videoPlayer.Volume = 0; // Audio volume will be determined on first update instead
        videoPlayer.Play(vidEntry!.video, vidEntry.usedHandle);
        videoPlayer.Pause();
        hasWoken = true;
    }

    public void Update() {
        if (CheckDisposed()) return;
        float normVolume = Settings.Instance.MusicVolume / 10f /* Max volume */ * config.volumeMult;
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
        float scalingFactor = config.hires ? CurrScaleFactor : 1;
        if (CheckDisposed()) {
            fallback.Draw(Position * scalingFactor, Vector2.Zero, Color.White * config.globalAlpha, MathF.Min(size.X / fallback.Width, size.Y / fallback.Height) * scalingFactor);
            return;
        }
        Texture2D currTexture = videoPlayer!.GetTexture();
        Rectangle dstRect;
        if (!config.keepAspectRatio) {
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
                if (config.centered)
                    finalPosX = Position.X + size.X/2 - finalSizeX/2;
            } else {
                finalSizeX = size.X;
                finalSizeY = size.X / ratio;
                if (config.centered)
                    finalPosY = Position.Y + size.Y/2 - finalSizeY/2;
            }
            dstRect = new Rectangle((int)(finalPosX * scalingFactor), (int)(finalPosY * scalingFactor), (int)(finalSizeX * scalingFactor), (int)(finalSizeY * scalingFactor));
        }
        if (config.chromaKey == null) {
            Draw.SpriteBatch.Draw(currTexture, dstRect, Color.White * config.globalAlpha);
            return;
        }
        
        
        // Steal the current transformMatrix to pass it to the effect, this is needed for its vertex shader
        Matrix view = Draw.SpriteBatch.transformMatrix;
        StopSpriteBatch();
        ChromaKeyShader.Parameters["key"].SetValue(new Vector4() {
            X = config.chromaKey.Value.R/255f,
            Y = config.chromaKey.Value.G/255f,
            Z = config.chromaKey.Value.B/255f,
            W = 1f
        });
        ChromaKeyShader.Parameters["base_thr"].SetValue(config.chromaBaseThr);
        ChromaKeyShader.Parameters["alpha_correction"].SetValue(config.chromaAlphaCorr);
        ChromaKeyShader.Parameters["spill"].SetValue(config.chromaSpill);

        int width = Engine.Graphics.GraphicsDevice.Viewport.Width;
        int height = Engine.Graphics.GraphicsDevice.Viewport.Height;
        Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

        ChromaKeyShader.Parameters["view_projection"].SetValue(view * projection);
        Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, ChromaKeyShader);
        Draw.SpriteBatch.Draw(currTexture, dstRect, Color.White * config.globalAlpha);
        Draw.SpriteBatch.End(); // Technically useless but oh well
        RestartSpriteBatch();
    }

    public bool CanBeRevived() {
        return !loadingFailure && CheckDisposed();
    }

    public bool CheckDisposed() {
        if (vidEntry == null) return true;
        if (vidEntry.videoPlayer.IsDisposed) {
            vidEntry = null;
            config.hires = false;
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
        return config.fixedEntitySize;
    }

    protected abstract void RestartSpriteBatch();
    protected virtual void StopSpriteBatch() {
        Draw.SpriteBatch.End();
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