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
    private static VirtualRenderTarget? tempFGRenderTarget;

    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
    }

    // hi-res stuff
    public static void LoadHooks() {
        IL.Celeste.Level.Render += onLevelRender;
    }

    public static void UnloadHooks() {
        IL.Celeste.Level.Render -= onLevelRender;
    }

    private static void onLevelRender(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        // Basically, the contents of the backbuffer are discarded when it is set as the active RenderTarget.
        // And since the active RenderTarget changes in core.Render() if we're chromakeying, this is an issue for
        // rendering FG stylegrounds since it'll clear the backbuffer if we're currently drawing there.
        // I've done some googling on why this happens, and it seems to be unavoidable -- SetRenderTarget(null)
        // internally binds the swap chain's backbuffer, and its contents are undefined after rebinding.
        // This *could* be worked around at a lower level, but it's probably going to be extremely messy
        // and involve digging into DirectX, which is quite a terrible idea.
        // So my solution is just another RenderTarget.
        //
        // (right before SetRenderTarget(null))
        // Pre-render all FG hi-res stylegrounds into a separate RenderTarget
        // (after SetRenderTarget(null), Clear())
        // Render BG hi-res stylegrounds
        // (Right after End())
        // Render contents of said separate RenderTarget
        if (!cursor.TryGotoNext(MoveType.Before, instr => instr.MatchLdnull(), instr => instr.MatchCallvirt<GraphicsDevice>("SetRenderTarget"))) {
            throw new InvalidOperationException("Cannot find SetRenderTarget(null)!");
        }

        // (right before SetRenderTarget(null))
        // Pre-render all FG hi-res stylegrounds into a separate RenderTarget
        cursor.EmitDelegate<Action>(levelRender_prerenderFG);

        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdnull(),
                instr => instr.MatchCallvirt<GraphicsDevice>("SetRenderTarget")) ||
                !cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<GraphicsDevice>("Clear"))) {
            throw new InvalidOperationException("Cannot find SetRenderTarget(null) and/or Clear()!");
        }

        // (after SetRenderTarget(null), Clear())
        // Render BG hi-res stylegrounds
        cursor.EmitDelegate<Action>(levelRender_renderBG);

        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SpriteBatch>("End"))) {
            throw new InvalidOperationException("Cannot find SpriteBatch.End()!");
        }

        // (Right after End())
        // Render contents of said separate RenderTarget
        cursor.EmitDelegate<Action>(levelRender_renderFGRT);
    }

    private static void levelRender_prerenderFG() {
        tempFGRenderTarget ??= VirtualContent.CreateRenderTarget(nameof(tempFGRenderTarget), Celeste.TargetWidth, Celeste.TargetHeight);
        Engine.Instance.GraphicsDevice.SetRenderTarget(tempFGRenderTarget);
        Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

        renderHires(fg: true);
    }

    private static void levelRender_renderBG() {
        renderHires(fg: false);
    }

    private static void levelRender_renderFGRT() {
        Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, getMatrix());
        Draw.SpriteBatch.Draw((RenderTarget2D)tempFGRenderTarget, Vector2.Zero, Color.White);
        Draw.SpriteBatch.End();
    }

    private static void renderHires(bool fg) {
        // Note: If there are no VidPlayerStylegrounds, then this Begin/End is pointless... does that even matter?
        Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, getMatrix());
        if (Engine.Scene is Level level) {
            foreach (Backdrop backdrop in (fg ? level.Foreground.Backdrops : level.Background.Backdrops)) {
                // I haven't ran into issues regarding this, so silence these warnings for now.
#pragma warning disable CS8602 // Dereference of a possibly null reference
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type
                if (backdrop != null && backdrop is VidPlayerStyleground && ((backdrop as VidPlayerStyleground).core?.Hires ?? false)) {
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

                    VidPlayerStyleground styleground = (backdrop as VidPlayerStyleground);
                    if (styleground.Visible) {
                        styleground.core?.Render();
                    }
                }
#pragma warning restore CS8602
#pragma warning restore CS8600
            }
        }
        Draw.SpriteBatch.End();
    }

    private static Matrix getMatrix() {
        Matrix matrix = Engine.ScreenMatrix;
        if (SaveData.Instance.Assists.MirrorMode) {
            matrix *= Matrix.CreateTranslation(-Engine.Viewport.Width, 0f, 0f);
            matrix *= Matrix.CreateScale(-1f, 1f, 1f);
        }
        return matrix;
    }
    // end of hi-res stuff

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
            base.Render(scene);
            core?.Render();
        }
    }

    public override void Ended(Scene scene) {
        base.Ended(scene);
        tempFGRenderTarget?.Dispose();
        tempFGRenderTarget = null;
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
                return new Vector2(320, 160);
            return ExCamModImports.GetCameraDimensions.Invoke((Level)owner.currentScene);
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
