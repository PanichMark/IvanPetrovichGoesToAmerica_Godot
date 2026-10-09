using Godot;
using System.Collections.Generic;

public partial class PlayerWeaponAmmoController : Node
{
	public delegate void ReserveAmmoChangedHandler(AmmoTypes ammoType, int newCount);
	public delegate void MagazineAmmoChangedHandler(PlayerWeaponNames weapon, AmmoTypes ammoType, int newCount);

	public event ReserveAmmoChangedHandler OnReserveAmmoChanged;
	public event MagazineAmmoChangedHandler OnMagazineAmmoChanged;

	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary { get; } = new();
	public Dictionary<PlayerWeaponNames, WeaponRangedData> WeaponsRangedDictionary { get; } = new();

	public void Initialize()
	{
		AmmoDictionary.Clear();
		WeaponsRangedDictionary.Clear();
		SetAmmo(AmmoTypes.Ammo9mm, 999, 100);
		SetAmmo(AmmoTypes.Ammo12gauge, 999, 30);
		SetAmmo(AmmoTypes.AmmoTranquilizerDart, 999, 10);
		SetWeapon(PlayerWeaponNames.Revolver, AmmoTypes.Ammo9mm, 5);
		SetWeapon(PlayerWeaponNames.AutoPistol, AmmoTypes.Ammo9mm, 30);
		SetWeapon(PlayerWeaponNames.Shotgun, AmmoTypes.Ammo12gauge, 2);
		SetWeapon(PlayerWeaponNames.Tranquilizer, AmmoTypes.AmmoTranquilizerDart, 1);
	}

	public void ConfigApplyPlayerAmmo(AmmoTypes type, int newAmount)
	{
		if (!AmmoDictionary.TryGetValue(type, out AmmoTypeData data)) return;
		data.AmmoReserve = Mathf.Clamp(newAmount, 0, data.AmmoMax);
		AmmoDictionary[type] = data;
		OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
	}

	public int GetAmmoReserve(AmmoTypes type) => AmmoDictionary.TryGetValue(type, out AmmoTypeData data) ? data.AmmoReserve : 0;
	public int GetAmmoMaximum(AmmoTypes type) => AmmoDictionary.TryGetValue(type, out AmmoTypeData data) ? data.AmmoMax : 0;

	public int TakeAmmo(AmmoTypes type, int requested)
	{
		if (requested <= 0 || !AmmoDictionary.TryGetValue(type, out AmmoTypeData data)) return 0;
		int taken = Mathf.Min(requested, data.AmmoReserve);
		data.AmmoReserve -= taken;
		AmmoDictionary[type] = data;
		if (taken > 0) OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
		return taken;
	}

	public void AddAmmoToReserve(AmmoTypes type, int amount)
	{
		if (amount <= 0 || !AmmoDictionary.TryGetValue(type, out AmmoTypeData data)) return;
		data.AmmoReserve = Mathf.Min(data.AmmoMax, data.AmmoReserve + amount);
		AmmoDictionary[type] = data;
		OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
	}

	public void RemoveAmmoFromReserve(AmmoTypes type, int amount)
	{
		if (amount <= 0 || !AmmoDictionary.TryGetValue(type, out AmmoTypeData data)) return;
		data.AmmoReserve = Mathf.Max(0, data.AmmoReserve - amount);
		AmmoDictionary[type] = data;
		OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
	}

	public void AddAmmoToMagazine(AmmoTypes type, int amount) => ChangeMagazines(type, amount);
	public void RemoveAmmoFromMagazine(AmmoTypes type, int amount) => ChangeMagazines(type, -amount);
	public void NotifyReserveAmmoChanged(AmmoTypes type, int newAmount) => OnReserveAmmoChanged?.Invoke(type, newAmount);
	public void NotifyMagazineAmmoChanged(PlayerWeaponNames weaponType, AmmoTypes ammoType, int newAmount)
	{
		if (WeaponsRangedDictionary.TryGetValue(weaponType, out WeaponRangedData data))
		{
			data.MagazineAmmoCurrent = Mathf.Clamp(newAmount, 0, data.MagazineAmmoMax);
			WeaponsRangedDictionary[weaponType] = data;
		}
		OnMagazineAmmoChanged?.Invoke(weaponType, ammoType, newAmount);
	}

	private void ChangeMagazines(AmmoTypes type, int amount)
	{
		if (amount == 0) return;
		foreach (PlayerWeaponNames weaponName in new List<PlayerWeaponNames>(WeaponsRangedDictionary.Keys))
		{
			WeaponRangedData data = WeaponsRangedDictionary[weaponName];
			if (data.AmmoType != type) continue;
			data.MagazineAmmoCurrent = Mathf.Clamp(data.MagazineAmmoCurrent + amount, 0, data.MagazineAmmoMax);
			WeaponsRangedDictionary[weaponName] = data;
			OnMagazineAmmoChanged?.Invoke(weaponName, type, data.MagazineAmmoCurrent);
		}
	}

	private void SetAmmo(AmmoTypes type, int max, int reserve) => AmmoDictionary[type] = new AmmoTypeData { AmmoType = type, AmmoMax = max, AmmoReserve = reserve };

	private void SetWeapon(PlayerWeaponNames weapon, AmmoTypes ammo, int magazine)
	{
		WeaponsRangedDictionary[weapon] = new WeaponRangedData { RagnedWeapon = weapon, AmmoType = ammo, MagazineAmmoMax = magazine, MagazineAmmoCurrent = magazine };
	}
}