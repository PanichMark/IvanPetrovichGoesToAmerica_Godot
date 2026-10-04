using Godot;
public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private Node _canvasHUDmission;
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;
	private MenuBackgroundController _menuBackgroundController;
	private GameTutorialsList _tutorialsList;
	private Node _canvasMainMenuChooseMission;
	private Node _canvasBootstrapSignTermsAndConditions;

	private GameplayCanvases _gameplayCanvases;
	public ViewModelBootstrapSignTermsAndConditions ViewModelBootstrapSignTermsAndConditions { get; private set; }
	public ViewModelSavingProcess ViewModelSavingProcess { get; private set; }
	public ViewModelMainMenuChooseMission ViewModelMainMenuChooseMission { get; private set; }
	private SavingProcessController _savingProcessController;
	public ViewModelPauseMenu ViewModelPauseMenu {  get; private set; }
	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;
	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;
	private ViewModelPauseSubMenuAppearance _viewModelPauseSubMenuAppearance;
	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;
	private ViewModelPauseSubMenuSettings _viewModelPauseSubMenuSettings;
	private ViewModelPauseMenuConfirmAction _viewModelPauseMenuConfirmAction;
	private ViewModelMainMenuReadNews _viewModelMainMenuReadNews;
	private Node _canvasSavingProcess;
	public HUDmissionsController HUDmissionsController { get; private set; }
	private Node _canvasMenuChooseFirstLanguage;
	public ViewModelHUDMission ViewModelHUDMission { get; private set; }
	public ViewModelMenuWeaponWheel ViewModelWeaponWheel { get; private set; }
	public ViewModelHUDHealthAndMana ViewModelHUDhealthAndMana {  get; private set; }
	public ViewModelHUDWeapons ViewModelHUDAmmo {  get; private set; }
	public ViewModelHUDInteraction ViewModelHUDInteraction { get; private set; }
	public ViewModelMenuNote ViewModelMenuNote { get; private set; }
	public ViewModelMenuLockpickMechanical ViewModelMenuLockpickMechanical { get; private set; }
	public ViewModelMenuLockpickElectronic ViewModelMenuLockpickElectronic { get; private set; }
	public ViewModelMenuDialogue ViewModelMenuDialogue { get; private set; }

	public ViewModelMenuCutscene ViewModelMenuCutscene { get; private set; }
	public ViewModelBootstrapChooseFirstLanguage ViewModelMenuChooseFirstLanguage { get; private set; }

	private ViewModelPauseSubMenuSettingsSectionGeneral _viewModelPauseSubMenuSettingsSectionGeneral;
	private ViewModelPauseSubMenuSettingsGameDifficultyController _viewModelPauseSubMenuSettingsGameDifficultyController;
	private ViewModelPauseSubMenuSettingsSectionControls _viewModelPauseSubMenuSettingsSectionControls;
	private ViewModelPauseSubMenuSettingsSectionGraphics _viewModelPauseSubMenuSettingsSectionGraphics;
	public ViewModelPauseSubMenuSettingsSectionAudio ViewModelPauseSubMenuSettingsSectionAudio { get; private set; }
	
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameScenesManager _gameSceneManager;
	private JsonSaveLoadController _saveLoadController;

	private Node _gameObjectBootstrapMenuSystem;
	public MenuManager MenuManager { get; private set; }

	public PauseMenuController PauseMenuController { get; private set; }
	public Node CanvasHUDinteraction {  get; private set; }
	private Node _canvasMenuNote;
	private Node _canvasMenuLockpickMechanical;
	private Node _canvasMenuLockpickElectronic;
	private Node _canvasMenuDialogue;


	private Node _canvasMenuBackground;
	private Node _canvasPauseMenu;

	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private Node _canvasPauseSubMenuSave;


	private PauseSubMenuLoadController _pauseSubMenuLoadController;
	private Node _canvasPauseSubMenuLoad;


	private PauseSubMenuAppearanceController _pauseSubMenuAppearanceController;
	private Node _canvasPauseSubMenuAppearance;

	private Node _canvasHUDmonocular;
	private PauseSubMenuTutorialController _pauseSubMenuTutorialController;
	private Node _canvasPauseSubMenuTutorial;


	public PauseSubMenuSettingsController PauseSubMenuSettingsController { get; private set; }
	private Node _canvasPauseSubMenuSettings;

	public PauseSubMenuSettingsSectionGeneralController PauseSubMenuSettingsSectionGeneralController { get; private set; }


	public PauseSubMenuSettingsSectionControlsController PauseSubMenuSettingsSectionControlsController {  get; private set; }

	private PauseSubMenuSettingsGameDifficultyController _pauseSubMenuSettingsGameDifficultyController;
	private Node _canvasPauseSubMenuSettingsGameDifficultyController;


	public PauseSubMenuSettingsSectionGraphicsController PauseSubMenuSettingsSectionGraphicsController { get; private set; }


	public PauseSubMenuSettingsSectionAudioController PauseSubMenuSettingsSectionAudioController { get; private set; }


	

	public PauseMenuConfirmActionController PauseMenuConfirmActionController {  get; private set; }
	private Node _canvasMenuConfirmAction;
	

	private Node _canvasMainMenuReadNews;


	private CutsceneMenuController _cutsceneMenuController;
	private Node _canvasMenuCutscene;

	public HUDhealthAndManaController HUDhealthAndManaController { get; private set; }
	private Node _canvasHUDhealthAndMana;

	public PlayerWeaponAmmoController PlayerResourcesAmmoManager { get; private set; }
	public HUDweaponsController HUDammoController { get; private set; }

	public Node CanvasHUDammo {  get; private set; }

	public Node CanvasMenuWeaponWheel {  get; private set; }


	public BootstrapSubProcessMenuSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessSaveLoadSystem bootstrapSubProcessSaveLoadSystem,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		Node canvasMenuChooseFirstLanguage,
		Node canvasMenuBackground,
		Node canvasSavingProcess,
		Node canvasPauseMenu,
		Node canvasPauseSubMenuSave,
		Node canvasPauseSubMenuLoad,
		Node canvasPauseSubMenuAppearance,
		Node canvasPauseSubMenuTutorial,
		Node canvasPauseSubMenuSettings,
		Node canvasPauseSubMenuSettingsGameDifficultyController,
		Node canvasMenuConfirmAction,
		Node canvasMainMenuReadNews,
		Node canvasMenuWeaponWheel,
		Node canvasMenuCutscene,
		Node canvasHUDhealthAndMana,
		Node canvasHUDammo,
		Node canvasHUDinteraction,
		Node canvasHUDmission,
		Node canvasMenuNote,
		Node canvasMenuLockpickMechanical,
		Node canvasMenuLockpickElectronic,
		Node canvasMenuDialogue,
		Node canvasMainMenuChooseMission,
		Node canvasBootstrapSignTermsAndConditions,
		Node canvasHUDmonocular)
	{
		_canvasHUDmonocular = canvasHUDmonocular;
		_canvasSavingProcess = canvasSavingProcess;
		_bootstrapSubProcessSaveLoadSystem = bootstrapSubProcessSaveLoadSystem;
		_canvasBootstrapSignTermsAndConditions = canvasBootstrapSignTermsAndConditions;
		_bootstrap = bootstrap;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_saveLoadController = bootstrapSubProcessSaveLoadSystem.SaveLoadController;
		_canvasMenuChooseFirstLanguage = canvasMenuChooseFirstLanguage;
		_canvasMenuBackground = canvasMenuBackground;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		_canvasPauseSubMenuSettingsGameDifficultyController = canvasPauseSubMenuSettingsGameDifficultyController;
		_canvasMenuConfirmAction = canvasMenuConfirmAction;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_canvasMenuCutscene = canvasMenuCutscene;
		CanvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		CanvasHUDammo = canvasHUDammo;
		_canvasHUDmission = canvasHUDmission;
		CanvasHUDinteraction = canvasHUDinteraction;
		_canvasMenuLockpickMechanical = canvasMenuLockpickMechanical;
		_canvasMenuLockpickElectronic = canvasMenuLockpickElectronic;
		_canvasMenuDialogue = canvasMenuDialogue;
		_canvasMenuNote = canvasMenuNote;
		_canvasMainMenuChooseMission = canvasMainMenuChooseMission;

		_tutorialsList = bootstrap.GameData.GameTutorialsList;
	}

	public System.Threading.Tasks.Task Initialize()
	{
		_gameObjectBootstrapMenuSystem = new Node("Bootstrap_MenuSystem");
		_bootstrap.AddChild(_gameObjectBootstrapMenuSystem);

		MenuManager = _gameObjectBootstrapMenuSystem.CreateChildNode<MenuManager>();
		_menuBackgroundController = _gameObjectBootstrapMenuSystem.CreateChildNode<MenuBackgroundController>();
		PauseMenuController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseMenuController>();
		_pauseSubMenuSaveController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSaveController>();
		_pauseSubMenuLoadController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuLoadController>();
		_pauseSubMenuTutorialController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuTutorialController>();
		//_pauseSubMenuAppearanceController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuAppearanceController>();
		PauseSubMenuSettingsController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsController>();
		PauseSubMenuSettingsSectionGeneralController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsSectionGeneralController>();
		_pauseSubMenuSettingsGameDifficultyController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsGameDifficultyController>();
		PauseSubMenuSettingsSectionControlsController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsSectionControlsController>();
		PauseSubMenuSettingsSectionGraphicsController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsSectionGraphicsController>();
		PauseSubMenuSettingsSectionAudioController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseSubMenuSettingsSectionAudioController>();
		PauseMenuConfirmActionController = _gameObjectBootstrapMenuSystem.CreateChildNode<PauseMenuConfirmActionController>();
		_cutsceneMenuController = _gameObjectBootstrapMenuSystem.CreateChildNode<CutsceneMenuController>();
		HUDhealthAndManaController = _gameObjectBootstrapMenuSystem.CreateChildNode<HUDhealthAndManaController>();
		HUDammoController = _gameObjectBootstrapMenuSystem.CreateChildNode<HUDweaponsController>();
		HUDmissionsController = _gameObjectBootstrapMenuSystem.CreateChildNode<HUDmissionsController>();
		_savingProcessController = _gameObjectBootstrapMenuSystem.CreateChildNode<SavingProcessController>();

		// В методе Initialize() класса BootstrapSubProcessMenuSystem (после инициализации всех ViewModel)
		_gameplayCanvases = new GameplayCanvases(
			_canvasMenuLockpickMechanical,
			_canvasMenuLockpickElectronic,
			_canvasMenuNote,
			_canvasMenuDialogue,
			_canvasMainMenuChooseMission,
			_canvasMainMenuReadNews,
			_canvasMenuCutscene,
			_canvasHUDmonocular);

		ViewModelMenuChooseFirstLanguage = new ViewModelBootstrapChooseFirstLanguage(_bootstrap, _canvasMenuChooseFirstLanguage);

		ViewModelPauseMenu = new ViewModelPauseMenu(_bootstrap, _canvasPauseMenu);
		_viewModelPauseSubMenuSave = new ViewModelPauseSubMenuSave(_bootstrap, _canvasPauseSubMenuSave);
		_viewModelPauseSubMenuLoad = new ViewModelPauseSubMenuLoad(_bootstrap, _canvasPauseSubMenuLoad);
		//_viewModelPauseSubMenuAppearance = new ViewModelPauseSubMenuAppearance(_bootstrap, _canvasPauseSubMenuAppearance);
		_viewModelPauseSubMenuTutorial = new ViewModelPauseSubMenuTutorial(_bootstrap, _canvasPauseSubMenuTutorial);
		_viewModelPauseSubMenuSettings = new ViewModelPauseSubMenuSettings(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsGameDifficultyController = new ViewModelPauseSubMenuSettingsGameDifficultyController(_bootstrap, _canvasPauseSubMenuSettingsGameDifficultyController);
		_viewModelPauseMenuConfirmAction = new ViewModelPauseMenuConfirmAction(_bootstrap, _canvasMenuConfirmAction);
		_viewModelMainMenuReadNews = new ViewModelMainMenuReadNews(_bootstrap, _canvasMainMenuReadNews);
		ViewModelHUDhealthAndMana = new ViewModelHUDHealthAndMana(_bootstrap, _canvasHUDhealthAndMana);
		ViewModelHUDAmmo = new ViewModelHUDWeapons(_bootstrap, CanvasHUDammo);
		ViewModelWeaponWheel = new ViewModelMenuWeaponWheel(_bootstrap, CanvasMenuWeaponWheel);
		ViewModelHUDMission = new ViewModelHUDMission(_bootstrap, _canvasHUDmission);
		ViewModelSavingProcess = new ViewModelSavingProcess(_bootstrap, _canvasSavingProcess);
		ViewModelHUDInteraction = new ViewModelHUDInteraction(_bootstrap, CanvasHUDinteraction);
		ViewModelMenuNote = new ViewModelMenuNote(_bootstrap, _canvasMenuNote);
		ViewModelMenuLockpickMechanical = new ViewModelMenuLockpickMechanical(_bootstrap, _canvasMenuLockpickMechanical);
		ViewModelMenuLockpickElectronic = new ViewModelMenuLockpickElectronic(_bootstrap, _canvasMenuLockpickElectronic);
		ViewModelMenuDialogue = new ViewModelMenuDialogue(_bootstrap, _canvasMenuDialogue);
		ViewModelMenuCutscene = new ViewModelMenuCutscene(_bootstrap, _canvasMenuCutscene);
		ViewModelBootstrapSignTermsAndConditions = new ViewModelBootstrapSignTermsAndConditions(_bootstrap, _canvasBootstrapSignTermsAndConditions);
		ViewModelMainMenuChooseMission = new ViewModelMainMenuChooseMission(_bootstrap, _canvasMainMenuChooseMission);

		_viewModelPauseSubMenuSettingsSectionGeneral = new ViewModelPauseSubMenuSettingsSectionGeneral(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionControls = new ViewModelPauseSubMenuSettingsSectionControls(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionGraphics = new ViewModelPauseSubMenuSettingsSectionGraphics(_bootstrap, _canvasPauseSubMenuSettings);
		ViewModelPauseSubMenuSettingsSectionAudio = new ViewModelPauseSubMenuSettingsSectionAudio(_bootstrap, _canvasPauseSubMenuSettings);

		MenuManager.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_gameSceneManager);

		_menuBackgroundController.Initialize(
			MenuManager,
			_canvasMenuBackground);

		PauseMenuController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_localizationManager,
			_gameSceneManager,
			MenuManager,
			_menuBackgroundController,
			_canvasPauseMenu,
			ViewModelPauseMenu);

		_pauseSubMenuSaveController.Initialize(
			_bootstrap,
			_localizationManager,
			_saveLoadController,
			PauseMenuController,
			_bootstrap.GameData.GameScenesList,
			_canvasPauseSubMenuSave,
			_viewModelPauseSubMenuSave);

		_pauseSubMenuLoadController.Initialize(
			_bootstrap,
			_localizationManager,
			_saveLoadController,
			PauseMenuController,
			_bootstrap.GameData.GameScenesList,
			_canvasPauseSubMenuLoad,
			_viewModelPauseSubMenuLoad);

		/*
		_pauseSubMenuAppearanceController.Initialize(
			PauseMenuController,
			_canvasPauseSubMenuAppearance,
			_viewModelPauseSubMenuAppearance);
		*/

		_pauseSubMenuTutorialController.Initialize(
			_inputDevice,
			_localizationManager,
			PauseMenuController,
			_canvasPauseSubMenuTutorial,
			_tutorialsList,
			_viewModelPauseSubMenuTutorial);

		PauseSubMenuSettingsController.Initialize(
			_localizationManager,
			PauseMenuController,
			_canvasPauseSubMenuSettings,
			_viewModelPauseSubMenuSettings);

		PauseSubMenuSettingsSectionGeneralController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,	
			_localizationManager,
			_bootstrapSubProcessSaveLoadSystem.PauseSubMenuSettingsPlayerPrefs,
			MenuManager,
			PauseMenuController,
			PauseSubMenuSettingsController,
			_viewModelPauseSubMenuSettingsSectionGeneral);

		_pauseSubMenuSettingsGameDifficultyController.Initialize(
			_localizationManager,
			PauseSubMenuSettingsSectionGeneralController,
			_bootstrap.GameData.GameDifficultiesList,
			_canvasPauseSubMenuSettingsGameDifficultyController,
			_viewModelPauseSubMenuSettingsGameDifficultyController);

		PauseSubMenuSettingsSectionControlsController.Initialize(
			_inputDevice,
			_localizationManager,
			_bootstrapSubProcessSaveLoadSystem.PauseSubMenuSettingsPlayerPrefs,
			PauseMenuController,
			_viewModelPauseSubMenuSettingsSectionControls);

		PauseSubMenuSettingsSectionGraphicsController.Initialize(
			_localizationManager,
			_bootstrapSubProcessSaveLoadSystem.PauseSubMenuSettingsPlayerPrefs,
			_viewModelPauseSubMenuSettingsSectionGraphics);

		PauseSubMenuSettingsSectionAudioController.Initialize(
			_bootstrap,
			_localizationManager,
			_bootstrapSubProcessSaveLoadSystem.PauseSubMenuSettingsPlayerPrefs,
			PauseMenuController,
			ViewModelPauseSubMenuSettingsSectionAudio,
			_bootstrap.GameData.AudioBusLayout);

		PauseMenuConfirmActionController.Initialize(
			_gameController,
			_localizationManager,
			_gameSceneManager,
			_saveLoadController,
			MenuManager,
			PauseMenuController,
			_pauseSubMenuSaveController,
			_pauseSubMenuLoadController,
			PauseSubMenuSettingsController,
			PauseSubMenuSettingsSectionGeneralController,
			PauseSubMenuSettingsSectionControlsController,
			PauseSubMenuSettingsSectionGraphicsController,
			PauseSubMenuSettingsSectionAudioController,
			_canvasMenuConfirmAction,
			_viewModelPauseMenuConfirmAction);

		_cutsceneMenuController.Initialize(
			_gameSceneManager,
			MenuManager,
			_canvasMenuCutscene);

		HUDhealthAndManaController.Initialize(
			_gameController,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			MenuManager,
			PauseSubMenuSettingsSectionGeneralController,
			_canvasHUDhealthAndMana,
			ViewModelHUDhealthAndMana);

		HUDmissionsController.Initialize(
			_gameController,
			_localizationManager,
			_gameSceneManager,
			MenuManager,
			PauseSubMenuSettingsSectionGeneralController,
			_canvasHUDmission,
			ViewModelPauseMenu,
			ViewModelHUDMission);

		_savingProcessController.Initialize(
			_saveLoadController,
			_canvasSavingProcess,
			ViewModelSavingProcess);



ServiceLocator.Register<PauseSubMenuSettingsGameDifficultyController>(_pauseSubMenuSettingsGameDifficultyController);
ServiceLocator.Register<PauseMenuConfirmActionController>(PauseMenuConfirmActionController);
		ServiceLocator.Register<MenuManager>(MenuManager);
ServiceLocator.Register<PauseMenuController>(PauseMenuController);
ServiceLocator.Register<PauseSubMenuSettingsController>(PauseSubMenuSettingsController);
ServiceLocator.Register<PauseSubMenuSettingsSectionGeneralController>(PauseSubMenuSettingsSectionGeneralController);
		ServiceLocator.Register<MenuBackgroundController>(_menuBackgroundController);







		ServiceLocator.Register<GameplayCanvases>(_gameplayCanvases);
		ServiceLocator.Register<HUDmissionsController>(HUDmissionsController);





		ServiceLocator.Register<ViewModelMenuDialogue>(ViewModelMenuDialogue);


		ServiceLocator.Register<ViewModelMainMenuChooseMission>(ViewModelMainMenuChooseMission);
ServiceLocator.Register<ViewModelMainMenuReadNews>(_viewModelMainMenuReadNews);
		ServiceLocator.Register<ViewModelMenuCutscene>(ViewModelMenuCutscene);
		ServiceLocator.Register<ViewModelMenuNote>(ViewModelMenuNote);
		ServiceLocator.Register<ViewModelMenuLockpickMechanical>(ViewModelMenuLockpickMechanical);
		ServiceLocator.Register<ViewModelMenuLockpickElectronic>(ViewModelMenuLockpickElectronic);
		ServiceLocator.Register<ViewModelHUDWeapons>(ViewModelHUDAmmo);
		ServiceLocator.Register<ViewModelHUDInteraction>(ViewModelHUDInteraction);

		return System.Threading.Tasks.Task.CompletedTask;
	}
}