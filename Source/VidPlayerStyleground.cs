using Celeste.Mod.Backdrops;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.VidPlayer;

[CustomBackdrop("VidPlayer/VidPlayerStyleground")]
public class VidPlayerStyleground : Backdrop {
    public VidPlayerEntity Entity { get; private set; }
    public VidPlayerStyleground(BinaryPacker.Element data) {
        Entity = new VidPlayerEntity(Vector2.Zero,
            new Vector2(320, 180),
            data.Attr("video"),
            data.AttrBool("muted", true),
            data.AttrBool("keepAspectRatio", true),
            true /* always looping */,
            false /* TODO: hires stylegrounds */,
            data.AttrFloat("volumeMult", 1), Vector2.Zero);
        Entity.sceneless = true;
        Entity.Awake(null!);
        
    }

    public override void Update(Scene scene) {
        base.Update(scene);
        Entity.Update();
    }

    public override void Render(Scene scene) {
        base.Render(scene);
        Entity.Render();
    }
}