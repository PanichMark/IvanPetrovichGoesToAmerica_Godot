using Godot;

[GlobalClass]
public partial class InteractionObjectPickableData : Resource
{
	[Export] public InteractionObjectsPickableTypes PickableType { get; set; }
	[Export] public Vector3 Position { get; set; }
	[Export] public Quaternion Rotation { get; set; } = Quaternion.Identity;
	public Vector3 RotationDegrees => Rotation.GetEuler() * Mathf.RadToDeg(1f);
}