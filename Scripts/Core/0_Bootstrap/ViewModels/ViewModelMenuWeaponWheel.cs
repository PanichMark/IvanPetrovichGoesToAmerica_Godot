using Godot;
public class ViewModelMenuWeaponWheel
{
	public Node GameObjectWeaponWheelSegment;

	public Node TextWeaponWheelWeaponName;
	public Node ImageWeaponWheelWeaponIcon;
	public Node TextWeaponAmmoMagazineNumber;
	public Node TextWeaponAmmoReserveNumber;
	public Node TextWeaponAmmoSeparator;

	public Node WeaponWheelRadius;

	public Node TextWeaponWheelHandType;

	public Node ButtonUseHealingItem;
	public Node TextHealingItemNumber;

	public Node ButtonUseManaReplenishItem;
	public Node TextManaReplenishItemNumber;

	public Node TextWeaponWheelUnavailable;

	public Node WeaponWheelData;

	public ViewModelMenuWeaponWheel(Bootstrap bootstrap, Node canvas)
	{
		GameObjectWeaponWheelSegment = GD.Load<PackedScene>("res://WeaponSystem/WeaponWheelSegment.tscn").Instantiate();
		
		TextWeaponWheelWeaponName = bootstrap.FindDeepNode(canvas, "TextWeaponWheelWeapon");
		ImageWeaponWheelWeaponIcon = bootstrap.FindDeepNode(canvas, "ImageWeaponWheelWeapon");
		TextWeaponAmmoMagazineNumber = bootstrap.FindDeepNode(canvas, "WeaponAmmoMagazineNumber");
		TextWeaponAmmoReserveNumber = bootstrap.FindDeepNode(canvas, "WeaponAmmoReserveNumber");
		TextWeaponAmmoSeparator = bootstrap.FindDeepNode(canvas, "WeaponAmmoSeparator");

		WeaponWheelRadius = bootstrap.FindDeepNode(canvas, "Radius");
		
		TextWeaponWheelHandType = bootstrap.FindDeepNode(canvas, "TextWeaponWheelHandType");

		ButtonUseHealingItem = bootstrap.FindDeepNode(canvas, "ButtonUseHealingItem");
		TextHealingItemNumber = bootstrap.FindDeepNode(canvas, "TextHealingItemNumber");

		ButtonUseManaReplenishItem = bootstrap.FindDeepNode(canvas, "ButtonUseManaReplenishItem");
		TextManaReplenishItemNumber = bootstrap.FindDeepNode(canvas, "TextManaReplenishItemNumber");

		TextWeaponWheelUnavailable = bootstrap.FindDeepNode(canvas, "TextWeaponWheelUnavailable");

		WeaponWheelData = bootstrap.FindDeepNode(canvas, "WeaponWheelData");
	}
}