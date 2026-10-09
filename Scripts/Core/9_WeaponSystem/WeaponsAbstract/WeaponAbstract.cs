using Godot;

public abstract partial class WeaponAbstract : Node3D
{
	[Export] public PlayerWeaponNames WeaponName { get; set; }
	[Export] public Texture2D WeaponIconSmall { get; set; }
	[Export] public Texture2D WeaponIcon { get; set; }
	[Export] public bool IsWeaponAuto { get; set; }
	[Export] public float WeaponDamage { get; set; }
	[Export] public int WeaponIndex { get; set; }

	public bool IsWeaponPlayerAutoAttacking { get; protected set; }
	public bool IsPlayerWeaponAttacking { get; protected set; }
	public bool IsWeaponVisible => Visible;

	public virtual void InitializeWeaponPlayer() { }
	public virtual void InitializeWeaponNpc(Node3D weaponShootPoint) { }
	public virtual void WeaponPlayerAttack() { }
	public virtual void StartAutoAttackingWeaponPlayer() { }
	public virtual void StopAutoAttackingWeaponPlayer() { }
	public virtual void ShowWeapon() => Visible = true;
	public virtual void HideWeapon() => Visible = false;
}