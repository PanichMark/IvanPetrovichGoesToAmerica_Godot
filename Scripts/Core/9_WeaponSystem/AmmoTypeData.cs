public struct AmmoTypeData
{
	public AmmoTypes AmmoType { get; set; }
	public int AmmoMax { get; set; }
	public int AmmoReserve { get; set; }
}

public struct WeaponRangedData
{
	public PlayerWeaponNames RagnedWeapon { get; set; }
	public AmmoTypes AmmoType { get; set; }
	public int MagazineAmmoMax { get; set; }
	public int MagazineAmmoCurrent { get; set; }
}