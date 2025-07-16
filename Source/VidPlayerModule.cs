using System;
using System.Reflection;
using KeraLua;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.ModInterop;
using Scene = Monocle.Scene;

namespace Celeste.Mod.VidPlayer;

public class VidPlayerModule : EverestModule {
    public static VidPlayerModule Instance { get; set; } = null!;

    public override Type SettingsType => typeof(VidPlayerModuleSettings);
    public static VidPlayerModuleSettings Settings => (VidPlayerModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(VidPlayerModuleSession);
    public static VidPlayerModuleSession Session => (VidPlayerModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(VidPlayerModuleSaveData);
    public static VidPlayerModuleSaveData SaveData => (VidPlayerModuleSaveData)Instance._SaveData;

    public VidPlayerModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(VidPlayerModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(VidPlayerModule), LogLevel.Info);
#endif
    }

    private static MethodBase GC_Collect = typeof(GC).GetMethod(nameof(GC.Collect), []) ?? throw new InvalidOperationException("Cannot find GC.Collect()!");
    public override void Load() {
        CheckFNAVersion();
        typeof(SRTModImports).ModInterop();
        typeof(ExCamModImports).ModInterop();
        VidPlayerCore.RegisterSRTInterop();

        // These completely break savestates...
        // On.Monocle.Engine.OnSceneTransition += EngineOnOnSceneTransition;
        // On.Celeste.Level._GCCollect += LevelOn_GCCollect;
        // IL.Celeste.Level.Reload += ILLevelOnReload;
        On.Monocle.Engine.Update += EngineOnUpdate;
    }
    
    private static void EngineOnUpdate(On.Monocle.Engine.orig_Update orig, Engine self, GameTime dt) {
        orig(self, dt);
        VidPlayerManager.Collect();
    }

    public override void Unload() {
        VidPlayerCore.UnregisterSRTInterop();
        // On.Monocle.Engine.OnSceneTransition -= EngineOnOnSceneTransition;
        // On.Celeste.Level._GCCollect -= LevelOn_GCCollect;
        // IL.Celeste.Level.Reload -= ILLevelOnReload;
    }
    
    private static void ILLevelOnReload(ILContext il) {
        ILCursor ilCursor = new(il);
        if (!ilCursor.TryGotoNext(MoveType.Before, i => i.MatchCall(GC_Collect))) {
            throw new InvalidOperationException("Cannot find GC.Collect()!");
        }

        ilCursor.EmitDelegate(LuaCollect);
    }

    private static void LuaCollect() {
        Everest.LuaLoader.Context.State.GarbageCollector(LuaGC.Collect, 0);
    }
    
    private static void LevelOn_GCCollect(On.Celeste.Level.orig__GCCollect orig) {
        LuaCollect();
        orig();
    }

    private static void EngineOnOnSceneTransition(On.Monocle.Engine.orig_OnSceneTransition orig, Monocle.Engine self, Scene from, Scene to) {
        LuaCollect();
        orig(self, from, to);
    }
    
    private void CheckFNAVersion() {
        Version? fnaVersion = typeof(Game).Assembly.GetName().Version;
        if (fnaVersion == null) {
            Logger.Log(LogLevel.Error, nameof(VidPlayer), "Version-less FNA?");
            return;
        }
        if (fnaVersion.Major != 23 || fnaVersion.Minor != 3) {
            Logger.Log(LogLevel.Error, nameof(VidPlayer), $"Wrong FNA version detected (expected: 23.3.x.x, found: {fnaVersion}), " +
                                                          $"incompatibilities may occur!");
        }
    }

    // Just some interesting debugging code in case i ever need it again
    // private static int CollectObjectHooker(Func<IntPtr, int> orig, IntPtr ptr) {
    //     KeraLua.Lua state = KeraLua.Lua.FromIntPtr(ptr);
    //     int udata = state.RawNetObj(1);
    //     Console.Write($"collecting object {udata} ");
    //     if (ObjectTranslatorPool.Instance.Find(state)._objects.TryGetValue(udata, out object? obj)) {
    //         Console.Write($"in _objects as {obj}");
    //         if (obj is VidPlayerEntity ve) {
    //             Console.Write($" with id {ve.id}");
    //         }
    //     }
    //     Console.WriteLine();
    //     
    //     return orig(ptr);
    // }
    
    // private static void LevelOnUpdate(On.Celeste.Level.orig_Update orig, Level self) {
    //     if (Input.MenuJournal.Pressed) {
    //         object[] objs;
    //         using (StreamReader reader = File.OpenText("./Mods/VidPlayer/Maps/VidPlayer/testcutscene.lua"))
    //             objs = ownLua!.DoString(reader.ReadToEnd(), "Maps/VidPlayer/testcutscene.lua");
    //         ((LuaFunction)objs[0]).Call();
    //         // LuaCoroutine coroutine = ((objs[2] as LuaFunction)!.Call((LuaFunction)objs[0])[0] as LuaCoroutine)!;
    //         // Coroutine co = new(coroutine);
    //         // co.RemoveOnComplete = true;
    //         // Entity e = new();
    //         // e.Add(co);
    //         // self.Add(e);
    //     }
    //     if (Input.Jump.Pressed) {
    //         Console.WriteLine(VidPlayerEntity.bornCounter);
    //         foreach (KeyValuePair<int, object> kvp in ownLua!._translator._objects) {
    //             if (kvp.Value is VidPlayerEntity vpe) {
    //                 Console.WriteLine($"vpe at id {kvp.Key} with id {vpe.id}");
    //             }
    //         }
    //         Console.WriteLine($"Lua stack size: {ownLua.State.GetTop()}");
    //     }
    //     orig(self);
    // }
}
