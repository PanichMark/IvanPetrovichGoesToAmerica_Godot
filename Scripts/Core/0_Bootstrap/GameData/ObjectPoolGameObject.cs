using Godot;

[GlobalClass]
public partial class ObjectPoolGameObject : ObjectPoolAbstract
{
	[Export] public PackedScene ObjectPoolScene { get; set; }
}