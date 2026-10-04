using Godot;
public class ViewModelHUDWeapons
{
	public Node HUDammo;
	public Node TextRightWeaponAmmoMagazineNumber;
	public Node TextRightWeaponAmmoReserveNumber;
	public Node RightWeaponAmmoBox;
	public Node TextLeftWeaponAmmoMagazineNumber;
	public Node TextLeftWeaponAmmoReserveNumber;
	public Node LeftWeaponAmmoBox;

	public Node HUDcrosshiars;

	public Node CrosshairRevolver;

	public Node CrosshairAutoPistol;
	public Node[] ListCrosshairPartsAutoPistol = new Node[4];

	public Node CrosshairShotgun;
	public Node[] ListCrosshairPartsShotgun = new Node[4];

	public Node CrosshairTranquilizer;

	public Node CrosshairCrossbow;
	public Node[] ListCrosshairTypesCrossbow = new Node[4];

	public ViewModelHUDWeapons(Bootstrap bootstrap, Node canvas)
	{
		HUDammo = bootstrap.FindDeepNode(canvas, "HUDammo");
		TextRightWeaponAmmoMagazineNumber = bootstrap.FindDeepNode(canvas, "TextRightWeaponAmmoMagazineNumber");
		TextRightWeaponAmmoReserveNumber = bootstrap.FindDeepNode(canvas, "TextRightWeaponAmmoReserveNumber");
		RightWeaponAmmoBox = bootstrap.FindDeepNode(canvas, "RightWeaponAmmoBox");
		TextLeftWeaponAmmoMagazineNumber = bootstrap.FindDeepNode(canvas, "TextLeftWeaponAmmoMagazineNumber");
		TextLeftWeaponAmmoReserveNumber = bootstrap.FindDeepNode(canvas, "TextLeftWeaponAmmoReserveNumber");
		LeftWeaponAmmoBox = bootstrap.FindDeepNode(canvas, "LeftWeaponAmmoBox");

		HUDcrosshiars = bootstrap.FindDeepNode(canvas, "HUDcrosshairs");

		CrosshairRevolver = bootstrap.FindDeepNode(HUDcrosshiars, "CrosshairRevolver");

		CrosshairAutoPistol = bootstrap.FindDeepNode(HUDcrosshiars, "CrosshairAutoPistol");
		for (int i = 0; i < ListCrosshairPartsAutoPistol.Length; i++)
		{
			ListCrosshairPartsAutoPistol[i] = bootstrap.FindDeepNode(CrosshairAutoPistol, $"CrosshairAutoPistolPart{i + 1}");
		}

		CrosshairShotgun = bootstrap.FindDeepNode(HUDcrosshiars, "CrosshairShotgun");
		for (int i = 0; i < ListCrosshairPartsShotgun.Length; i++)
		{
			ListCrosshairPartsShotgun[i] = bootstrap.FindDeepNode(CrosshairShotgun, $"CrosshairShotgunPart{i + 1}");
		}

		CrosshairTranquilizer = bootstrap.FindDeepNode(HUDcrosshiars, "CrosshairTranquilizer");

		CrosshairCrossbow = bootstrap.FindDeepNode(HUDcrosshiars, "CrosshairCrossbow");
		ListCrosshairTypesCrossbow[0] = bootstrap.FindDeepNode(CrosshairCrossbow, "CrosshairCrossbowDefault");
		ListCrosshairTypesCrossbow[1] = bootstrap.FindDeepNode(CrosshairCrossbow, "CrosshairCrossbowPlungingAvailable");
		ListCrosshairTypesCrossbow[2] = bootstrap.FindDeepNode(CrosshairCrossbow, "CrosshairCrossbowHookingObjectAvailable");
		ListCrosshairTypesCrossbow[3] = bootstrap.FindDeepNode(CrosshairCrossbow, "CrosshairCrossbowFail");
	}
}