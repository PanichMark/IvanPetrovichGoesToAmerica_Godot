using Godot;
[GlobalClass]
public partial class ConfigPlayerTransform : Resource
{
	[Export] public Godot.Vector3 PlayerPosition { get; set; } = Godot.Vector3.Zero;
	[Export] public int PlayerRotationY { get; set; }
}