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
    private static VirtualRenderTarget? tempHiresRenderTarget;

    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
    }

    // hi-res hook stuff
    // thank you maddie's helping hand HD parallax
    public static void LoadHooks() {
        IL.Celeste.Level.Render += onLevelRender;
    }

    public static void UnloadHooks() {
        IL.Celeste.Level.Render -= onLevelRender;
    }

    private static void onLevelRender(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        // all of this manipulation and the temp rendertarget is needed cause swapping rendertargets clears the backbuffer
        if (cursor.TryGotoNext(instr => instr.MatchLdnull(), instr => instr.MatchCallvirt<GraphicsDevice>("SetRenderTarget"))
            && cursor.TryGotoNext(MoveType.Before, instr => instr.MatchCallvirt<SpriteBatch>("Begin"))) {
            cursor.EmitDelegate<Action>(() => {
                tempHiresRenderTarget ??= VirtualContent.CreateRenderTarget(nameof(tempHiresRenderTarget), Celeste.TargetWidth, Celeste.TargetHeight);
                Engine.Instance.GraphicsDevice.SetRenderTarget(tempHiresRenderTarget);
                Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
            });

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SpriteBatch>("Begin"))) {
                cursor.EmitDelegate<Action>(() => renderIfHires(fg: false));
            }

            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SpriteBatch>("End"))) {
                cursor.EmitDelegate<Action>(() => {
                    renderIfHires(fg: true);
                    Engine.Instance.GraphicsDevice.SetRenderTarget(null);
                    Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
                    Draw.SpriteBatch.Draw((RenderTarget2D)tempHiresRenderTarget, Vector2.Zero, Color.White);
                    Draw.SpriteBatch.End();
                });
            }
        }
    }

    private static void renderIfHires(bool fg) {
        if (Engine.Scene is Level level) {
            foreach (Backdrop backdrop in (fg ? level.Foreground.Backdrops : level.Background.Backdrops)) {
                if (backdrop is VidPlayerStyleground && ((backdrop as VidPlayerStyleground).core?.Hires ?? false)) {
                    Color old = level.BackgroundColor;
                    level.BackgroundColor = Color.Transparent;
                    renderHires((backdrop as VidPlayerStyleground));
                    level.BackgroundColor = old;
                }
            }
        }
    }

    private static void renderHires(VidPlayerStyleground styleground) {
        if (!styleground.Visible) {
            return;
        }

        Matrix matrix = Engine.ScreenMatrix;
        if (SaveData.Instance.Assists.MirrorMode) {
            matrix *= Matrix.CreateTranslation(-Engine.Viewport.Width, 0f, 0f);
            matrix *= Matrix.CreateScale(-1f, 1f, 1f);
        }

        Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, matrix);
        styleground.core?.Render();
        Draw.SpriteBatch.End();
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
        tempHiresRenderTarget?.Dispose();
        tempHiresRenderTarget = null;
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
