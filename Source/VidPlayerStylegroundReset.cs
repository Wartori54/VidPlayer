using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.VidPlayer;

[CustomEntity("VidPlayer/VidPlayerStylegroundReset")]
public class VidPlayerStylegroundReset : Trigger {
    private readonly string stTag;

    public VidPlayerStylegroundReset(EntityData data, Vector2 offset) : base(data, offset) {
        stTag = data.String("tag", "");
    }

    public override void OnEnter(Player player) {
        base.OnEnter(player);
        if (string.IsNullOrWhiteSpace(stTag)) return;
        foreach (Backdrop backdrop in SceneAs<Level>().Background.Backdrops) {
            if (backdrop is VidPlayerStyleground vps && backdrop.Tags.Contains(stTag)) {
                vps.Core?.Reset();
            }
        }
    }
}
