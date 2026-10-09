using Godot;

[GlobalClass]
public partial class AmmoGive : Resource
{
	[Export] public AmmoTypes AmmoType { get; set; }
	[Export] public int StartAmount { get; set; }
}