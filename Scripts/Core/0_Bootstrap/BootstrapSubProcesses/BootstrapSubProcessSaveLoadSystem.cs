using Godot;
public class BootstrapSubProcessSaveLoadSystem
{
	private Node _gameObjectBootstrapSaveLoadSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private GameScenesManager _gameSceneManager;
	public PlayerPrefsSettingsController PauseSubMenuSettingsPlayerPrefs { get; private set; }
	private IInputDevice _inputDevice;
	public JsonSaveLoadController SaveLoadController { get; private set; }

	public BootstrapSubProcessSaveLoadSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapSaveLoadSystem = new Node("Bootstrap_SaveLoadSystem");
		_bootstrap.AddChild(_gameObjectBootstrapSaveLoadSystem);

		SaveLoadController = _gameObjectBootstrapSaveLoadSystem.CreateChildNode<JsonSaveLoadController>();
		PauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapSaveLoadSystem.CreateChildNode<PlayerPrefsSettingsController>();

		SaveLoadController.Initialize(
			_bootstrap,
			_gameSceneManager,
			_gameController);

		PauseSubMenuSettingsPlayerPrefs.Initialize(
			_bootstrap,
			_inputDevice);

		ServiceLocator.Register<JsonSaveLoadController>(SaveLoadController);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}