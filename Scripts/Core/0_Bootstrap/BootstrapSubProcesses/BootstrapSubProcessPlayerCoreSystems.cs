using Godot;
public class BootstrapSubProcessPlayerSystems
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private GameScenesManager _gameSceneManager;

	private Node _gameObjectPlayer;
	private Node _gameObjectPlayerCollider;
	private Node _canvasMenuBackground;

	private Node _gameObjectPlayerHead;
	public Node GameObjectPlayerThirdPersonHandRight { get; private set; }
	public Node GameObjectPlayerThirdPersonHandLeft { get; private set; }
	public Node GameObjectPlayerFirstPersonHandRight { get; private set; }
	public Node GameObjectPlayerFirstPersonHandLeft { get; private set; }

	public TransferSkinnedMeshRendererArmatureBones TransferBonesFirstPerson { get; private set; }
	public TransferSkinnedMeshRendererArmatureBones TransferBonesThirdPerson {  get; private set; }	

	public AudioStreamPlayer PlayerAudioVoice { get; private set; }
	public AudioStreamPlayer PlayerAudioMovement { get; private set; }
	public AudioStreamPlayer PlayerAudioWeaponRight { get; private set; }
	public AudioStreamPlayer PlayerAudioWeaponLeft { get; private set; }

	public Node GameObjectPlayerHatSlot { get; private set; }

	private Node _gameObjectPlayerCamera;
	public Node PlayerCameraFirstPerson { get; private set; }
	public Node PlayerCameraPostProcessing {  get; private set; }
	private Node _gameobjectPlayerEyesLookAt;
	public PlayerBehaviourController PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	public PlayerMovementStateMachineController PlayerMovementStateMachineController { get; private set; }
	private PlayerColliderController _playerColliderController;
	
	public PlayerCameraController PlayerCameraController { get; private set; }
	public PlayerCameraStateMachineController PlayerCameraStateMachineController { get; private set; }
	private PlayerCameraVolumeController _playerCameraBlurFilter;
	private PlayerCameraFirstPersonRenderer _playerCameraFirstPersonRender;

	private PlayerMovementAnimationController _playerMovementAnimationController;

	public PlayerHealthController PlayerResourcesHealthManager {  get; private set; }

	public PlayerManaController PlayerResourcesManaManager { get; private set; }

	public PlayerMoneyController PlayerResourcesMoneyManager { get; private set; }

	public BootstrapSubProcessPlayerSystems(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameController gameController,
		IInputDevice inputDevice,
		Node canvasMenuBackground,
		Node playerGameObject,
		Node playerMainCameraGameObject)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_canvasMenuBackground = canvasMenuBackground;
		_gameObjectPlayer = playerGameObject;
		_gameObjectPlayerCamera = playerMainCameraGameObject;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectPlayerCollider = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerCollider");
		PlayerCameraFirstPerson = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "CameraFirstPerson");
		PlayerCameraPostProcessing = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "CameraIgnorePostProcessing");

		PlayerBehaviour = _gameObjectPlayer.FindNodeOfType<PlayerBehaviourController>();
		PlayerMovementController = _gameObjectPlayer.FindNodeOfType<PlayerMovementController>();
		PlayerMovementStateMachineController = _gameObjectPlayer.FindNodeOfType<PlayerMovementStateMachineController>();
		_playerColliderController = _gameObjectPlayer.GetComponentInChildren<PlayerColliderController>();
		_playerMovementAnimationController = _gameObjectPlayer.FindNodeOfType<PlayerMovementAnimationController>();

		PlayerCameraController = _gameObjectPlayerCamera.FindNodeOfType<PlayerCameraController>();
		PlayerCameraStateMachineController = _gameObjectPlayerCamera.FindNodeOfType<PlayerCameraStateMachineController>();
		_playerCameraBlurFilter = _gameObjectPlayerCamera.FindNodeOfType<PlayerCameraVolumeController>();
		_playerCameraFirstPersonRender = _gameObjectPlayerCamera.FindNodeOfType<PlayerCameraFirstPersonRenderer>();

		PlayerResourcesHealthManager = _gameObjectPlayer.FindNodeOfType<PlayerHealthController>();
		PlayerResourcesManaManager = _gameObjectPlayer.FindNodeOfType<PlayerManaController>();
		PlayerResourcesMoneyManager = _gameObjectPlayer.FindNodeOfType<PlayerMoneyController>();

		_gameObjectPlayerHead = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerHead");
		_gameobjectPlayerEyesLookAt = _bootstrap.FindDeepNode(_gameObjectPlayer, "EyesLookAt");
		GameObjectPlayerHatSlot = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerHatSlot");

		GameObjectPlayerFirstPersonHandRight = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "PlayerFirstPersonArmRight");
		GameObjectPlayerFirstPersonHandLeft = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "PlayerFirstPersonArmLeft");
		GameObjectPlayerThirdPersonHandRight = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerThirdPersonArmRight");
		GameObjectPlayerThirdPersonHandLeft = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerThirdPersonArmLeft");

		PlayerAudioVoice = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerAudioVoice").FindNodeOfType<AudioStreamPlayer>();
		PlayerAudioMovement = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerAudioMovement").FindNodeOfType<AudioStreamPlayer>();
		PlayerAudioWeaponRight = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerAudioWeaponRight").FindNodeOfType<AudioStreamPlayer>();
		PlayerAudioWeaponLeft = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerAudioWeaponLeft").FindNodeOfType<AudioStreamPlayer>();

		TransferBonesThirdPerson = _gameObjectPlayer.FindNodeOfType<TransferSkinnedMeshRendererArmatureBones>();
		TransferBonesFirstPerson = _gameObjectPlayerCamera.FindNodeOfType<TransferSkinnedMeshRendererArmatureBones>();

		var canvasComponentBackgroundMenu = _canvasMenuBackground.FindNodeOfType<Canvas>();
		var PlayerCameraComponentPostProcessing = PlayerCameraPostProcessing.FindNodeOfType<Camera3D>();
		canvasComponentBackgroundMenu.worldCamera = PlayerCameraComponentPostProcessing;
		canvasComponentBackgroundMenu.planeDistance = 2;

		PlayerBehaviour.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager);

		PlayerMovementController.Initialize(_bootstrap,
			_gameSceneManager,
			PlayerBehaviour);

		PlayerMovementStateMachineController.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager,
			PlayerMovementController);

		_playerColliderController.Initialize(
			_bootstrap,
			PlayerMovementStateMachineController);

		PlayerCameraController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionControlsController,
			PlayerMovementController,
			_playerColliderController,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		PlayerCameraStateMachineController.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager,
			//_bootstrapSubProcessMenuSystem.PauseMenuConfirmActionController,
			PlayerMovementController,
			PlayerMovementStateMachineController,
			PlayerCameraController);

		_playerCameraBlurFilter.Initialize(
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			PlayerCameraFirstPerson);

		_playerCameraFirstPersonRender.Initialize(
			PlayerCameraStateMachineController,
			_gameObjectPlayerHead,
			GameObjectPlayerHatSlot);

		_playerMovementAnimationController.Initialize(
			_gameController,
			_inputDevice,
			PlayerBehaviour,
			PlayerMovementController,
			PlayerMovementStateMachineController,
			PlayerCameraStateMachineController,
			_gameObjectPlayer);

		PlayerResourcesHealthManager.Initialize(
			_bootstrap,
			_gameController,
			_playerMovementAnimationController,
			PlayerMovementStateMachineController,
			_bootstrapSubProcessMenuSystem.ViewModelHUDhealthAndMana,
			_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel);

		PlayerResourcesManaManager.Initialize(
			_bootstrapSubProcessMenuSystem.ViewModelHUDhealthAndMana,
			_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel);

		PlayerResourcesMoneyManager.Initialize(
			_bootstrapSubProcessMenuSystem.ViewModelPauseMenu.TextCurrentPlayerMoneyDisplay);

	ServiceLocator.Register<PlayerBehaviourController>(PlayerBehaviour);
ServiceLocator.Register<PlayerMovementController>(PlayerMovementController);
ServiceLocator.Register<PlayerMovementStateMachineController>(PlayerMovementStateMachineController);
ServiceLocator.Register<PlayerCameraController>(PlayerCameraController);
ServiceLocator.Register<PlayerCameraStateMachineController>(PlayerCameraStateMachineController);
ServiceLocator.Register<PlayerCameraVolumeController>(_playerCameraBlurFilter);

ServiceLocator.Register<PlayerHealthController>(PlayerResourcesHealthManager);
ServiceLocator.Register<PlayerManaController>(PlayerResourcesManaManager);
ServiceLocator.Register<PlayerMoneyController>(PlayerResourcesMoneyManager);

		ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioVoice, PlayerAudioVoice);
ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioMovement, PlayerAudioMovement);
ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponRight, PlayerAudioWeaponRight);
ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponLeft, PlayerAudioWeaponLeft);

	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.Player, _gameObjectPlayer);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerEyes, _gameobjectPlayerEyesLookAt);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerHead, _gameObjectPlayerHead);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerCollider, _gameObjectPlayerCollider);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerCamera, _gameObjectPlayerCamera);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}