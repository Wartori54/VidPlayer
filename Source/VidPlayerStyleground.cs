using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod;
using MonoMod.Cil;
using System;

namespace Celeste.Mod.VidPlayer;

[CustomBackdrop("VidPlayer/VidPlayerStyleground")]
public sealed class VidPlayerStyleground : Backdrop {
    private VidPlayerCore? core;
    public VidPlayerCore? Core => core;
    
    private Scene? currentScene;
    private readonly BinaryPacker.Element data;

    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
        UseSpritebatch = false;
    }

    internal static void ILLevelRender(ILContext il) {
        ILCursor cursor = new(il);

        // (after SetRenderTarget(null), Clear())
        // Render BG hi-res stylegrounds
        // (Right after End())
        // Render FG hi-res stylegrounds
        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdnull(),
                instr => instr.MatchCallvirt<GraphicsDevice>("SetRenderTarget")) ||
                !cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<GraphicsDevice>("Clear"))) {
            throw new InvalidOperationException("Cannot find SetRenderTarget(null) and/or Clear()!");
        }

        // (after SetRenderTarget(null), Clear())
        // Render BG hi-res stylegrounds
        cursor.EmitLdarg0();
        cursor.EmitLdcI4(0); // emit false
        cursor.EmitDelegate(RenderHiresVPS);

        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SpriteBatch>("End"))) {
            throw new InvalidOperationException("Cannot find SpriteBatch.End()!");
        }

        // (Right after End())
        // Render contents of said separate RenderTarget
        cursor.EmitLdarg0();
        cursor.EmitLdcI4(1); // emit true
        cursor.EmitDelegate(RenderHiresVPS);
        Console.WriteLine(il);
    }

    private static void RenderHiresVPS(Level level, bool fg) {
        bool sbBegin = false;
        foreach (Backdrop backdrop in fg ? level.Foreground.Backdrops : level.Background.Backdrops) {
            if (backdrop is not VidPlayerStyleground vps || !(vps.core?.Hires ?? false)) continue;
            // XXX: This is a workaround that Maddie's Helping Hand also does. GameplayBuffers.Level is
            // cleared with BackgroundColor before drawing anything -- this means that background hires stylegrounds
            // will be completely blocked by the default black color.
            // A cleaner solution would probably be to replace the BackgroundColor in the original call to
            // Color.Transparent, and then draw the black background later when GameplayBuffers.Level is
            // rendered.
            //
            // The only drawback of this workaround (that I know of) is that, from the player/mapper's perspective,
            // the black background of the level will now be unaffected by colorgrading/etc.
            level.BackgroundColor = Color.Transparent;

            if (!sbBegin) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, GetMatrix());
                sbBegin = true;
            }

            if (vps.Visible) {
                vps.core?.Render();
            }
        }
        if (sbBegin)
            Draw.SpriteBatch.End();
    }

    private static Matrix GetMatrix() {
        Matrix matrix = Engine.ScreenMatrix;
        if (SaveData.Instance.Assists.MirrorMode) {
            matrix *= Matrix.CreateTranslation(-Engine.Viewport.Width, 0f, 0f);
            matrix *= Matrix.CreateScale(-1f, 1f, 1f);
        }
        return matrix;
    }

    private void Load() {
        core?.Mark();
        Color? chromaKey = null;
        string stringChromaKey = data.Attr("chromaKey", "");
        if (!string.IsNullOrEmpty(stringChromaKey)) {
            chromaKey = Calc.HexToColor(stringChromaKey);
        }
        VidPlayerCore.CoreConfig config = new(Vector2.Zero, data.AttrBool("muted", true),
            data.AttrBool("keepAspectRatio", true),
            true /* always looping */,
            data.AttrBool("hires", false),
            data.AttrFloat("volumeMult", 1),
            data.AttrFloat("globalAlpha"),
            data.AttrBool("centered", false),
            chromaKey,
            data.AttrFloat("chromaKeyBaseThr"), 
            data.AttrFloat("chromaKeyAlphaCorr"),
            data.AttrFloat("chromaKeySpill"));
        core = new VidPlayerStylegroundCore(this,
            data.Attr("video"),
            config);
        core.Init();
    }

    public override void Update(Scene scene) {
        base.Update(scene);
        // For some reason backdrops are not scene-aware :catplush:
        if (currentScene == null) {
            currentScene = scene;
            currentScene.Add(new UpdateFeederEntity(this));
            core?.Update();
        }
    }

    private void ConsistentUpdate(Scene scene) {
        if (core?.CanBeRevived() ?? true) { // Try to revive it
            Load();
        }
        core?.Update();
    }

    public override void Render(Scene scene) {
        if (!(core?.Hires ?? false)) {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
            base.Render(scene);
            core?.Render();
            Draw.SpriteBatch.End();
        }
    }

    public override void Ended(Scene scene) {
        base.Ended(scene);
        core?.Mark();
    }

    private class VidPlayerStylegroundCore : VidPlayerCore {
        private readonly VidPlayerStyleground owner;
        
        public VidPlayerStylegroundCore(VidPlayerStyleground owner, string videoTarget, CoreConfig config) 
            : base(videoTarget, config) {
            this.owner = owner;
        }

        protected override bool Paused => (owner.currentScene?.Paused ?? true) || !owner.Visible;
        protected override Vector2 Position => Vector2.Zero;

        protected override Level? CurrentLevel => owner.currentScene as Level;

        protected override Vector2 GetEntitySize() {
            if (ExCamModImports.GetCameraDimensions == null || owner.currentScene == null)
                return new Vector2(320, 180);
            return ExCamModImports.GetCameraDimensions.Invoke((Level)owner.currentScene);
        }

        protected override void RestartSpriteBatch() {
            if (config.hires) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, GetMatrix());
            } else {
                // UseSpritebatch is set to false, and Render makes its own one
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, GetMatrix());
            }
        }

        protected override void StopSpriteBatch() {
            Draw.SpriteBatch.End();
        }
    }

    // Styleground are updated in weird ways, and don't have tags, so lets hack it!
    private class UpdateFeederEntity : Entity {
        private readonly VidPlayerStyleground owner;
        public UpdateFeederEntity(VidPlayerStyleground owner) {
            this.owner = owner;
            Tag = global::Celeste.Tags.Global | global::Celeste.Tags.TransitionUpdate | global::Celeste.Tags.PauseUpdate;
        }

        public override void Update() {
            owner.ConsistentUpdate(Scene);
        }
    }
}
