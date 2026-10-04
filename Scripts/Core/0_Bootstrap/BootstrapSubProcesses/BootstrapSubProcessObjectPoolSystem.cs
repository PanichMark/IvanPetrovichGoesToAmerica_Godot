using Godot;
public class BootstrapSubProcessObjectPoolSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private PlayerPrefsSettingsController _pauseSubMenuSettingsPlayerPrefs;
	private BootstrapSubProcessMenuSystem _subProcessMenuSystem;
	private Node _gameObjectBootstrapObjectPoolSystem;
	private ObjectPoolWeaponController _objectPoolWeaponController;

	public BootstrapSubProcessObjectPoolSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapObjectPoolSystem = new Node("Bootstrap_ObjectPoolSystem");
		_bootstrap.AddChild(_gameObjectBootstrapObjectPoolSystem);

		_objectPoolWeaponController = _gameObjectBootstrapObjectPoolSystem.CreateChildNode<ObjectPoolWeaponController>();

		_objectPoolWeaponController.Initialize(
			_bootstrap,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController);

		ServiceLocator.Register<ObjectPoolWeaponController>(_objectPoolWeaponController);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}