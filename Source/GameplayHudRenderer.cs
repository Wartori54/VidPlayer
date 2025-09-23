using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.VidPlayer;

public class GameplayHudRenderer : Renderer {
    public static BitTag GameplayHud = null!;
    public static readonly GameplayHudRenderer Instance = new();

    public override void Render(Scene scene) {
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, GetMatrix());
        scene.Entities.RenderOnly(GameplayHud);
        Draw.SpriteBatch.End();
    }
    
    public static Matrix GetMatrix() {
        Matrix matrix = Engine.ScreenMatrix;
        if (SaveData.Instance.Assists.MirrorMode) {
            matrix *= Matrix.CreateTranslation(-Engine.Viewport.Width, 0f, 0f);
            matrix *= Matrix.CreateScale(-1f, 1f, 1f);
        }
        return matrix;
    }

    internal static void ILLevelRender(ILContext il) {
        ILCursor cursor = new(il);
        if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg0(), i => i.MatchLdfld(typeof(Level), nameof(Level.SubHudRenderer)))) {
            throw new InvalidOperationException("Cannot find this.SubHudRenderer!");
        }

        cursor.EmitLdarg0();
        cursor.EmitDelegate(RenderForScene);
    }

    private static void RenderForScene(Scene scene) {
        Instance.Render(scene);
    }

    internal static void ILGameplayRenderer(ILContext il) {
        ILCursor cursor = new(il);

        if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt(typeof(EntityList), "RenderExcept"))) {
            throw new InvalidOperationException("Cannot find scene.Entities.RenderExcept(int32)!");
        }
        
        cursor.EmitLdsfld(typeof(GameplayHudRenderer).GetField(nameof(GameplayHud))!);
        cursor.EmitCall(typeof(BitTag).GetMethod("op_Implicit")!);
        cursor.EmitOr();
    }
}