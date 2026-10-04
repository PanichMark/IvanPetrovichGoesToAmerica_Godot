using Godot;
public class BootstrapSubProcessScenesSystem
{
	private Node _gameObjectBootstrapGameSceneSystem;
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private Node _canvasSceneLoadingScreen;
	public GameScenesManager GameSceneManager { get; private set; }
	private ViewModelSceneLoadingScreen _viewModelSceneLoadingScreen;
	public BootstrapSubProcessScenesSystem(
		Bootstrap bootstrap,
		GameController gameController,
		LocalizationManager localizationManager,
		Node canvasSceneLoadingScreen)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_localizationManager = localizationManager;
		_canvasSceneLoadingScreen = canvasSceneLoadingScreen;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapGameSceneSystem = new Node("Bootstrap_GameSceneSystem");
		_bootstrap.AddChild(_gameObjectBootstrapGameSceneSystem);
		GameSceneManager = _gameObjectBootstrapGameSceneSystem.CreateChildNode<GameScenesManager>();

		_viewModelSceneLoadingScreen = new ViewModelSceneLoadingScreen(_bootstrap, _canvasSceneLoadingScreen);

		GameSceneManager.Initialize(
			_gameController,
			_localizationManager,
			_bootstrap.GameData.GameScenesList,
			_canvasSceneLoadingScreen,
			_viewModelSceneLoadingScreen);

		ServiceLocator.Register<GameScenesManager>(GameSceneManager);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}