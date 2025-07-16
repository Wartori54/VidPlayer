using System;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;
// ReSharper disable UnassignedField.Global

namespace Celeste.Mod.VidPlayer;

[ModImportName("ExtendedCameraDynamics")]
public class ExCamModImports {
    public static Func<Level, Vector2>? GetCameraDimensions;
}