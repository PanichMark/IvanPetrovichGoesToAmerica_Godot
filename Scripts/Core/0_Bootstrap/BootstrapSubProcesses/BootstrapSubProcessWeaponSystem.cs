using Godot;
public class BootstrapSubProcessWeaponSystem
{
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;

	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private Node _GameObjectBootstrapWeaponSystem;
	private Node _gameObjectPlayerCamera;
	public PlayerWeaponController WeaponController { get; private set; }
	private Node _gameObjectFirstPersonRightHandWeaponSlot;
	private Node _gameObjectFirstPersonLeftHandWeaponSlot;
	private Node _gameObjectThirdPersonRightHandWeaponSlot;
	private Node _gameObjectThirdPersonLeftHandWeaponSlot;

	public PlayerWeaponAmmoController PlayerResourcesAmmoManager { get; private set; }

	private IWeaponWheelMenuController _weaponWheelMenuController;

	private PlayerWeaponAnimationController _weaponAnimationController;

	private PlayerWeaponFirstPersonRenderer _weaponFirstPersonRender;

	private PlayerLegKickAttackController _legKickAttackController;
	private Node _gameObjectPlayer;

	private HUDweaponsController _HUDweaponController;

	public BootstrapSubProcessWeaponSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		Node playerGameObject,
		Node playerCamera,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		BootstrapSubProcessInteractionSystem bootstrapSubSystemInteraction)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameObjectPlayerCamera = playerCamera;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_bootstrapSubProcessInteractionSystem = bootstrapSubSystemInteraction;
		_gameObjectPlayer = playerGameObject;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_GameObjectBootstrapWeaponSystem = new Node("Bootstrap_WeaponSystem");
		_bootstrap.AddChild(_GameObjectBootstrapWeaponSystem);

		WeaponController = _GameObjectBootstrapWeaponSystem.CreateChildNode<PlayerWeaponController>();

		PlayerResourcesAmmoManager = _GameObjectBootstrapWeaponSystem.CreateChildNode<PlayerWeaponAmmoController>();
		//_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.CreateChildNode<WeaponWheelMenuController2D>();

		_weaponAnimationController = _GameObjectBootstrapWeaponSystem.CreateChildNode<PlayerWeaponAnimationController>();
		_weaponFirstPersonRender = _GameObjectBootstrapWeaponSystem.CreateChildNode<PlayerWeaponFirstPersonRenderer>();
		_legKickAttackController = _GameObjectBootstrapWeaponSystem.CreateChildNode<PlayerLegKickAttackController>();
		_HUDweaponController = _GameObjectBootstrapWeaponSystem.CreateChildNode<HUDweaponsController>();

		_gameObjectFirstPersonRightHandWeaponSlot = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "WeaponSlot_Hand.R");
		_gameObjectFirstPersonLeftHandWeaponSlot = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "WeaponSlot_Hand.L");
		_gameObjectThirdPersonRightHandWeaponSlot = _bootstrap.FindDeepNode(_gameObjectPlayer, "WeaponSlot_Hand.R");
		_gameObjectThirdPersonLeftHandWeaponSlot = _bootstrap.FindDeepNode(_gameObjectPlayer, "WeaponSlot_Hand.L");
		WeaponController.ConfigureWeaponSlots(
			_gameObjectFirstPersonRightHandWeaponSlot,
			_gameObjectFirstPersonLeftHandWeaponSlot,
			_gameObjectThirdPersonRightHandWeaponSlot,
			_gameObjectThirdPersonLeftHandWeaponSlot);

		PlayerResourcesAmmoManager.Initialize();

		WeaponController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerResourcesManaManager,
			_bootstrapSubProcessMenuSystem.HUDhealthAndManaController,
			PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController);

		_legKickAttackController.Initialize(
		_bootstrap,
		_inputDevice,
		_bootstrapSubProcessPlayerSystems.PlayerMovementController,
		_bootstrapSubProcessPlayerSystems.PlayerMovementStateMachineController,
		_gameObjectPlayer,
		WeaponController);

		/*
		_weaponWheelMenuController.Initialize(
		_bootstrap,
		_inputDevice,
		_localizationManager,
		_bootstrapSubProcessMenuSystem.MenuManager,
		_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
		_bootstrapSubProcessInteractionSystem.InteractionController,
		PlayerResourcesAmmoManager,
		WeaponController,
		_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
		_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
		_bootstrap.GameObjectPlayerCamera);
		*/
		
		_weaponAnimationController.Initialize(
			_bootstrap,
			_gameController,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			WeaponController,
			_legKickAttackController,
			_bootstrapSubProcessPlayerSystems.TransferBonesFirstPerson,
			_bootstrapSubProcessPlayerSystems.TransferBonesThirdPerson,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		_weaponFirstPersonRender.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			WeaponController,
			_weaponAnimationController,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandLeft,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandLeft);

		_HUDweaponController.Initialize(
			_gameController,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			_bootstrapSubProcessMenuSystem.CanvasHUDammo,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo);

		ServiceLocator.Register<PlayerWeaponAmmoController>(PlayerResourcesAmmoManager);
		ServiceLocator.Register<PlayerWeaponController>(WeaponController);
		ServiceLocator.Register<PlayerWeaponAnimationController>(_weaponAnimationController);
		ServiceLocator.Register<PlayerWeaponFirstPersonRenderer>(_weaponFirstPersonRender);
		ServiceLocator.Register<HUDweaponsController>(_HUDweaponController);

		ServiceLocator.Register(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonLeftHand, _gameObjectFirstPersonLeftHandWeaponSlot);
		ServiceLocator.Register(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonRightHand, _gameObjectFirstPersonRightHandWeaponSlot);
		ServiceLocator.Register(ServiceLocatorGameObjectsEnum.WeaponSlotThirdPersonLeftHand, _gameObjectThirdPersonLeftHandWeaponSlot);
		ServiceLocator.Register(ServiceLocatorGameObjectsEnum.WeaponSlotThirdPersonRightHand, _gameObjectThirdPersonRightHandWeaponSlot);

		return System.Threading.Tasks.Task.CompletedTask;
	}

	public void ChangeWeaponWheelType(WeaponWheelMenuTypes weaponWheelMenuTypes)
	{
		if ((weaponWheelMenuTypes == WeaponWheelMenuTypes._2D) && !(_weaponWheelMenuController is WeaponWheelMenuController2D))
		{
			if (_weaponWheelMenuController != null)
			{
				Object.Destroy(_weaponWheelMenuController as Node);
			}

			_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.CreateChildNode<WeaponWheelMenuController2D>();

			_weaponWheelMenuController.Initialize(
				_bootstrap,
				_inputDevice,
				_localizationManager,
				_bootstrapSubProcessMenuSystem.MenuManager,
				_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
				_bootstrapSubProcessInteractionSystem.InteractionController,
				PlayerResourcesAmmoManager,
				WeaponController,
				_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
				_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
				_bootstrap.GameObjectPlayerCamera
			);
		}
		if ((weaponWheelMenuTypes == WeaponWheelMenuTypes._3D) && !(_weaponWheelMenuController is WeaponWheelMenuController3D))
		{
			if (_weaponWheelMenuController != null)
			{
				Object.Destroy(_weaponWheelMenuController as Node);
			}

			_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.CreateChildNode<WeaponWheelMenuController3D>();

			_weaponWheelMenuController.Initialize(
				_bootstrap,
				_inputDevice,
				_localizationManager,
				_bootstrapSubProcessMenuSystem.MenuManager,
				_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
				_bootstrapSubProcessInteractionSystem.InteractionController,
				PlayerResourcesAmmoManager,
				WeaponController,
				_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
				_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
				_bootstrap.GameObjectPlayerCamera
			);
		}
	}
}