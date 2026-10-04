using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class Bootstrap : Node
{
	public delegate void SettingsDataEventHandler();
	public event SettingsDataEventHandler OnLoadSettingsData;

	[Export] private BootstrapGameDataList _gameData;
	public BootstrapGameDataList GameData => _gameData;

	[Export] private ConfigBootstrapInitializationScreenDuration _initializationScreenDuration;
	[Export] private ConfigBootstrapKeyPauseMenu _keyPauseMenu;
	[Export] private GameScenesSystemEnum _firstSceneToLoad;

	[Export] private ConfigPlayerPrefsPresequisites _playerPrefsPresequisites;
	[Export] private ConfigPlayerPrefsReset _playerPrefsReset;

	[Export] private bool _applyConfigsPlayer;
	[Export] private ConfigPlayerTransform _playerTransform;
	[Export] private int _playerHealth = 100;
	[Export] private int _playerHealingItems;
	[Export] private int _playerMana = 100;
	[Export] private int _playerManaReplenishItems;
	[Export] private int _playerMoney;
	[Export] private ConfigPlayerWeapons _playerWeapons;
	[Export] private ConfigPlayerAmmo _playerAmmo;

	[Export] private bool _applyConfigsMission;
	[Export] private Mission _mission;
	[Export] private int _missionStep;

	private ViewModelBootstrapInitialization _viewModelBootstrapInitialization;

	private Node _canvasBootstrapInitialization;
	private Node _canvasBootstrapChooseFirstLanguage;
	private Node _canvasBootstrapSignTermsAndConditions;
	private Node _canvasSceneLoadingScreen;
	private Node _canvasSavingProcess;
	private Node _canvasMenuBackground;
	private Node _canvasPauseMenu;
	private Node _canvasPauseSubMenuSave;
	private Node _canvasPauseSubMenuLoad;
	private Node _canvasPauseSubMenuAppearance;
	private Node _canvasPauseSubMenuTutorial;
	private Node _canvasPauseSubMenuSettings;
	private Node _canvasPauseSubMenuSettingsGameDifficulty;
	private Node _canvasPauseMenuConfirmAction;
	public Node _canvasMainMenuChooseMission {  get; private set; }
	public Node _canvasMainMenuReadNews { get; private set; }
	private Node _canvasHUDinteraction;
	private Node _canvasHUDmission;
	private Node _canvasHUDhealthAndMana;
	private Node _canvasHUDweapons;
	private Node _canvasHUDmonocular;
	private Node _canvasMenuWeaponWheel;
	public Node _canvasMenuNote {  get; private set; }
	public Node _canvasMenuLockpickMechanical {  get; private set; }
	public Node _canvasMenuLockpickElectronic { get; private set; }
	public Node _canvasMenuDialogue { get; private set; }
	public Node _canvasMenuCutscene { get; private set; }

	private GameController _gameController;
	private IInputDevice _inputDevice;
	public LocalizationManager LocalizationManager { get; private set; }
	private readonly ConfigFile _bootstrapPreferences = new();

	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;
	private BootstrapSubProcessMissionsSystem _bootstrapSubProcessMissionsSystem;
	private BootstrapSubProcessObjectPoolSystem _bootstrapSubProcessObjectPoolSystem;

	private Key _keyCodePauseMenu;

	private Node _gameObjectPlayer;
	public Node GameObjectPlayerCamera { get; private set; }
	private Node _gameObjectBootstrapTemporaryCamera;
	[Export] private PackedScene _playerScene;
	[Export] private PackedScene _playerCameraScene;
	public bool IsBootstrapInitialized { get; private set; }
	private const string BootstrapPreferencesPath = "user://bootstrap.cfg";

	public override async void _Ready()
	{
		GD.Print("!!! STARTED GAME INITIALIZATION !!!");

		ServiceLocator.ClearAllServices();

		_canvasBootstrapInitialization = _gameData.GameCanvasesList.CanvasBootstrapInitialization.Instantiate();
		AddChild(_canvasBootstrapInitialization);
		_canvasBootstrapInitialization.SetActive(true);
		_viewModelBootstrapInitialization = new ViewModelBootstrapInitialization(this, _canvasBootstrapInitialization);
		_viewModelBootstrapInitialization.BootstrapInitializationPart1.SetActive(true);

		Input.MouseMode = Input.MouseModeEnum.Captured;

		CreateBootstrapTemporaryCamera();

		await BootstrapSystemsInitialization();

		float duration = 5;
		if (_initializationScreenDuration == ConfigBootstrapInitializationScreenDuration.Instant)
		{
			duration = 0;
		}

		await WaitForDurationOrInput(duration);

		_viewModelBootstrapInitialization.BootstrapInitializationPart1.SetActive(false);
		_viewModelBootstrapInitialization.BootstrapInitializationPart2.SetActive(true);

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await WaitForDurationOrInput(duration);

		_viewModelBootstrapInitialization.BootstrapInitializationPart2.SetActive(false);

		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		GD.Print("!!! GAME INITIALIZED !!!");

		await _bootstrapSubProcessSaveLoadSystem.SaveLoadController.NewGame();

		if (_playerPrefsReset == ConfigPlayerPrefsReset.Yes)
		{
			_bootstrapPreferences.Clear();
			_bootstrapPreferences.Save(BootstrapPreferencesPath);
		}
		else
		{
			LoadBootstrapPreferences();
		}
		if (!_bootstrapPreferences.GetValue("Bootstrap", "PrerequisitesMet", false).AsBool() || _playerPrefsPresequisites == ConfigPlayerPrefsPresequisites.No)
		{
			await BootstrapPrerequisites();
		}
		else
		{
			string storedLanguage = _bootstrapPreferences.GetValue("Settings", "Language", LanguagesEnum.English.ToString()).AsString();
			ChangeLanguage(Enum.TryParse(storedLanguage, out LanguagesEnum language) ? language : LanguagesEnum.English);
		}

		_viewModelBootstrapInitialization.BootstrapInitializationPart2.SetActive(false);
		_viewModelBootstrapInitialization.BootstrapInitializationPart3.SetActive(true);
		((Label)_viewModelBootstrapInitialization.TextSavingProcessIcon.FindChild("*", true, false)).Text = LocalizationManager.GetLocalizedString("UI_Menu_Bootstrap_SavingProcess");

		await WaitForDurationOrInput(duration * 1.5f);

		_canvasBootstrapInitialization.SetActive(false);
		_gameObjectBootstrapTemporaryCamera.QueueFree();

		await LoadFirstGameplayScene();

		if (_applyConfigsPlayer)
		{
			ApplyBootstrapPlayerConfigs();
		}

		if (_applyConfigsMission)
		{
			ApplyBootstrapMissionConfigs();
		}

		OnLoadSettingsData?.Invoke();

		IsBootstrapInitialized = true;
	}

	public override void _Process(double delta)
	{
		if (IsBootstrapInitialized)
			return;

		RotateGear(300f, (float)delta);
	}

	private void RotateGear(float speed, float delta)
	{
		((Node2D)_viewModelBootstrapInitialization.Gear).Rotation += Mathf.DegToRad(speed) * delta;
	}

	private async Task BootstrapSystemsInitialization()
	{
		InitializeInterfaces();
		InitializeCanvases();
		await InitializeSceneSystem();
		await InitializeSaveLoadSystem();
		await InitializeMenuSystem();
		await InitializePlayerSystems();
		await InitializeInteractionSystem();
		await InitializeWeaponSystem();
		await InitializeMissionsSystem();
		await InitializeObjectPoolSystem();
		RegisterBootstrapDependencies();
	}

	private void InitializeInterfaces()
	{
		_gameController = new GameController();

		if (_keyPauseMenu == ConfigBootstrapKeyPauseMenu.DEFAULT_Escape)
		{
			_keyCodePauseMenu = Key.Escape;
		}
		else
		{
			_keyCodePauseMenu = Key.Key1;
		}

		_inputDevice = new InputKeyboard(_gameController, _keyCodePauseMenu);

		LocalizationManager = new LocalizationManager(this);

		GD.Print("=== INTERFACES INITIALIZED ===");
	}

	private void InitializeCanvases()
	{
		_canvasBootstrapChooseFirstLanguage = _gameData.GameCanvasesList.CanvasBootstrapChooseFirstLanguage.Instantiate();
		AddChild(_canvasBootstrapChooseFirstLanguage);
		_canvasBootstrapSignTermsAndConditions = _gameData.GameCanvasesList.CanvasBootstrapSignTermsAndConditions.Instantiate();
		AddChild(_canvasBootstrapSignTermsAndConditions);

		_canvasSceneLoadingScreen = _gameData.GameCanvasesList.CanvasSceneLoadingScreen.Instantiate();
		AddChild(_canvasSceneLoadingScreen);
	
		_canvasSavingProcess = _gameData.GameCanvasesList.CanvasSavingProcess.Instantiate();
		AddChild(_canvasSavingProcess);

		_canvasMenuBackground = _gameData.GameCanvasesList.CanvasMenuBackground.Instantiate();
		AddChild(_canvasMenuBackground);

	    _canvasPauseMenu = _gameData.GameCanvasesList.CanvasPauseMenu.Instantiate();
		AddChild(_canvasPauseMenu);
		_canvasPauseSubMenuSave = _gameData.GameCanvasesList.CanvasPauseSubMenuSave.Instantiate();
		AddChild(_canvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = _gameData.GameCanvasesList.CanvasPauseSubMenuLoad.Instantiate();
		AddChild(_canvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = _gameData.GameCanvasesList.CanvasPauseSubMenuAppearance.Instantiate();
		AddChild(_canvasPauseSubMenuAppearance);
		_canvasPauseSubMenuTutorial = _gameData.GameCanvasesList.CanvasPauseSubMenuTutorial.Instantiate();
		AddChild(_canvasPauseSubMenuTutorial);
		_canvasPauseSubMenuSettings = _gameData.GameCanvasesList.CanvasPauseSubMenuSettings.Instantiate();
		AddChild(_canvasPauseSubMenuSettings);
		_canvasPauseSubMenuSettingsGameDifficulty = _gameData.GameCanvasesList.CanvasPauseSubMenuSettingsGameDifficulty.Instantiate();
		AddChild(_canvasPauseSubMenuSettingsGameDifficulty);
		_canvasPauseMenuConfirmAction = _gameData.GameCanvasesList.CanvasPauseMenuConfirmAction.Instantiate();
		AddChild(_canvasPauseMenuConfirmAction);

		_canvasMainMenuChooseMission = _gameData.GameCanvasesList.CanvasMainMenuChooseMission.Instantiate();
		AddChild(_canvasMainMenuChooseMission);
		_canvasMainMenuReadNews = _gameData.GameCanvasesList.CanvasMainMenuReadNews.Instantiate();
		AddChild(_canvasMainMenuReadNews);

		_canvasMenuWeaponWheel = _gameData.GameCanvasesList.CanvasMenuWeaponWheel.Instantiate();
		AddChild(_canvasMenuWeaponWheel);
		_canvasMenuCutscene = _gameData.GameCanvasesList.CanvasMenuCutscene.Instantiate();
		AddChild(_canvasMenuCutscene);

		_canvasHUDhealthAndMana = _gameData.GameCanvasesList.CanvasHUDhealthAndMana.Instantiate();
		AddChild(_canvasHUDhealthAndMana);
		_canvasHUDweapons = _gameData.GameCanvasesList.CanvasHUDweapons.Instantiate();
		AddChild(_canvasHUDweapons);
		_canvasHUDinteraction = _gameData.GameCanvasesList.CanvasHUDinteraction.Instantiate();
		AddChild(_canvasHUDinteraction);
		_canvasHUDmission = _gameData.GameCanvasesList.CanvasHUDmission.Instantiate();
		AddChild(_canvasHUDmission);
		_canvasHUDmonocular = _gameData.GameCanvasesList.CanvasHUDmonocular.Instantiate();
		AddChild(_canvasHUDmonocular);

		_canvasMenuNote = _gameData.GameCanvasesList.CanvasMenuNote.Instantiate();
		AddChild(_canvasMenuNote);
		_canvasMenuLockpickElectronic = _gameData.GameCanvasesList.CanvasMenuLockpickElectronic.Instantiate();
		AddChild(_canvasMenuLockpickElectronic);
		_canvasMenuLockpickMechanical = _gameData.GameCanvasesList.CanvasMenuLockpickMechanical.Instantiate();
		AddChild(_canvasMenuLockpickMechanical);
		_canvasMenuDialogue = _gameData.GameCanvasesList.CanvasMenuDialogue.Instantiate();
		AddChild(_canvasMenuDialogue);

		GD.Print("=== CANVASES INITIALIZED ===");
	}

	private async Task InitializeSceneSystem()
	{
		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessScenesSystem(
			this, 
			_gameController,
			LocalizationManager,
			_canvasSceneLoadingScreen);

		await _bootstrapSubProcessSceneSystem.Initialize();

		GD.Print("=== SCENE SYSTEM INITIALIZED ===");
	}

	private async Task InitializeSaveLoadSystem()
	{
		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(
			this,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem);

		await _bootstrapSubProcessSaveLoadSystem.Initialize();

		GD.Print("=== SAVELOAD SYSTEM INITIALIZED ===");
	}

	private async Task InitializeMenuSystem()
	{
		_bootstrapSubProcessMenuSystem = new BootstrapSubProcessMenuSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessSaveLoadSystem,
			_gameController,
			_inputDevice,
			LocalizationManager,
			_canvasBootstrapChooseFirstLanguage,
			_canvasMenuBackground,
			_canvasSavingProcess,
			_canvasPauseMenu,
			_canvasPauseSubMenuSave,
			_canvasPauseSubMenuLoad,
			_canvasPauseSubMenuAppearance,
			_canvasPauseSubMenuTutorial,
			_canvasPauseSubMenuSettings,
			_canvasPauseSubMenuSettingsGameDifficulty,
			_canvasPauseMenuConfirmAction,
			_canvasMainMenuReadNews,
			_canvasMenuWeaponWheel,
			_canvasMenuCutscene,
			_canvasHUDhealthAndMana,
			_canvasHUDweapons,
			_canvasHUDinteraction,
			_canvasHUDmission,
			_canvasMenuNote,
			_canvasMenuLockpickMechanical,
			_canvasMenuLockpickElectronic,
			_canvasMenuDialogue,
			_canvasMainMenuChooseMission,
			_canvasBootstrapSignTermsAndConditions,
			_canvasHUDmonocular);

		await _bootstrapSubProcessMenuSystem.Initialize();

		GD.Print("=== MENU SYSTEM INITIALIZED ===");
	}

	private async Task InitializePlayerSystems()
	{
		_gameObjectPlayer = _playerScene.Instantiate();
		AddChild(_gameObjectPlayer);
		GameObjectPlayerCamera = _playerCameraScene.Instantiate();
		AddChild(GameObjectPlayerCamera);

		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_canvasMenuBackground,
			_gameObjectPlayer,
			GameObjectPlayerCamera);

		await _bootstrapSubProcessPlayerSystems.Initialize();

		GD.Print("=== PLAYER SYSTEMS INITIALIZED ===");
	}

	private async Task InitializeInteractionSystem()
	{
		_bootstrapSubProcessInteractionSystem = new BootstrapSubProcessInteractionSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_gameController,
			_inputDevice,
			LocalizationManager,
			_gameObjectPlayer,
			GameObjectPlayerCamera);

		await _bootstrapSubProcessInteractionSystem.Initialize();

		GD.Print("=== INTERACTION SYSTEM INITIALIZED ===");
	}

	private async Task InitializeWeaponSystem()
	{
		_bootstrapSubProcessWeaponSystem = new BootstrapSubProcessWeaponSystem(
			this,
			_gameController,
			_inputDevice,
			LocalizationManager,
			_gameObjectPlayer,
			GameObjectPlayerCamera,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_bootstrapSubProcessInteractionSystem);

		await _bootstrapSubProcessWeaponSystem.Initialize();

		GD.Print("=== WEAPON SYSTEM INITIALIZED ===");
	}

	private async Task InitializeMissionsSystem()
	{
		_bootstrapSubProcessMissionsSystem = new BootstrapSubProcessMissionsSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessSaveLoadSystem,
			_bootstrapSubProcessMenuSystem,
			GameObjectPlayerCamera);

		await _bootstrapSubProcessMissionsSystem.Initialize();

		GD.Print("=== MISSIONS SYSTEM INITIALIZED ===");
	}

	private async Task InitializeObjectPoolSystem()
	{
		_bootstrapSubProcessObjectPoolSystem = new BootstrapSubProcessObjectPoolSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem);

		await _bootstrapSubProcessObjectPoolSystem.Initialize();

		GD.Print("=== OBJECT POOL SYSTEM INITIALIZED ===");
	}

	private void RegisterBootstrapDependencies()
	{
		ServiceLocator.Register<LocalizationManager>(LocalizationManager);
		ServiceLocator.Register<GameController>(_gameController);
		ServiceLocator.Register<IInputDevice>(_inputDevice);
		ServiceLocator.Register<Key>(_keyCodePauseMenu);
		ServiceLocator.Register<GameScenesList>(GameData.GameScenesList);
		ServiceLocator.Register<GameMissionsList>(GameData.GameMissionsList);
		ServiceLocator.Register<Godot.Collections.Array<Texture2D>>(GameData.NPCdetectionSignFrames);
		ServiceLocator.Register<Bootstrap>(this);
		GD.Print("=== BOOTSTRAP SERVICES REGISTERED ===");
	}

	private void CreateBootstrapTemporaryCamera()
	{
		_gameObjectBootstrapTemporaryCamera = new Node3D { Name = "BootstrapTemporaryCamera" };
		_gameObjectBootstrapTemporaryCamera.AddChild(new Camera3D());
		AddChild(_gameObjectBootstrapTemporaryCamera);
	}

	public void ChangeLanguage(LanguagesEnum newLanguage)
	{
		LocalizationManager.ChangeLanguage(newLanguage);

		// Удаление конкретного сервиса
		ServiceLocator.Remove<LocalizationManager>();

		// Теперь можно регистрировать его заново
		ServiceLocator.Register<LocalizationManager>(LocalizationManager);
	}

	public void ChangeWeaponWheelType(WeaponWheelMenuTypes weaponWheelMenuTypes)
	{
		_bootstrapSubProcessWeaponSystem.ChangeWeaponWheelType(weaponWheelMenuTypes);
	}

	private async Task LoadFirstGameplayScene()
	{
		if (_firstSceneToLoad == GameScenesSystemEnum.Scene_System_MainMenu)
		{
			await _bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene();
		}
		else if (_firstSceneToLoad == GameScenesSystemEnum.Scene_System_EndGameTitles)
		{
			await _bootstrapSubProcessSceneSystem.GameSceneManager.LoadEndGameTitlesScene();
		}
		else
		{
			await _bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene((GameScenesGameplayEnum)((int)_firstSceneToLoad - 2));
		}
	}

	private async Task BootstrapPrerequisites()
	{
		await ChooseInitialLanguage();
		await SignTermsAndConditions();

		_bootstrapPreferences.SetValue("Bootstrap", "PrerequisitesMet", true);
		_bootstrapPreferences.Save(BootstrapPreferencesPath);
	}

	private void LoadBootstrapPreferences()
	{
		Error loadResult = _bootstrapPreferences.Load(BootstrapPreferencesPath);
		if (loadResult != Error.Ok && loadResult != Error.FileNotFound)
		{
			GD.PushWarning($"Could not load bootstrap settings ({loadResult}); default settings will be used.");
		}
	}

	private async Task WaitForDurationOrInput(double durationSeconds)
	{
		if (durationSeconds <= 0)
		{
			return;
		}

		ulong startTime = Time.GetTicksMsec();
		while ((Time.GetTicksMsec() - startTime) / 1000.0 < durationSeconds)
		{
			if (Input.IsAnythingPressed())
			{
				return;
			}

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
	}

	private async Task ChooseInitialLanguage()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;

		_canvasBootstrapChooseFirstLanguage.SetActive(true);

		TaskCompletionSource<bool> languageSelected = new(TaskCreationOptions.RunContinuationsAsynchronously);

		_bootstrapSubProcessMenuSystem.ViewModelMenuChooseFirstLanguage.ButtonRussianLangauge.FindNodeOfType<Godot.Button>().Pressed += () =>
		{
			ChangeLanguage(LanguagesEnum.Russian);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected.TrySetResult(true);
		};

		_bootstrapSubProcessMenuSystem.ViewModelMenuChooseFirstLanguage.ButtonEnglishLanguage.FindNodeOfType<Godot.Button>().Pressed += () =>
		{
			ChangeLanguage(LanguagesEnum.English);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected.TrySetResult(true);
		};

		await languageSelected.Task;

		Input.MouseMode = Input.MouseModeEnum.Captured;

		_canvasBootstrapChooseFirstLanguage.QueueFree();
	}

	private async Task SignTermsAndConditions()
	{
		_canvasBootstrapSignTermsAndConditions.SetActive(true);

		Input.MouseMode = Input.MouseModeEnum.Visible;

		TaskCompletionSource<bool> termsSigned = new(TaskCreationOptions.RunContinuationsAsynchronously);

		var toggleComponent = _bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ToggleAgreeWithTerms.FindNodeOfType<CheckButton>();
		var buttonSignComponent = _bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonSign.FindNodeOfType<Godot.Button>();

		buttonSignComponent.Disabled = true;

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextHeaderTermsAndConditions.FindNodeOfType<Label>().Text = LocalizationManager.GetLocalizedString("UI_Menu_Bootstrap_SignTermsnAndConditions_TextHeaderTermsAndConditions");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextButtonSign.FindNodeOfType<Label>().Text = LocalizationManager.GetLocalizedString("UI_Menu_Bootstrap_SignTermsnAndConditions_ButtonSign");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextButtonRefuse.FindNodeOfType<Label>().Text = LocalizationManager.GetLocalizedString("UI_Menu_Bootstrap_SignTermsnAndConditions_ButtonRefuse");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextToggleAgreeWithTerms.FindNodeOfType<Label>().Text = LocalizationManager.GetLocalizedString("UI_Menu_Bootstrap_SignTermsnAndConditions_ToggleAcceptWithTerms");

		if (LocalizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextTermsAndConditions.FindNodeOfType<Label>().Text = GameData.TermsAndConditions.TermsAndConditions_RU;
		}
		else
		{
			_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextTermsAndConditions.FindNodeOfType<Label>().Text = GameData.TermsAndConditions.TermsAndConditions_EN;
		}

		toggleComponent.ButtonPressed = false;

		toggleComponent.Toggled += (bool isOn) =>
		{
			buttonSignComponent.Disabled = !isOn;
		};

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonSign.FindNodeOfType<Godot.Button>().Pressed += () =>
		{
			if (toggleComponent.ButtonPressed)
			{
				termsSigned.TrySetResult(true);
			}
		};

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonRefuse.FindNodeOfType<Godot.Button>().Pressed += () =>
		{
			GD.Print("EXIT GAME");
			GetTree().Quit();
		};

		await termsSigned.Task;

		Input.MouseMode = Input.MouseModeEnum.Captured;

		_canvasBootstrapSignTermsAndConditions.QueueFree();
	}

	private void ApplyBootstrapPlayerConfigs()
	{
		_bootstrapSubProcessPlayerSystems.PlayerResourcesHealthManager.ConfigApplyPlayerHealth(_playerHealth);
		_bootstrapSubProcessPlayerSystems.PlayerResourcesHealthManager.ConfigApplyPlayerHealingItems(_playerHealingItems);

		_bootstrapSubProcessPlayerSystems.PlayerResourcesManaManager.ConfigApplyPlayerMana(_playerMana);
		_bootstrapSubProcessPlayerSystems.PlayerResourcesManaManager.ConfigApplyPlayerManaReplenishItems(_playerManaReplenishItems);

		_bootstrapSubProcessPlayerSystems.PlayerResourcesMoneyManager.ConfigApplyPlayerMoney(_playerMoney);

		Node[] availableWeapons = _playerWeapons.GetAvailableWeapons();
		if (availableWeapons != null)
		{
			foreach (Node weaponPrefab in availableWeapons)
			{
				_bootstrapSubProcessWeaponSystem.WeaponController.UnlockWeapon(weaponPrefab);
			}
		}

		var startAmmoEntries = _playerAmmo.GetStartAmmoEntries();
		if (startAmmoEntries != null && startAmmoEntries.Length > 0)
		{
			foreach (var ammoEntry in startAmmoEntries)
			{
				_bootstrapSubProcessWeaponSystem.PlayerResourcesAmmoManager.ConfigApplyPlayerAmmo(
					ammoEntry.AmmoType,
					ammoEntry.StartAmount
				);
			}
		}

		if (_firstSceneToLoad != GameScenesSystemEnum.Scene_System_MainMenu && _firstSceneToLoad != GameScenesSystemEnum.Scene_System_EndGameTitles)
		{
			_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerPosition(_playerTransform.PlayerPosition);
			_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerRotationY(_playerTransform.PlayerRotationY);
			_bootstrapSubProcessPlayerSystems.PlayerCameraController.SetCameraRotationY(_playerTransform.PlayerRotationY);
		}
	}

	private void ApplyBootstrapMissionConfigs()
	{
		_bootstrapSubProcessMissionsSystem.MissionsManager.ApplyMissionConfig(_mission, _missionStep);
	}

	public Node FindDeepNode(Node root, string targetName)
	{
		return root.FindDeepNode(targetName)
			?? throw new InvalidOperationException($"Child named '{targetName}' was not found under '{root.Name}'.");
	}

	public override void _ExitTree()
	{
		ServiceLocator.ClearAllServices();
	}
}