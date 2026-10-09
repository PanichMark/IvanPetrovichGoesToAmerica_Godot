using Godot;

public abstract partial class WeaponRangedAbstract : WeaponAbstract
{
	[Export] public float WeaponRange { get; set; } = 100f;
	[Export] public AmmoTypes PlayerWeaponAmmoType { get; set; }
	[Export] public bool LeavesBulletHole { get; set; } = true;
	[Export] public bool IsReloadingAnimationSingle { get; set; }
	[Export] public int PlayerMagazineAmmoMax { get; set; }

	public Node3D WeaponRangedShootPoint { get; protected set; }
	public int PlayerMagazineAmmoCurrent { get; set; }
	public int PlayerAmmoReserve => ServiceLocator.Resolve<PlayerWeaponAmmoController>()?.GetAmmoReserve(PlayerWeaponAmmoType) ?? 0;
	public int PlayerAmmoMax => ServiceLocator.Resolve<PlayerWeaponAmmoController>()?.GetAmmoMaximum(PlayerWeaponAmmoType) ?? 0;

	public override void InitializeWeaponPlayer()
	{
		WeaponRangedShootPoint = GetViewport()?.GetCamera3D();
		if (PlayerMagazineAmmoMax > 0 && PlayerMagazineAmmoCurrent <= 0)
			PlayerMagazineAmmoCurrent = PlayerMagazineAmmoMax;
		InitializeWeaponRanged();
	}

	protected virtual void InitializeWeaponRanged() { }

	public override void WeaponPlayerAttack()
	{
		if (IsReloadingAnimationSingle || PlayerMagazineAmmoCurrent <= 0)
			return;
		IsPlayerWeaponAttacking = true;
		if (IsWeaponAuto)
			StartAutoAttackingWeaponPlayer();
		else
			Shoot();
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (IsWeaponPlayerAutoAttacking || PlayerMagazineAmmoCurrent <= 0)
			return;
		IsWeaponPlayerAutoAttacking = true;
		Shoot();
	}

	public override void StopAutoAttackingWeaponPlayer()
	{
		IsWeaponPlayerAutoAttacking = false;
		IsPlayerWeaponAttacking = false;
	}

	public virtual void Reload()
	{
		PlayerWeaponAmmoController ammo = ServiceLocator.Resolve<PlayerWeaponAmmoController>();
		if (ammo == null || PlayerMagazineAmmoCurrent >= PlayerMagazineAmmoMax)
			return;
		int loaded = ammo.TakeAmmo(PlayerWeaponAmmoType, PlayerMagazineAmmoMax - PlayerMagazineAmmoCurrent);
		if (loaded <= 0)
			return;
		PlayerMagazineAmmoCurrent += loaded;
		ammo.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
	}

	protected virtual void Shoot()
	{
		if (PlayerMagazineAmmoCurrent <= 0)
		{
			StopAutoAttackingWeaponPlayer();
			return;
		}
		PlayerMagazineAmmoCurrent--;
		ServiceLocator.Resolve<PlayerWeaponAmmoController>()?.NotifyMagazineAmmoChanged(WeaponName, PlayerWeaponAmmoType, PlayerMagazineAmmoCurrent);
		OnShoot();
		if (!IsWeaponAuto)
			IsPlayerWeaponAttacking = false;
	}

	protected virtual void OnShoot() { }
}