using Godot;

[GlobalClass]
public partial class ObjectPoolWeaponList : Resource
{
	[Export] public ObjectPoolSprite BulletHoleSolid { get; set; }
	[Export] public ObjectPoolSprite BulletHoleBlood { get; set; }
}