using Godot;
public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private GameScenesManager _gameSceneManager;
	public Node GameObjectSpineSlot {  get; private set; }
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private Node _gameObjectBootstrapInteractionSystem;
	public PlayerInteractionController InteractionController { get; private set; }

	private Node _gameObjectPlayer;
	private Node _gameObjectPlayerCamera;

	private KeysManager _keysManager;

	public BootstrapSubProcessInteractionSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		Node gameObjectPlayer,
		Node gameObjectPlayerCamera)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_playerBehaviour = bootstrapSubProcessPlayerSystems.PlayerBehaviour;
		_playerCameraController = bootstrapSubProcessPlayerSystems.PlayerCameraController;
		_playerCameraStateMachineController = bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController;

		_gameObjectPlayer = gameObjectPlayer;
		_gameObjectPlayerCamera = gameObjectPlayerCamera;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapInteractionSystem = new Node("Bootstrap_InteractionSystem");
		_bootstrap.AddChild(_gameObjectBootstrapInteractionSystem);

		InteractionController = _gameObjectBootstrapInteractionSystem.CreateChildNode<PlayerInteractionController>();
		_keysManager = _gameObjectBootstrapInteractionSystem.CreateChildNode<KeysManager>();

		GameObjectSpineSlot = _bootstrap.FindDeepNode(_gameObjectPlayer, "Spine");

		InteractionController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_localizationManager,
			_gameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_playerBehaviour,
			_playerCameraController,
			_playerCameraStateMachineController,
			_bootstrapSubProcessMenuSystem.CanvasHUDinteraction,
			_bootstrapSubProcessMenuSystem.ViewModelHUDInteraction);

		_keysManager.Initialize();

		ServiceLocator.Register<PlayerInteractionController>(InteractionController);
		ServiceLocator.Register<KeysManager>(_keysManager);

		ServiceLocator.Register(ServiceLocatorGameObjectsEnum.PlayerSpineBone, GameObjectSpineSlot);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}