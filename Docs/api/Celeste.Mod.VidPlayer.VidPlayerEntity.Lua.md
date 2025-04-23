# <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Lua"></a> Class VidPlayerEntity.Lua

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
public static class VidPlayerEntity.Lua
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VidPlayerEntity.Lua](Celeste.Mod.VidPlayer.VidPlayerEntity.Lua.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Lua_SpawnFullParameterized_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Single_"></a> SpawnFullParameterized\(string, int, int, int, int, bool, bool, bool, bool, float\)

Spawns a video entity with all the parameters available.

```csharp
public static VidPlayerEntity.Lua.LuaHandle SpawnFullParameterized(string videoTarget, int x, int y, int width, int height, bool muted, bool hires, bool keepAspectRatio, bool looping, float volumeMult)
```

#### Parameters

`videoTarget` [string](https://learn.microsoft.com/dotnet/api/system.string)

The video file path.

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

X position relative to the room we are currently in.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Y position relative to the room we are currently in.

`width` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Width of the video, in in-game pixels.

`height` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Height of the video, in in-game pixels.

`muted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether audio plays.

`hires` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the video will be downscaled to celeste's gameplay resolution.

`keepAspectRatio` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether to stretch the video to fit the given width/height or to keep the ratio width/height ratio from the video file.

`looping` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether playback loops forever or stops after the video is over.

`volumeMult` [float](https://learn.microsoft.com/dotnet/api/system.single)

Real value to multiply the volume with, ranging from 0 to 1.

#### Returns

 [VidPlayerEntity](Celeste.Mod.VidPlayer.VidPlayerEntity.md).[Lua](Celeste.Mod.VidPlayer.VidPlayerEntity.Lua.md).[LuaHandle](Celeste.Mod.VidPlayer.VidPlayerEntity.Lua.LuaHandle.md)

A handle you can use in Lua. See LuaHandle down below.

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Lua_SpawnLoop_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_"></a> SpawnLoop\(string, int, int, int, int, bool, bool\)

Spawns a looping video entity, playing the given video file.

```csharp
public static VidPlayerEntity.Lua.LuaHandle SpawnLoop(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true)
```

#### Parameters

`videoTarget` [string](https://learn.microsoft.com/dotnet/api/system.string)

The video file path.

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

X position relative to the room we are currently in.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Y position relative to the room we are currently in.

`width` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Width of the video, in in-game pixels.

`height` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Height of the video, in in-game pixels.

`muted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether audio plays.

`hires` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the video will be downscaled to celeste's gameplay resolution.

#### Returns

 [VidPlayerEntity](Celeste.Mod.VidPlayer.VidPlayerEntity.md).[Lua](Celeste.Mod.VidPlayer.VidPlayerEntity.Lua.md).[LuaHandle](Celeste.Mod.VidPlayer.VidPlayerEntity.Lua.LuaHandle.md)

A handle you can use in Lua. See LuaHandle down below.

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Lua_SpawnOneShot_System_String_System_Int32_System_Int32_System_Int32_System_Int32_System_Boolean_System_Boolean_"></a> SpawnOneShot\(string, int, int, int, int, bool, bool\)

Spawns a video entity that will play a video until it ends, removing itself afterwards.

```csharp
public static IEnumerator SpawnOneShot(string videoTarget, int x, int y, int width, int height, bool muted = false, bool hires = true)
```

#### Parameters

`videoTarget` [string](https://learn.microsoft.com/dotnet/api/system.string)

The video file path.

`x` [int](https://learn.microsoft.com/dotnet/api/system.int32)

X position relative to the room we are currently in.

`y` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Y position relative to the room we are currently in.

`width` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Width of the video, in in-game pixels.

`height` [int](https://learn.microsoft.com/dotnet/api/system.int32)

Height of the video, in in-game pixels.

`muted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether audio plays.

`hires` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Whether the video will be downscaled to celeste's gameplay resolution.

#### Returns

 [IEnumerator](https://learn.microsoft.com/dotnet/api/system.collections.ienumerator)

An IEnumerator you can `coroutine.yield` in lua to execute it.

