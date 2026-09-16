using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste.Mod.VidPlayer;

[CustomBackdrop("VidPlayer/VidPlayerStyleground")]
public sealed class VidPlayerStyleground : Backdrop, IVidPlayerStyleground {
    private VidPlayerCore? core;
    public VidPlayerCore? Core => core;
    bool IVidPlayerStyleground.Visible => base.Visible;

    private Scene? currentScene;
    private readonly BinaryPacker.Element data;

    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
        UseSpritebatch = false;
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
            data.AttrFloat("globalAlpha", 1F),
            data.AttrBool("centered", false),
            chromaKey,
            data.AttrFloat("chromaKeyBaseThr"), 
            data.AttrFloat("chromaKeyAlphaCorr"),
            data.AttrFloat("chromaKeySpill"),
            data.AttrBool("unpausable"));
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

    public void RenderHires(Scene scene) {
        core?.Render();
    }

    public override void Ended(Scene scene) {
        base.Ended(scene);
        core?.Mark();
    }

    public class VidPlayerStylegroundCore : VidPlayerCore {
        private readonly VidPlayerStyleground owner;
        
        public VidPlayerStylegroundCore(VidPlayerStyleground owner, string videoTarget, CoreConfig config) 
            : base(videoTarget, config) {
            this.owner = owner;
        }

        protected override bool Paused => owner.currentScene?.Paused ?? true;
        protected override bool ForcePaused => !owner.Visible || owner.currentScene == null;
        public override Vector2 Position => Vector2.Zero;

        protected override Level? CurrentLevel => owner.currentScene as Level;

        internal override Vector2 GetEntitySize() {
            if (ExCamModImports.GetCameraDimensions == null || owner.currentScene == null)
                return new Vector2(320, 180);
            return ExCamModImports.GetCameraDimensions.Invoke((Level)owner.currentScene);
        }

        protected override void RestartSpriteBatch() {
            if (Hires) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, GameplayHudRenderer.GetMatrix());
            } else {
                // UseSpritebatch is set to false, and Render makes its own one
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, GameplayHudRenderer.GetMatrix());
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
