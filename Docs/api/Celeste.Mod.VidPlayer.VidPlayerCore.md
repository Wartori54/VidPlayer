# <a id="Celeste_Mod_VidPlayer_VidPlayerCore"></a> Class VidPlayerCore

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
public abstract class VidPlayerCore
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[VidPlayerCore](Celeste.Mod.VidPlayer.VidPlayerCore.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore__ctor_Microsoft_Xna_Framework_Vector2_System_String_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Single_"></a> VidPlayerCore\(Vector2, string, bool, bool, bool, bool, float\)

```csharp
public VidPlayerCore(Vector2 entitySize, string videoTarget, bool entityIsMuted, bool entityKeepAspectRatio, bool entityLooping, bool entityHires, float entityVolumeMult)
```

#### Parameters

`entitySize` Vector2

`videoTarget` [string](https://learn.microsoft.com/dotnet/api/system.string)

`entityIsMuted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityKeepAspectRatio` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityLooping` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityHires` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityVolumeMult` [float](https://learn.microsoft.com/dotnet/api/system.single)

## Properties

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Done"></a> Done

```csharp
public bool Done { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_LoadingState"></a> LoadingState

```csharp
public static bool LoadingState { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Muted"></a> Muted

```csharp
public bool Muted { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Paused"></a> Paused

```csharp
protected abstract bool Paused { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Position"></a> Position

```csharp
protected abstract Vector2 Position { get; }
```

#### Property Value

 Vector2

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Init"></a> Init\(\)

```csharp
public void Init()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_LoadState_Celeste_Level_"></a> LoadState\(Level\)

```csharp
protected virtual void LoadState(Level newLevel)
```

#### Parameters

`newLevel` Level

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Mark"></a> Mark\(\)

```csharp
public void Mark()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_RegisterSRTInterop"></a> RegisterSRTInterop\(\)

```csharp
public static void RegisterSRTInterop()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Render"></a> Render\(\)

```csharp
public void Render()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Reset"></a> Reset\(\)

```csharp
public void Reset()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_SaveState_Celeste_Level_"></a> SaveState\(Level\)

```csharp
protected virtual void SaveState(Level newLevel)
```

#### Parameters

`newLevel` Level

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_UnregisterSRTInterop"></a> UnregisterSRTInterop\(\)

```csharp
public static void UnregisterSRTInterop()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_Update"></a> Update\(\)

```csharp
public void Update()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerCore_checkDisposed"></a> checkDisposed\(\)

```csharp
public bool checkDisposed()
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

