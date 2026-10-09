using Godot;

public partial class PlayerWeaponFirstPersonRenderer : Node
{
	private GameScenesManager _gameScenes;
	private PlayerCameraStateMachineController _cameraStates;
	private PlayerInteractionController _interaction;
	private PlayerWeaponController _weapons;
	private PlayerWeaponAnimationController _weaponAnimation;
	private Node _firstPersonRightHand;
	private Node _firstPersonLeftHand;
	private Node _thirdPersonRightHand;
	private Node _thirdPersonLeftHand;
	private WeaponAbstract _rightWeapon;
	private WeaponAbstract _leftWeapon;

	public void Initialize(
		GameScenesManager gameSceneManager,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerInteractionController interactionController,
		PlayerWeaponController weaponController,
		PlayerWeaponAnimationController weaponAnimationController,
		Node playerFirstPersonHandRight,
		Node playerFirstPersonHandLeft,
		Node playerThirdPersonHandRight,
		Node playerThirdPersonHandLeft)
	{
		_gameScenes = gameSceneManager;
		_cameraStates = playerCameraStateMachineController;
		_interaction = interactionController;
		_weapons = weaponController;
		_weaponAnimation = weaponAnimationController;
		_firstPersonRightHand = playerFirstPersonHandRight;
		_firstPersonLeftHand = playerFirstPersonHandLeft;
		_thirdPersonRightHand = playerThirdPersonHandRight;
		_thirdPersonLeftHand = playerThirdPersonHandLeft;

		if (_cameraStates != null)
		{
			_cameraStates.OnFirstPersonCameraState += RefreshVisibility;
			_cameraStates.OnThirdPersonCameraState += RefreshVisibility;
		}
		if (_weapons != null)
		{
			_weapons.OnWeaponChanged += RefreshVisibility;
			_weapons.OnShowWeapon += HandleWeaponVisibilityChanged;
			_weapons.OnHideWeapon += HandleWeaponVisibilityChanged;
			_rightWeapon = _weapons.RightHandWeaponComponent;
			_leftWeapon = _weapons.LeftHandWeaponComponent;
		}
		if (_weaponAnimation != null)
		{
			_weaponAnimation.OnShowWeapon += HandleWeaponVisibilityChanged;
			_weaponAnimation.OnHideWeapon += HandleWeaponVisibilityChanged;
			_weaponAnimation.OnPlayerStartedReloading += ShowReloadingHands;
			_weaponAnimation.OnPlayerEndedReloading += RefreshVisibility;
		}
		if (_gameScenes != null)
		{
			_gameScenes.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideFirstPersonHands;
			_gameScenes.OnBeginLoadingGameplayScene += RefreshVisibility;
		}
		RefreshVisibility();
	}

	private void RefreshVisibility() => RefreshVisibility(default);

	private void RefreshVisibility(WeaponHandType activeHand)
	{
		bool firstPerson = _cameraStates?.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson;
		SetVisible(_firstPersonRightHand, firstPerson);
		SetVisible(_firstPersonLeftHand, firstPerson);
		SetVisible(_thirdPersonRightHand, !firstPerson && _rightWeapon?.Visible == true);
		SetVisible(_thirdPersonLeftHand, !firstPerson && _leftWeapon?.Visible == true);
	}

	private void HandleWeaponVisibilityChanged(WeaponAbstract weapon)
	{
		if (weapon == null) return;
		if (weapon == _weapons?.RightHandWeaponComponent) _rightWeapon = weapon;
		if (weapon == _weapons?.LeftHandWeaponComponent) _leftWeapon = weapon;
		RefreshVisibility();
	}

	private void ShowReloadingHands()
	{
		if (_cameraStates?.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			SetVisible(_firstPersonRightHand, true);
			SetVisible(_firstPersonLeftHand, true);
		}
	}

	private void HideFirstPersonHands()
	{
		SetVisible(_firstPersonRightHand, false);
		SetVisible(_firstPersonLeftHand, false);
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (node is CanvasItem canvasItem) canvasItem.Visible = visible;
		else if (node is Node3D node3D) node3D.Visible = visible;
	}

	public override void _ExitTree()
	{
		if (_cameraStates != null)
		{
			_cameraStates.OnFirstPersonCameraState -= RefreshVisibility;
			_cameraStates.OnThirdPersonCameraState -= RefreshVisibility;
		}
		if (_weapons != null)
		{
			_weapons.OnWeaponChanged -= RefreshVisibility;
			_weapons.OnShowWeapon -= HandleWeaponVisibilityChanged;
			_weapons.OnHideWeapon -= HandleWeaponVisibilityChanged;
		}
		if (_weaponAnimation != null)
		{
			_weaponAnimation.OnShowWeapon -= HandleWeaponVisibilityChanged;
			_weaponAnimation.OnHideWeapon -= HandleWeaponVisibilityChanged;
			_weaponAnimation.OnPlayerStartedReloading -= ShowReloadingHands;
			_weaponAnimation.OnPlayerEndedReloading -= RefreshVisibility;
		}
		if (_gameScenes != null)
		{
			_gameScenes.OnBeginLoadingMainMenuOrEndGameTitlesScene -= HideFirstPersonHands;
			_gameScenes.OnBeginLoadingGameplayScene -= RefreshVisibility;
		}
	}
}