using System;
using System.Collections.Generic;
using Monocle;
using MonoMod.ModInterop;
// ReSharper disable UnassignedField.Global

namespace Celeste.Mod.VidPlayer;

[ModImportName("SpeedrunTool.SaveLoad")]
public class SRTModImports {
    public static Action<Func<Type, bool>>? AddReturnSameObjectProcessor;

    public static Action<Func<Type, bool>>? RemoveReturnSameObjectProcessor;

    public static Action<Entity, bool>? IgnoreSaveState;

    public static Func<Type, string[], object>? RegisterStaticTypes;
    
    public static Func<Action<Dictionary<Type, Dictionary<string, object>>, Level>,
        Action<Dictionary<Type, Dictionary<string, object>>, Level>, Action?,
        Action<Level>?, Action<Level>?, Action?, object>? RegisterSaveLoadAction;

    public static Action<object>? Unregister;
}
