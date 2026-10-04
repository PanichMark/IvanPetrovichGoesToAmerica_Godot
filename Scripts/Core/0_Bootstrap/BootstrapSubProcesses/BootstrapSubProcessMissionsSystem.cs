using Godot;
public class BootstrapSubProcessMissionsSystem
{
	private Bootstrap _bootstrap;
	private Node _gameObjectBootstrapMissionsSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;
	private Node _playerCameraGameObject;
	private MissionGoalMarkerController _missionGoalMarkerManager;
	private LocalizationManager _localizationManager;
	public MissionsManager MissionsManager { get; private set; }
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private GameMissionsList _gameMissions;

	public BootstrapSubProcessMissionsSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessSaveLoadSystem bootstrapSubProcessSaveLoad,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		Node playerCameraGameObject)
	{
		_bootstrapSubProcessSaveLoadSystem = bootstrapSubProcessSaveLoad;
		_bootstrap = bootstrap;
		_localizationManager = _bootstrap.LocalizationManager;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_playerCameraGameObject = playerCameraGameObject;
		_gameMissions = _bootstrap.GameData.GameMissionsList;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapMissionsSystem = new Node("Bootstrap_MissionsSystem");
		_bootstrap.AddChild(_gameObjectBootstrapMissionsSystem);
		
		MissionsManager = _gameObjectBootstrapMissionsSystem.CreateChildNode<MissionsManager>();

		_missionGoalMarkerManager = _gameObjectBootstrapMissionsSystem.CreateChildNode<MissionGoalMarkerController>();

		MissionsManager.Initialize(
			_localizationManager,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
				_bootstrapSubProcessSaveLoadSystem.SaveLoadController,
			_bootstrapSubProcessMenuSystem.HUDmissionsController,
			_gameMissions);

		_missionGoalMarkerManager.Initialize(
			_bootstrap,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			MissionsManager, 
			_playerCameraGameObject,
			_bootstrapSubProcessMenuSystem.ViewModelHUDMission.ImageMissionGoalMarker);

		ServiceLocator.Register<MissionsManager>(MissionsManager);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}
