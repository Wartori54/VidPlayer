# <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground"></a> Class VidPlayerStyleground

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
[CustomBackdrop(new string[] { "VidPlayer/VidPlayerStyleground" })]
public sealed class VidPlayerStyleground : Backdrop
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
Backdrop ← 
[VidPlayerStyleground](Celeste.Mod.VidPlayer.VidPlayerStyleground.md)

#### Inherited Members

Backdrop.UseSpritebatch, 
Backdrop.Name, 
Backdrop.Tags, 
Backdrop.Position, 
Backdrop.Scroll, 
Backdrop.Speed, 
Backdrop.Color, 
Backdrop.LoopX, 
Backdrop.LoopY, 
Backdrop.FlipX, 
Backdrop.FlipY, 
Backdrop.FadeX, 
Backdrop.FadeY, 
Backdrop.FadeAlphaMultiplier, 
Backdrop.WindMultiplier, 
Backdrop.ExcludeFrom, 
Backdrop.OnlyIn, 
Backdrop.OnlyIfFlag, 
Backdrop.OnlyIfNotFlag, 
Backdrop.AlsoIfFlag, 
Backdrop.Dreaming, 
Backdrop.Visible, 
Backdrop.InstantIn, 
Backdrop.InstantOut, 
Backdrop.ForceVisible, 
Backdrop.Renderer, 
Backdrop.IsVisible\(Level\), 
Backdrop.Update\(Scene\), 
Backdrop.BeforeRender\(Scene\), 
Backdrop.Render\(Scene\), 
Backdrop.Ended\(Scene\), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground__ctor_Celeste_BinaryPacker_Element_"></a> VidPlayerStyleground\(Element\)

```csharp
public VidPlayerStyleground(BinaryPacker.Element data)
```

#### Parameters

`data` BinaryPacker.Element

## Properties

### <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground_Core"></a> Core

```csharp
public VidPlayerCore Core { get; }
```

#### Property Value

 [VidPlayerCore](Celeste.Mod.VidPlayer.VidPlayerCore.md)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground_Ended_Monocle_Scene_"></a> Ended\(Scene\)

```csharp
public override void Ended(Scene scene)
```

#### Parameters

`scene` Scene

### <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground_Render_Monocle_Scene_"></a> Render\(Scene\)

```csharp
public override void Render(Scene scene)
```

#### Parameters

`scene` Scene

### <a id="Celeste_Mod_VidPlayer_VidPlayerStyleground_Update_Monocle_Scene_"></a> Update\(Scene\)

```csharp
public override void Update(Scene scene)
```

#### Parameters

`scene` Scene

