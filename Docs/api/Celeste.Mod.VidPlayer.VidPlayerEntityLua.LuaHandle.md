# <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle"></a> Class VidPlayerEntityLua.LuaHandle

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

A handle usable in lua to manipulate VidPlayerEntities
Call `RemoveSelf` on this to remove the entity. This handle won't be functional after the call.
You can also control the player with the properties and methods below.

```csharp
public class VidPlayerEntityLua.LuaHandle
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VidPlayerEntityLua.LuaHandle](Celeste.Mod.VidPlayer.VidPlayerEntityLua.LuaHandle.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle__ctor_Celeste_Mod_VidPlayer_VidPlayerEntity_"></a> LuaHandle\(VidPlayerEntity\)

```csharp
public LuaHandle(VidPlayerEntity vidPlayerEntity)
```

#### Parameters

`vidPlayerEntity` [VidPlayerEntity](Celeste.Mod.VidPlayer.VidPlayerEntity.md)

## Properties

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_Done"></a> Done

Whether the video has finished, or always false if the video loops.

```csharp
public bool Done { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_Muted"></a> Muted

Whether audio is muted or playing.

```csharp
public bool Muted { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_Paused"></a> Paused

Pauses or resumes the video player.

```csharp
public bool Paused { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_Visible"></a> Visible

Whether the entity is or should be visible. Does not stop audio from playing. Use Pause for that.

```csharp
public bool Visible { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_RemoveSelf"></a> RemoveSelf\(\)

```csharp
public void RemoveSelf()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntityLua_LuaHandle_Reset"></a> Reset\(\)

Resets the video player to the start of the video.

```csharp
public void Reset()
```

