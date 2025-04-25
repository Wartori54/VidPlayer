using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.VidPlayer;

[CustomBackdrop("VidPlayer/VidPlayerStyleground")]
public sealed class VidPlayerStyleground : Backdrop {
    private VidPlayerCore? core;
    public VidPlayerCore Core => core;
    
    private Scene? currentScene;
    private BinaryPacker.Element data;
    
    public VidPlayerStyleground(BinaryPacker.Element data) {
        this.data = data;
    }

    private void Load() {
        core?.Mark();
        core = new VidPlayerStylegroundCore(this, new Vector2(320, 180),
            data.Attr("video"),
            data.AttrBool("muted", true),
            data.AttrBool("keepAspectRatio", true),
            true /* always looping */,
            false /* TODO: hires stylegrounds */,
            data.AttrFloat("volumeMult", 1));
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
        if (core?.checkDisposed() ?? true) { // Try to revive it
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
        
        public VidPlayerStylegroundCore(VidPlayerStyleground owner, Vector2 entitySize, string videoTarget, bool entityIsMuted, bool entityKeepAspectRatio, bool entityLooping, bool entityHires, float entityVolumeMult) : base(entitySize, videoTarget, entityIsMuted, entityKeepAspectRatio, entityLooping, entityHires, entityVolumeMult) {
            this.owner = owner;
        }

        protected override bool Paused => owner.currentScene?.Paused ?? true;
        protected override Vector2 Position => Vector2.Zero;
    }

    // Styleground are updated in weird ways, and don't have tags, so lets hack it!
    private class UpdateFeederEntity : Entity {
        private readonly VidPlayerStyleground owner;
        public UpdateFeederEntity(VidPlayerStyleground owner) {
            this.owner = owner;
            Tag = global::Celeste.Tags.Global | global::Celeste.Tags.TransitionUpdate | global::Celeste.Tags.PauseUpdate;
        }

        public override void Update() {
            base.Update();
            owner.ConsistentUpdate(Scene);
        }
    }
}
