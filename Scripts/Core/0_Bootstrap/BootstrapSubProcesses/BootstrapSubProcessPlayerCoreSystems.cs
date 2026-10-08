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

	public Node TransferBonesFirstPerson { get; private set; }
	public Node TransferBonesThirdPerson {  get; private set; }

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
		PlayerCameraFirstPerson = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "CameraFirstPerson") ?? _gameObjectPlayerCamera;
		PlayerCameraPostProcessing = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "CameraIgnorePostProcessing") ?? _gameObjectPlayerCamera;

		PlayerBehaviour = FindOrCreate<PlayerBehaviourController>(_gameObjectPlayer);
		PlayerMovementController = FindOrCreate<PlayerMovementController>(_gameObjectPlayer);
		PlayerMovementStateMachineController = FindOrCreate<PlayerMovementStateMachineController>(_gameObjectPlayer);
		_playerColliderController = FindOrCreate<PlayerColliderController>(_gameObjectPlayer);
		_playerMovementAnimationController = FindOrCreate<PlayerMovementAnimationController>(_gameObjectPlayer);

		PlayerCameraController = FindOrCreate<PlayerCameraController>(_gameObjectPlayerCamera);
		PlayerCameraStateMachineController = FindOrCreate<PlayerCameraStateMachineController>(_gameObjectPlayerCamera);
		_playerCameraBlurFilter = FindOrCreate<PlayerCameraVolumeController>(_gameObjectPlayerCamera);
		_playerCameraFirstPersonRender = FindOrCreate<PlayerCameraFirstPersonRenderer>(_gameObjectPlayerCamera);

		PlayerResourcesHealthManager = FindOrCreate<PlayerHealthController>(_gameObjectPlayer);
		PlayerResourcesManaManager = FindOrCreate<PlayerManaController>(_gameObjectPlayer);
		PlayerResourcesMoneyManager = FindOrCreate<PlayerMoneyController>(_gameObjectPlayer);

		_gameObjectPlayerHead = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerHead");
		_gameobjectPlayerEyesLookAt = _bootstrap.FindDeepNode(_gameObjectPlayer, "EyesLookAt") ?? _gameObjectPlayer;
		GameObjectPlayerHatSlot = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerHatSlot");

		GameObjectPlayerFirstPersonHandRight = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "PlayerFirstPersonArmRight");
		GameObjectPlayerFirstPersonHandLeft = _bootstrap.FindDeepNode(_gameObjectPlayerCamera, "PlayerFirstPersonArmLeft");
		GameObjectPlayerThirdPersonHandRight = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerThirdPersonArmRight");
		GameObjectPlayerThirdPersonHandLeft = _bootstrap.FindDeepNode(_gameObjectPlayer, "PlayerThirdPersonArmLeft");

		PlayerAudioVoice = FindAudioPlayer(_gameObjectPlayer, "PlayerAudioVoice");
		PlayerAudioMovement = FindAudioPlayer(_gameObjectPlayer, "PlayerAudioMovement");
		PlayerAudioWeaponRight = FindAudioPlayer(_gameObjectPlayer, "PlayerAudioWeaponRight");
		PlayerAudioWeaponLeft = FindAudioPlayer(_gameObjectPlayer, "PlayerAudioWeaponLeft");

		TransferBonesThirdPerson = _gameObjectPlayer.FindDeepNode("TransferBonesThirdPerson");
		TransferBonesFirstPerson = _gameObjectPlayerCamera.FindDeepNode("TransferBonesFirstPerson");

		var canvasComponentBackgroundMenu = _canvasMenuBackground.FindDeepNodeOfType<Canvas>();
		var cameraPostProcessing = PlayerCameraPostProcessing.FindDeepNodeOfType<Camera3D>();
		if (canvasComponentBackgroundMenu != null && cameraPostProcessing != null)
		{
			canvasComponentBackgroundMenu.worldCamera = cameraPostProcessing;
			canvasComponentBackgroundMenu.planeDistance = 2;
		}

		PlayerBehaviour.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager);

		PlayerMovementController.Initialize(_bootstrap,
			_gameSceneManager,
			PlayerBehaviour);
		PlayerMovementController.SetInputDevice(_inputDevice);

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

		_playerCameraBlurFilter?.Initialize(
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			PlayerCameraFirstPerson);

		_playerCameraFirstPersonRender?.Initialize(
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
		PlayerMovementStateMachineController.SetHealthController(PlayerResourcesHealthManager);

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
	if (_playerCameraBlurFilter != null) ServiceLocator.Register<PlayerCameraVolumeController>(_playerCameraBlurFilter);

ServiceLocator.Register<PlayerHealthController>(PlayerResourcesHealthManager);
ServiceLocator.Register<PlayerManaController>(PlayerResourcesManaManager);
ServiceLocator.Register<PlayerMoneyController>(PlayerResourcesMoneyManager);

		if (PlayerAudioVoice != null) ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioVoice, PlayerAudioVoice);
		if (PlayerAudioMovement != null) ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioMovement, PlayerAudioMovement);
		if (PlayerAudioWeaponRight != null) ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponRight, PlayerAudioWeaponRight);
		if (PlayerAudioWeaponLeft != null) ServiceLocator.Register(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponLeft, PlayerAudioWeaponLeft);

	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.Player, _gameObjectPlayer);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerEyes, _gameobjectPlayerEyesLookAt);
	if (_gameObjectPlayerHead != null) ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerHead, _gameObjectPlayerHead);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerCollider, _gameObjectPlayerCollider);
	ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerCamera, _gameObjectPlayerCamera);

		return System.Threading.Tasks.Task.CompletedTask;
	}

	private static T FindOrCreate<T>(Node parent) where T : Node, new()
	{
		T existing = parent.FindDeepNodeOfType<T>();
		if (existing != null)
			return existing;

		T created = new();
		created.Name = typeof(T).Name;
		parent.AddChild(created);
		return created;
	}

	private static AudioStreamPlayer FindAudioPlayer(Node root, string nodeName)
	{
		Node audioNode = root?.FindDeepNode(nodeName);
		return audioNode?.FindDeepNodeOfType<AudioStreamPlayer>();
	}
}