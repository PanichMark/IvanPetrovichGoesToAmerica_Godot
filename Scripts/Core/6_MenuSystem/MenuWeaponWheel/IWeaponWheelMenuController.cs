using Godot;
public interface IWeaponWheelMenuController
{
	void CreateWheel();

	void RecreateWheel();

	void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed);

	void OnWeaponUnlocked(Node weaponPrefab);

	void ShowWeaponName();

	void RestrictWeaponWheelWhilePickable();
	void UnrestrictWeaponWheelWhilePickable();


	void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerInteractionController playerInteractionController,
		PlayerWeaponAmmoController playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		Node weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel,
		Node PlayerCamera);
}
