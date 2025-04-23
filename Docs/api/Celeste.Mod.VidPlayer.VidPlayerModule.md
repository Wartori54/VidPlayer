# <a id="Celeste_Mod_VidPlayer_VidPlayerModule"></a> Class VidPlayerModule

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
public class VidPlayerModule : EverestModule
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
EverestModule ← 
[VidPlayerModule](Celeste.Mod.VidPlayer.VidPlayerModule.md)

#### Inherited Members

EverestModule.ForceSaveDataAsync, 
EverestModule.ForceSaveDataFlush, 
EverestModule.\_PrevSettingsType, 
EverestModule.\_PrevSettingsProps, 
EverestModule.LoadSettings\(\), 
EverestModule.SaveSettings\(\), 
EverestModule.LoadSaveData\(int\), 
EverestModule.SaveSaveData\(int\), 
EverestModule.DeleteSaveData\(int\), 
EverestModule.ReadSaveData\(int\), 
EverestModule.WriteSaveData\(int, byte\[\]\), 
EverestModule.DeserializeSaveData\(int, byte\[\]\), 
EverestModule.SerializeSaveData\(int\), 
EverestModule.LoadSession\(int, bool\), 
EverestModule.SaveSession\(int\), 
EverestModule.DeleteSession\(int\), 
EverestModule.ReadSession\(int\), 
EverestModule.WriteSession\(int, byte\[\]\), 
EverestModule.DeserializeSession\(int, byte\[\]\), 
EverestModule.SerializeSession\(int\), 
EverestModule.Load\(\), 
EverestModule.Initialize\(\), 
EverestModule.LoadContent\(\), 
EverestModule.LoadContent\(bool\), 
EverestModule.Unload\(\), 
EverestModule.ParseArg\(string, Queue<string\>\), 
EverestModule.OnInputInitialize\(\), 
EverestModule.InitializeButtonBinding\(object, PropertyInfo\), 
EverestModule.OnInputDeregister\(\), 
EverestModule.CreateModMenuSectionHeader\(TextMenu, bool, EventInstance\), 
EverestModule.CreateModMenuSectionKeyBindings\(TextMenu, bool, EventInstance\), 
EverestModule.CreateKeyboardConfigUI\(TextMenu\), 
EverestModule.CreateButtonConfigUI\(TextMenu\), 
EverestModule.CreateModMenuSection\(TextMenu, bool, EventInstance\), 
EverestModule.PrepareMapDataProcessors\(MapDataFixup\), 
EverestModule.LogRegistration\(\), 
EverestModule.LogUnregistration\(\), 
EverestModule.Metadata, 
EverestModule.SettingsType, 
EverestModule.\_Settings, 
EverestModule.SaveDataType, 
EverestModule.\_SaveData, 
EverestModule.SaveDataAsync, 
EverestModule.SessionType, 
EverestModule.\_Session, 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule__ctor"></a> VidPlayerModule\(\)

```csharp
public VidPlayerModule()
```

## Properties

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_Instance"></a> Instance

```csharp
public static VidPlayerModule Instance { get; set; }
```

#### Property Value

 [VidPlayerModule](Celeste.Mod.VidPlayer.VidPlayerModule.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_SaveData"></a> SaveData

```csharp
public static VidPlayerModuleSaveData SaveData { get; }
```

#### Property Value

 [VidPlayerModuleSaveData](Celeste.Mod.VidPlayer.VidPlayerModuleSaveData.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_SaveDataType"></a> SaveDataType

The type used for the save data object. Used for serialization, among other things.

```csharp
public override Type SaveDataType { get; }
```

#### Property Value

 [Type](https://learn.microsoft.com/dotnet/api/system.type)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_Session"></a> Session

```csharp
public static VidPlayerModuleSession Session { get; }
```

#### Property Value

 [VidPlayerModuleSession](Celeste.Mod.VidPlayer.VidPlayerModuleSession.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_SessionType"></a> SessionType

The type used for the session object. Used for serialization, among other things.

```csharp
public override Type SessionType { get; }
```

#### Property Value

 [Type](https://learn.microsoft.com/dotnet/api/system.type)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_Settings"></a> Settings

```csharp
public static VidPlayerModuleSettings Settings { get; }
```

#### Property Value

 [VidPlayerModuleSettings](Celeste.Mod.VidPlayer.VidPlayerModuleSettings.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_SettingsType"></a> SettingsType

The type used for the settings object. Used for serialization, among other things.

```csharp
public override Type SettingsType { get; }
```

#### Property Value

 [Type](https://learn.microsoft.com/dotnet/api/system.type)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_Load"></a> Load\(\)

Perform any initializing actions after all mods have been loaded.
Do not depend on any specific order in which the mods get initialized.

```csharp
public override void Load()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerModule_Unload"></a> Unload\(\)

Unload any unmanaged resources allocated by the mod (f.e. textures) and
undo any changes performed by the mod.

```csharp
public override void Unload()
```

