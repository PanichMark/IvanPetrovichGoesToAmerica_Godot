using Godot;

[GlobalClass]
public partial class PlayerWeaponGive : Resource
{
	[Export] public PackedScene WeaponScene { get; set; }
}