# <a id="Celeste_Mod_VidPlayer_VidPlayerEntity"></a> Class VidPlayerEntity

Namespace: [Celeste.Mod.VidPlayer](Celeste.Mod.VidPlayer.md)  
Assembly: VidPlayer.dll  

```csharp
[Tracked(false)]
[CustomEntity(new string[] { "VidPlayer/VidPlayerEntity" })]
public sealed class VidPlayerEntity : Entity, IEnumerable<Component>, IEnumerable
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
Entity ← 
[VidPlayerEntity](Celeste.Mod.VidPlayer.VidPlayerEntity.md)

#### Implements

[IEnumerable<Component\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1), 
[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.ienumerable)

#### Inherited Members

Entity.Active, 
Entity.Visible, 
Entity.Collidable, 
Entity.Position, 
Entity.tag, 
Entity.collider, 
Entity.depth, 
Entity.actualDepth, 
Entity.SceneBegin\(Scene\), 
Entity.SceneEnd\(Scene\), 
Entity.Awake\(Scene\), 
Entity.Added\(Scene\), 
Entity.Removed\(Scene\), 
Entity.Update\(\), 
Entity.Render\(\), 
Entity.DebugRender\(Camera\), 
Entity.HandleGraphicsReset\(\), 
Entity.HandleGraphicsCreate\(\), 
Entity.RemoveSelf\(\), 
Entity.TagFullCheck\(int\), 
Entity.TagCheck\(int\), 
Entity.AddTag\(int\), 
Entity.RemoveTag\(int\), 
Entity.CollideCheck\(Entity\), 
Entity.CollideCheck\(Entity, Vector2\), 
Entity.CollideCheck\(BitTag\), 
Entity.CollideCheck\(BitTag, Vector2\), 
Entity.CollideCheck<T\>\(\), 
Entity.CollideCheck<T\>\(Vector2\), 
Entity.CollideCheck<T, Exclude\>\(\), 
Entity.CollideCheck<T, Exclude\>\(Vector2\), 
Entity.CollideCheck<T, Exclude1, Exclude2\>\(\), 
Entity.CollideCheck<T, Exclude1, Exclude2\>\(Vector2\), 
Entity.CollideCheckByComponent<T\>\(\), 
Entity.CollideCheckByComponent<T\>\(Vector2\), 
Entity.CollideCheckOutside\(Entity, Vector2\), 
Entity.CollideCheckOutside\(BitTag, Vector2\), 
Entity.CollideCheckOutside<T\>\(Vector2\), 
Entity.CollideCheckOutsideByComponent<T\>\(Vector2\), 
Entity.CollideFirst\(BitTag\), 
Entity.CollideFirst\(BitTag, Vector2\), 
Entity.CollideFirst<T\>\(\), 
Entity.CollideFirst<T\>\(Vector2\), 
Entity.CollideFirstByComponent<T\>\(\), 
Entity.CollideFirstByComponent<T\>\(Vector2\), 
Entity.CollideFirstOutside\(BitTag, Vector2\), 
Entity.CollideFirstOutside<T\>\(Vector2\), 
Entity.CollideFirstOutsideByComponent<T\>\(Vector2\), 
Entity.CollideAll\(BitTag\), 
Entity.CollideAll\(BitTag, Vector2\), 
Entity.CollideAll<T\>\(\), 
Entity.CollideAll<T\>\(Vector2\), 
Entity.CollideAll<T\>\(Vector2, List<Entity\>\), 
Entity.CollideAllByComponent<T\>\(\), 
Entity.CollideAllByComponent<T\>\(Vector2\), 
Entity.CollideDo\(BitTag, Action<Entity\>\), 
Entity.CollideDo\(BitTag, Action<Entity\>, Vector2\), 
Entity.CollideDo<T\>\(Action<T\>\), 
Entity.CollideDo<T\>\(Action<T\>, Vector2\), 
Entity.CollideDoByComponent<T\>\(Action<T\>\), 
Entity.CollideDoByComponent<T\>\(Action<T\>, Vector2\), 
Entity.CollidePoint\(Vector2\), 
Entity.CollidePoint\(Vector2, Vector2\), 
Entity.CollideLine\(Vector2, Vector2\), 
Entity.CollideLine\(Vector2, Vector2, Vector2\), 
Entity.CollideRect\(Rectangle\), 
Entity.CollideRect\(Rectangle, Vector2\), 
Entity.Add\(Component\), 
Entity.Remove\(Component\), 
Entity.Add\(params Component\[\]\), 
Entity.Remove\(params Component\[\]\), 
Entity.Get<T\>\(\), 
Entity.GetEnumerator\(\), 
Entity.IEnumerable.GetEnumerator\(\), 
Entity.Closest\(params Entity\[\]\), 
Entity.Closest\(BitTag\), 
Entity.SceneAs<T\>\(\), 
Entity.DissociateFromScene\(\), 
Entity.\_PreUpdate\(\), 
Entity.\_PostUpdate\(\), 
Entity.Scene, 
Entity.Components, 
Entity.Depth, 
Entity.X, 
Entity.Y, 
Entity.Collider, 
Entity.Width, 
Entity.Height, 
Entity.Left, 
Entity.Right, 
Entity.Top, 
Entity.Bottom, 
Entity.CenterX, 
Entity.CenterY, 
Entity.TopLeft, 
Entity.TopRight, 
Entity.BottomLeft, 
Entity.BottomRight, 
Entity.Center, 
Entity.CenterLeft, 
Entity.CenterRight, 
Entity.TopCenter, 
Entity.BottomCenter, 
Entity.Tag, 
Entity.PreUpdate, 
Entity.PostUpdate, 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity__ctor_Celeste_EntityData_Microsoft_Xna_Framework_Vector2_"></a> VidPlayerEntity\(EntityData, Vector2\)

```csharp
public VidPlayerEntity(EntityData data, Vector2 offset)
```

#### Parameters

`data` EntityData

`offset` Vector2

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity__ctor_Microsoft_Xna_Framework_Vector2_Microsoft_Xna_Framework_Vector2_System_String_System_Boolean_System_Boolean_System_Boolean_System_Boolean_System_Single_Microsoft_Xna_Framework_Vector2_"></a> VidPlayerEntity\(Vector2, Vector2, string, bool, bool, bool, bool, float, Vector2\)

```csharp
public VidPlayerEntity(Vector2 position, Vector2 entitySize, string videoTarget, bool entityIsMuted, bool entityKeepAspectRatio, bool entityLooping, bool entityHires, float entityVolumeMult, Vector2 offset)
```

#### Parameters

`position` Vector2

`entitySize` Vector2

`videoTarget` [string](https://learn.microsoft.com/dotnet/api/system.string)

`entityIsMuted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityKeepAspectRatio` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityLooping` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityHires` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`entityVolumeMult` [float](https://learn.microsoft.com/dotnet/api/system.single)

`offset` Vector2

## Fields

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_ForcePause"></a> ForcePause

```csharp
public bool ForcePause
```

#### Field Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Core"></a> Core

```csharp
public VidPlayerCore Core { get; }
```

#### Property Value

 [VidPlayerCore](Celeste.Mod.VidPlayer.VidPlayerCore.md)

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Done"></a> Done

```csharp
public bool Done { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Muted"></a> Muted

```csharp
public bool Muted { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Methods

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Awake_Monocle_Scene_"></a> Awake\(Scene\)

Called before the frame starts, after Entities are added and removed, on the frame that the Entity was added.<br />
Useful if you added two Entities in the same frame, and need them to detect each other before they start Updating.

```csharp
public override void Awake(Scene scene)
```

#### Parameters

`scene` Scene

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Removed_Monocle_Scene_"></a> Removed\(Scene\)

Called when the Entity is removed from a Scene.

```csharp
public override void Removed(Scene scene)
```

#### Parameters

`scene` Scene

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Render"></a> Render\(\)

Draw the Entity here. Not called if the Entity is not Visible.

```csharp
public override void Render()
```

### <a id="Celeste_Mod_VidPlayer_VidPlayerEntity_Update"></a> Update\(\)

Do game logic here, but do not render here. Not called if the Entity is not Active.

```csharp
public override void Update()
```

