using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.VidPlayer;

[CustomBackdrop("VidPlayer/VidPlayerStyleground")]
public sealed class VidPlayerStyleground : Backdrop {
    private VidPlayerCore? core;
    public VidPlayerCore? Core => core;
    
    private Scene? currentScene;
    private readonly BinaryPacker.Element data;
    
    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
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
            false /* TODO: hires stylegrounds */,
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
        if (core?.CheckDisposed() ?? true) { // Try to revive it
            Load();
        }
        core?.Update();
    }

    public override void Render(Scene scene) {
        base.Render(scene);
        core?.Render();
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
