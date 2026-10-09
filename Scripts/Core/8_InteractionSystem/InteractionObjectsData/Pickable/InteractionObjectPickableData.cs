using Godot;

[GlobalClass]
public partial class InteractionObjectPickableData : Resource
{
	[Export] public InteractionObjectsPickableTypes PickableType { get; set; }
	[Export] public Vector3 Position { get; set; }
	[Export] public Vector3 RotationDegrees { get; set; }
}