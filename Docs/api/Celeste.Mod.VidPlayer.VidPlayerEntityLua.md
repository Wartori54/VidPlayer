# <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua"></a> Class VidPlayerEntityLua

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
public class VidPlayerEntityLua
```

#### Inheritance

object ← 
[VidPlayerEntityLua](Celeste.Mod.VidPlayer.VidPlayerEntityLua.md)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_SpawnFullParameterized_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Single_"></a> SpawnFullParameterized\(string, int, int, int, int, bool, bool, bool, bool, float\)

Spawns a video entity with all the parameters available.

```csharp
public static VidPlayerEntityLua.LuaHandle SpawnFullParameterized(string videoTarget, int x, int y, int width, int height, bool muted, bool hires, bool keepAspectRatio, bool looping, float volumeMult)
```

#### Parameters

`videoTarget` string

The video file path.

`x` int

X position relative to the room we are currently in.

`y` int

Y position relative to the room we are currently in.

`width` int

Width of the video, in in-game pixels.

`height` int

Height of the video, in in-game pixels.

`muted` bool

Whether audio plays.

`hires` bool

Whether the video will be downscaled to celeste's gameplay resolution.

`keepAspectRatio` bool

Whether to stretch the video to fit the given width/height or to keep the ratio width/height ratio from the video file.

`looping` bool

Whether playback loops forever or stops after the video is over.

`volumeMult` float

Real value to multiply the volume with, ranging from 0 to 1.

#### Returns

 [VidPlayerEntityLua](Celeste.Mod.VidPlayer.VidPlayerEntityLua.md).[LuaHandle](Celeste.Mod.VidPlayer.VidPlayerEntityLua.LuaHandle.md)

A handle you can use in Lua. See LuaHandle for more info.

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_SpawnLoop_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_"></a> SpawnLoop\(string, int, int, int, int, bool, bool\)

Spawns a looping video entity, playing the given video file.

```csharp
public static VidPlayerEntityLua.LuaHandle SpawnLoop(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true)
```

#### Parameters

`videoTarget` string

The video file path.

`x` int

X position relative to the room we are currently in.

`y` int

Y position relative to the room we are currently in.

`width` int

Width of the video, in in-game pixels.

`height` int

Height of the video, in in-game pixels.

`muted` bool

Whether audio plays.

`hires` bool

Whether the video will be downscaled to celeste's gameplay resolution.

#### Returns

 [VidPlayerEntityLua](Celeste.Mod.VidPlayer.VidPlayerEntityLua.md).[LuaHandle](Celeste.Mod.VidPlayer.VidPlayerEntityLua.LuaHandle.md)

A handle you can use in Lua. See LuaHandle for more info.

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_SpawnOneShot_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_"></a> SpawnOneShot\(string, int, int, int, int, bool, bool\)

Spawns a video entity that will play a video until it ends, removing itself afterwards.

```csharp
public static IEnumerator SpawnOneShot(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true)
```

#### Parameters

`videoTarget` string

The video file path.

`x` int

X position relative to the room we are currently in.

`y` int

Y position relative to the room we are currently in.

`width` int

Width of the video, in in-game pixels.

`height` int

Height of the video, in in-game pixels.

`muted` bool

Whether audio plays.

`hires` bool

Whether the video will be downscaled to celeste's gameplay resolution.

#### Returns

 IEnumerator

An IEnumerator you can `coroutine.yield` in lua to execute it.

