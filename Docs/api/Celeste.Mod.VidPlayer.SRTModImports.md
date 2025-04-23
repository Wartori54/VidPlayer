# <a id="Celeste_Mod_VidPlayer_SRTModImports"></a> Class SRTModImports

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
[ModImportName("SpeedrunTool.SaveLoad")]
public class SRTModImports
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SRTModImports](Celeste.Mod.VidPlayer.SRTModImports.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Fields

### <a id="Celeste_Mod_VidPlayer_SRTModImports_AddReturnSameObjectProcessor"></a> AddReturnSameObjectProcessor

```csharp
public static Action<Func<Type, bool>>? AddReturnSameObjectProcessor
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[Type](https://learn.microsoft.com/dotnet/api/system.type), [bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>\>?

### <a id="Celeste_Mod_VidPlayer_SRTModImports_IgnoreSaveState"></a> IgnoreSaveState

```csharp
public static Action<Entity, bool>? IgnoreSaveState
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action\-2)<Entity, [bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>?

### <a id="Celeste_Mod_VidPlayer_SRTModImports_RegisterSaveLoadAction"></a> RegisterSaveLoadAction

```csharp
public static Func<Action<Dictionary<Type, Dictionary<string, object>>, Level>, Action<Dictionary<Type, Dictionary<string, object>>, Level>, Action?, Action<Level>?, Action<Level>?, Action?, object>? RegisterSaveLoadAction
```

#### Field Value

 [Func](https://learn.microsoft.com/dotnet/api/system.func\-7)<[Action](https://learn.microsoft.com/dotnet/api/system.action\-2)<[Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[Type](https://learn.microsoft.com/dotnet/api/system.type), [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [object](https://learn.microsoft.com/dotnet/api/system.object)\>\>, Level\>, [Action](https://learn.microsoft.com/dotnet/api/system.action\-2)<[Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[Type](https://learn.microsoft.com/dotnet/api/system.type), [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [object](https://learn.microsoft.com/dotnet/api/system.object)\>\>, Level\>, [Action](https://learn.microsoft.com/dotnet/api/system.action)?, [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<Level\>?, [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<Level\>?, [Action](https://learn.microsoft.com/dotnet/api/system.action)?, [object](https://learn.microsoft.com/dotnet/api/system.object)\>?

### <a id="Celeste_Mod_VidPlayer_SRTModImports_RegisterStaticTypes"></a> RegisterStaticTypes

```csharp
public static Func<Type, string[], object>? RegisterStaticTypes
```

#### Field Value

 [Func](https://learn.microsoft.com/dotnet/api/system.func\-3)<[Type](https://learn.microsoft.com/dotnet/api/system.type), [string](https://learn.microsoft.com/dotnet/api/system.string)\[\], [object](https://learn.microsoft.com/dotnet/api/system.object)\>?

### <a id="Celeste_Mod_VidPlayer_SRTModImports_RemoveReturnSameObjectProcessor"></a> RemoveReturnSameObjectProcessor

```csharp
public static Action<Func<Type, bool>>? RemoveReturnSameObjectProcessor
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[Type](https://learn.microsoft.com/dotnet/api/system.type), [bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>\>?

### <a id="Celeste_Mod_VidPlayer_SRTModImports_Unregister"></a> Unregister

```csharp
public static Action<object>? Unregister
```

#### Field Value

 [Action](https://learn.microsoft.com/dotnet/api/system.action\-1)<[object](https://learn.microsoft.com/dotnet/api/system.object)\>?

