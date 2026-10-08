using System;
using Godot;

/// <summary>Connects the pause-menu view model to menu state, gameplay events, and submenu actions.</summary>
public partial class PauseMenuController : Node
{
	private const int SaveButtonIndex = 1;
	private const int LoadButtonIndex = 2;
	private const int AppearanceButtonIndex = 3;
	private const int TutorialButtonIndex = 4;
	private const int SettingsButtonIndex = 5;
	private const int ExitToMainMenuButtonIndex = 6;

	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private GameScenesManager _gameSceneManager;
	private MenuManager _menuManager;
	private MenuBackgroundController _menuBackgroundController;
	private Node _canvasPauseMenu;
	private ViewModelPauseMenu _viewModel;
	private Button[] _buttons;
	private Label[] _buttonLabels;
	private bool _initialized;

	public bool IsPauseConfirmMenuOpened { get; private set; }

	public event Action OnOpenSaveSubMenu;
	public event Action OnOpenLoadSubMenu;
	public event Action OnOpenAppearanceSubMenu;
	public event Action OnOpenTutorialSubMenu;
	public event Action OnOpenSettingsSubMenu;
	public event Action OnCloseAnyPauseSubMenu;
	public event Action OnExitToMainMenu;
	public event Action OnOpenConfirmMenu;
	public event Action OnCloseConfirmMenu;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		MenuBackgroundController menuBackgroundController,
		Node canvasPauseMenu,
		ViewModelPauseMenu viewModelPauseMenu)
	{
		if (_initialized)
			UnsubscribeFromEvents();

		_bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
		_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
		_inputDevice = inputDevice ?? throw new ArgumentNullException(nameof(inputDevice));
		_localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
		_gameSceneManager = gameSceneManager ?? throw new ArgumentNullException(nameof(gameSceneManager));
		_menuManager = menuManager ?? throw new ArgumentNullException(nameof(menuManager));
		_menuBackgroundController = menuBackgroundController ?? throw new ArgumentNullException(nameof(menuBackgroundController));
		_canvasPauseMenu = canvasPauseMenu ?? throw new ArgumentNullException(nameof(canvasPauseMenu));
		_viewModel = viewModelPauseMenu ?? throw new ArgumentNullException(nameof(viewModelPauseMenu));

		if (_viewModel.ButtonsPauseMenu == null || _viewModel.ButtonsPauseMenu.Length != 7 ||
			_viewModel.TextButtonsPauseMenu == null || _viewModel.TextButtonsPauseMenu.Length != 7)
		{
			throw new ArgumentException("Pause menu view model must provide seven buttons and seven button labels.", nameof(viewModelPauseMenu));
		}

		_buttons = new Button[_viewModel.ButtonsPauseMenu.Length];
		_buttonLabels = new Label[_viewModel.TextButtonsPauseMenu.Length];
		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i] = _viewModel.ButtonsPauseMenu[i] as Button
				?? throw new InvalidOperationException($"Pause menu button at index {i} is not a Godot Button.");
			_buttonLabels[i] = _viewModel.TextButtonsPauseMenu[i] as Label
				?? throw new InvalidOperationException($"Pause menu button label at index {i} is not a Godot Label.");
		}

		_buttons[0].Pressed += _menuManager.ClosePauseMenu;
		_buttons[SaveButtonIndex].Pressed += OpenSaveSubMenu;
		_buttons[LoadButtonIndex].Pressed += OpenLoadSubMenu;
		_buttons[AppearanceButtonIndex].Pressed += OpenAppearanceSubMenu;
		_buttons[TutorialButtonIndex].Pressed += OpenTutorialSubMenu;
		_buttons[SettingsButtonIndex].Pressed += OpenSettingsSubMenu;
		_buttons[ExitToMainMenuButtonIndex].Pressed += ExitToMainMenu;

		_gameController.OnSaveGameAvailable += EnableSaveButton;
		_gameController.OnSaveGameUnavailable += DisableSaveButton;
		_gameController.OnPlayerLateDeath += PauseMenuOnDeath;
		_gameController.OnPlayerRevive += PauseMenuOnRevive;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += ClosePauseSubMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += ClosePauseSubMenu;
		_gameSceneManager.OnEndLoadingGameplayScene += UpdateMissionGoal;
		_menuManager.OnOpenPauseMenu += ShowPauseMenu;
		_menuManager.OnClosePauseMenu += HidePauseMenu;
		_menuManager.OnCloseConfirmationOnExitToMainMenu += ClosePauseConfirmMenu;

		ChangeLanguage(_localizationManager);
		if (_gameController.IsGameAbleToSave)
			EnableSaveButton();
		else
			DisableSaveButton();
		_initialized = true;
		GD.Print("PauseMenuController initialized.");
	}

	public override void _Process(double delta)
	{
		if (!_initialized || !_bootstrap.IsBootstrapInitialized || !_inputDevice.GetKeyPauseMenu())
			return;

		if (_menuManager.PauseMenuLevel.Count == 2 &&
			!_gameController.IsMainMenuOrEndGameTitlesActive && !IsPauseConfirmMenuOpened)
		{
			ClosePauseSubMenu();
		}
		else if (_menuManager.PauseMenuLevel.Count == 3)
		{
			ClosePauseConfirmMenu();
		}
	}

	public override void _ExitTree()
	{
		if (_initialized)
			UnsubscribeFromEvents();
	}

	public void OpenPauseConfirmMenu()
	{
		if (IsPauseConfirmMenuOpened)
			return;

		IsPauseConfirmMenuOpened = true;
		_menuManager.PushPauseMenuLevel();
		OnOpenConfirmMenu?.Invoke();
	}

	public void ClosePauseConfirmMenu()
	{
		if (_menuManager.PauseMenuLevel.Count > 0 && IsPauseConfirmMenuOpened)
			_menuManager.PopPauseMenuLevel();

		if (!IsPauseConfirmMenuOpened)
			return;

		IsPauseConfirmMenuOpened = false;
		OnCloseConfirmMenu?.Invoke();
	}

	private void EnableSaveButton()
	{
		_buttons[SaveButtonIndex].Disabled = false;
		SetLabel(_buttonLabels[SaveButtonIndex], "UI_Menu_PauseMenu_ButtonSave");
	}

	private void DisableSaveButton()
	{
		_buttons[SaveButtonIndex].Disabled = true;
		SetLabel(_buttonLabels[SaveButtonIndex], "UI_Menu_PauseMenu_ButtonSave_CANNOT-SAVE-GAME-RIGHT-NOW");
	}

	private void PauseMenuOnDeath()
	{
		SetVisible(_viewModel.TextDeathMessage, true);
		SetVisible(_viewModel.ButtonsPauseMenu[0], false);
		SetVisible(_viewModel.ButtonsPauseMenu[SaveButtonIndex], false);
		SetVisible(_viewModel.ButtonsPauseMenu[AppearanceButtonIndex], false);
		SetVisible(_viewModel.ButtonsPauseMenu[TutorialButtonIndex], false);
		SetVisible(_viewModel.PlayerMoney, false);
		SetVisible(_viewModel.CurrentMission, false);
	}

	private void PauseMenuOnRevive()
	{
		SetVisible(_viewModel.TextDeathMessage, false);
		SetVisible(_viewModel.ButtonsPauseMenu[0], true);
		SetVisible(_viewModel.ButtonsPauseMenu[SaveButtonIndex], true);
		SetVisible(_viewModel.ButtonsPauseMenu[AppearanceButtonIndex], true);
		SetVisible(_viewModel.ButtonsPauseMenu[TutorialButtonIndex], true);
		SetVisible(_viewModel.PlayerMoney, true);
		SetVisible(_viewModel.CurrentMission, true);
	}

	public void ClosePauseSubMenu()
	{
		OnCloseAnyPauseSubMenu?.Invoke();
		if (_menuManager.PauseMenuLevel.Count > 0)
			_menuManager.PopPauseMenuLevel();

		if (_gameController.IsMainMenuOrEndGameTitlesActive)
		{
			_menuManager.CloseAnyMenu();
			_menuBackgroundController.HideCanvasMenuBackground();
			return;
		}

		if (_gameController.IsPauseMenuAvailable || _gameController.IsPlayerDead)
			ShowPauseMenu();
	}

	public void ShowPauseMenu() => SetVisible(_canvasPauseMenu, true);
	public void HidePauseMenu() => SetVisible(_canvasPauseMenu, false);

	public void OpenSaveSubMenu() => OpenSubMenu(OnOpenSaveSubMenu, "SaveSubMenu");
	public void OpenLoadSubMenu() => OpenSubMenu(OnOpenLoadSubMenu, "LoadSubMenu");
	public void OpenAppearanceSubMenu() => OpenSubMenu(OnOpenAppearanceSubMenu, "AppearanceSubMenu");
	public void OpenTutorialSubMenu() => OpenSubMenu(OnOpenTutorialSubMenu, "TutorialSubMenu");
	public void OpenSettingsSubMenu() => OpenSubMenu(OnOpenSettingsSubMenu, "SettingsSubMenu");

	public void ExitToMainMenu() => OnExitToMainMenu?.Invoke();

	private void OpenSubMenu(Action openSubMenu, string menuName)
	{
		openSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		HidePauseMenu();
		GD.Print($"{menuName} opened");
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		SetLabel(_buttonLabels[0], "UI_Menu_PauseMenu_ButtonResume");
		SetLabel(_buttonLabels[LoadButtonIndex], "UI_Menu_PauseMenu_ButtonLoad");
		SetLabel(_buttonLabels[AppearanceButtonIndex], "UI_Menu_PauseMenu_ButtonAppearance_NO-APPEARANCE-MENU-IN-DEMO-VERSION");
		SetLabel(_buttonLabels[TutorialButtonIndex], "UI_Menu_PauseMenu_ButtonTutorial");
		SetLabel(_buttonLabels[SettingsButtonIndex], "UI_Menu_PauseMenu_ButtonSettings");
		SetLabel(_buttonLabels[ExitToMainMenuButtonIndex], "UI_Menu_PauseMenu_ButtonExitToMainMenu");
		if (_gameController.IsGameAbleToSave)
			SetLabel(_buttonLabels[SaveButtonIndex], "UI_Menu_PauseMenu_ButtonSave");
		else
			SetLabel(_buttonLabels[SaveButtonIndex], "UI_Menu_PauseMenu_ButtonSave_CANNOT-SAVE-GAME-RIGHT-NOW");
		SetLabel(_viewModel.TextCurrentPlayerMoney, "UI_Menu_PauseMenu_PlayerMoney");
		SetLabel(_viewModel.TextDeathMessage, "UI_Menu_PauseMenu_TextDeathMessage");
		UpdateMissionGoal();
	}

	private void UpdateMissionGoal()
	{
		SetLabel(_viewModel.TextCurrentMissionGoal,
			_gameSceneManager.CurrentScene == GameScenesSystemEnum.Scene_System_Test
				? "UI_Menu_PauseMenu_MissionGoal_THERE-IS-NO-MISSION-GOAL-IN-TEST-SCENE"
				: "UI_Menu_PauseMenu_MissionGoal_Current");
	}

	private void SetLabel(Node node, string localizationKey)
	{
		if (node is Label label)
			label.Text = _localizationManager.GetLocalizedString(localizationKey);
		else if (node is RichTextLabel richTextLabel)
			richTextLabel.Text = _localizationManager.GetLocalizedString(localizationKey);
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}

	private void UnsubscribeFromEvents()
	{
		_buttons[0].Pressed -= _menuManager.ClosePauseMenu;
		_buttons[SaveButtonIndex].Pressed -= OpenSaveSubMenu;
		_buttons[LoadButtonIndex].Pressed -= OpenLoadSubMenu;
		_buttons[AppearanceButtonIndex].Pressed -= OpenAppearanceSubMenu;
		_buttons[TutorialButtonIndex].Pressed -= OpenTutorialSubMenu;
		_buttons[SettingsButtonIndex].Pressed -= OpenSettingsSubMenu;
		_buttons[ExitToMainMenuButtonIndex].Pressed -= ExitToMainMenu;

		_gameController.OnSaveGameAvailable -= EnableSaveButton;
		_gameController.OnSaveGameUnavailable -= DisableSaveButton;
		_gameController.OnPlayerLateDeath -= PauseMenuOnDeath;
		_gameController.OnPlayerRevive -= PauseMenuOnRevive;
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= ClosePauseSubMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene -= ClosePauseSubMenu;
		_gameSceneManager.OnEndLoadingGameplayScene -= UpdateMissionGoal;
		_menuManager.OnOpenPauseMenu -= ShowPauseMenu;
		_menuManager.OnClosePauseMenu -= HidePauseMenu;
		_menuManager.OnCloseConfirmationOnExitToMainMenu -= ClosePauseConfirmMenu;
		_initialized = false;
	}
}