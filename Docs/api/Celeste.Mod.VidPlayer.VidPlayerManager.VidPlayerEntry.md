# <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry"></a> Class VidPlayerManager.VidPlayerEntry

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
public record VidPlayerManager.VidPlayerEntry : IDisposable, IEquatable<VidPlayerManager.VidPlayerEntry>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VidPlayerManager.VidPlayerEntry](Celeste.Mod.VidPlayer.VidPlayerManager.VidPlayerEntry.md)

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IEquatable<VidPlayerManager.VidPlayerEntry\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Fields

### <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry_video"></a> video

```csharp
public readonly Video video
```

#### Field Value

 Video

### <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry_videoPlayer"></a> videoPlayer

```csharp
public readonly VideoPlayer videoPlayer
```

#### Field Value

 VideoPlayer

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry_Create_System_String_"></a> Create\(string\)

```csharp
public static VidPlayerManager.VidPlayerEntry Create(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [VidPlayerManager](Celeste.Mod.VidPlayer.VidPlayerManager.md).[VidPlayerEntry](Celeste.Mod.VidPlayer.VidPlayerManager.VidPlayerEntry.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry_Dispose"></a> Dispose\(\)

Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

```csharp
public void Dispose()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerManager_VidPlayerEntry_Finalize"></a> \~VidPlayerEntry\(\)

```csharp
protected ~VidPlayerEntry()
```

