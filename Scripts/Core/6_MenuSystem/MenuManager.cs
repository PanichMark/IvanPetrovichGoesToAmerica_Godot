using System;
using System.Collections.Generic;
using Godot;

/// <summary>Coordinates menu state, gameplay control, cursor state, and scene transitions.</summary>
public partial class MenuManager : Node
{
	public delegate void MenuEventHandler();

	public event MenuEventHandler OnOpenPauseMenu;
	public event MenuEventHandler OnClosePauseMenu;
	public event MenuEventHandler OnOpenWeaponWheelMenu;
	public event MenuEventHandler OnCloseWeaponWheelMenu;
	public event MenuEventHandler OnOpenInteractionHUD;
	public event MenuEventHandler OnCloseInteractionHUD;
	public event MenuEventHandler OnOpenInteractionMenu;
	public event MenuEventHandler OnCloseInteractionMenu;
	public event MenuEventHandler OnOpenDialogueMenu;
	public event MenuEventHandler OnCloseDialogueMenu;
	public event MenuEventHandler OnOpenCutsceneMenu;
	public event MenuEventHandler OnCloseCutsceneMenu;
	public event MenuEventHandler OnClosePauseMenuDuringOpenedDialogueMenu;
	public event MenuEventHandler OnClosePauseMenuDuringOpenedCutsceneMenu;
	public event MenuEventHandler OnOpenAnyMenu;
	public event MenuEventHandler OnCloseAnyMenu;
	public event MenuEventHandler OnOpenMenuBackground;
	public event MenuEventHandler OnCloseMenuBackground;
	public event MenuEventHandler OnOpenConfirmationOnExitToMainMenu;
	public event MenuEventHandler OnCloseConfirmationOnExitToMainMenu;

	public bool IsConfirmationOnExitToMainMenuOpened { get; private set; }
	public bool IsPauseMenuOpened { get; private set; }
	public bool IsWeaponWheelMenuOpened { get; private set; }
	public bool IsAnyMenuOpened { get; private set; }
	public bool IsDialogueMenuOpened { get; private set; }
	public bool IsInteractionHUDOpened { get; private set; }
	public bool IsInteractionMenuOpened { get; private set; }
	public bool IsCutsceneMenuOpened { get; private set; }
	public bool IsMainMenuBeingLoaded { get; private set; }

	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameController _gameController;
	private GameScenesManager _gameSceneManager;
	private readonly Stack<int> _pauseMenuLevel = new();
	private bool _initialized;

	public Stack<int> PauseMenuLevel => _pauseMenuLevel;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		GameScenesManager gameSceneManager)
	{
		_bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
		_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
		_inputDevice = inputDevice ?? throw new ArgumentNullException(nameof(inputDevice));
		_gameSceneManager = gameSceneManager ?? throw new ArgumentNullException(nameof(gameSceneManager));

		if (_initialized)
			UnsubscribeFromGameEvents();

		Input.MouseMode = Input.MouseModeEnum.Captured;
		IsPauseMenuOpened = false;
		IsWeaponWheelMenuOpened = false;
		IsAnyMenuOpened = false;
		_gameController.OnPlayerLateDeath += OpenPauseMenu;

		_gameSceneManager.OnBeginLoadingGameplayScene += ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseCutsceneMenu;

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += CloseCutsceneMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += BeginMainMenuLoading;
		_gameSceneManager.OnEndLoadingMainMenuOrEndGameTitlesScene += EndMainMenuLoading;

		_initialized = true;
		GD.Print("MenuManager initialized.");
	}

	public override void _ExitTree()
	{
		if (_initialized)
			UnsubscribeFromGameEvents();
	}

	public override void _Process(double delta)
	{
		if (!_initialized || !_bootstrap.IsBootstrapInitialized || !_inputDevice.GetKeyPauseMenu() ||
			_gameController.IsMainMenuOrEndGameTitlesActive)
		{
			return;
		}

		if (_pauseMenuLevel.Count == 0)
		{
			OpenPauseMenu();
		}
		else if (_pauseMenuLevel.Count == 1)
		{
			if (!_gameController.IsPlayerDead && !_gameController.IsMainMenuOrEndGameTitlesActive)
				ClosePauseMenu();

			if (IsDialogueMenuOpened)
			{
				OnCloseMenuBackground?.Invoke();
				OnClosePauseMenuDuringOpenedDialogueMenu?.Invoke();
			}
			if (IsCutsceneMenuOpened)
			{
				OnCloseMenuBackground?.Invoke();
				OnClosePauseMenuDuringOpenedCutsceneMenu?.Invoke();
			}
		}
		else if (_pauseMenuLevel.Count == 2 && IsConfirmationOnExitToMainMenuOpened)
		{
			CloseConfirmationOnExitToMainMenu();
		}
	}

	public void PushPauseMenuLevel()
	{
		_pauseMenuLevel.Push(1);
		GD.Print($"PauseStack is: {_pauseMenuLevel.Count}");
	}

	public void PopPauseMenuLevel()
	{
		if (_pauseMenuLevel.Count == 0)
		{
			GD.PushWarning("Cannot pop a pause-menu level: the stack is empty.");
			return;
		}

		_pauseMenuLevel.Pop();
		GD.Print($"PauseStack is: {_pauseMenuLevel.Count}");
	}

	public void OpenConfirmationOnExitToMainMenu()
	{
		IsConfirmationOnExitToMainMenuOpened = true;
		OnOpenConfirmationOnExitToMainMenu?.Invoke();
	}

	public void CloseConfirmationOnExitToMainMenu()
	{
		IsConfirmationOnExitToMainMenuOpened = false;
		OnCloseConfirmationOnExitToMainMenu?.Invoke();
	}

	public void OpenPauseMenu()
	{
		if (IsPauseMenuOpened)
			return;

		if (IsWeaponWheelMenuOpened)
			CloseWeaponWheelMenu();

		OnOpenMenuBackground?.Invoke();
		PushPauseMenuLevel();
		OnOpenPauseMenu?.Invoke();
		IsPauseMenuOpened = true;
		OpenAnyMenu();
		_gameController.MakePlayerNonControllable();
		Engine.TimeScale = 0f;
		GD.Print("PauseMenu opened");
	}

	public void ClosePauseMenu()
	{
		if (!IsPauseMenuOpened)
			return;

		OnClosePauseMenu?.Invoke();
		IsPauseMenuOpened = false;
		if (_pauseMenuLevel.Count > 0)
			PopPauseMenuLevel();

		if (!IsInteractionMenuOpened && !IsDialogueMenuOpened && !IsCutsceneMenuOpened)
		{
			OnCloseMenuBackground?.Invoke();
			CloseAnyMenu();
			_gameController.MakePlayerControllable();
			Engine.TimeScale = 1f;
		}

		if (!IsPauseMenuOpened && IsCutsceneMenuOpened)
			Input.MouseMode = Input.MouseModeEnum.Captured;

		GD.Print("PauseMenu closed");
	}

	public void OpenCutsceneMenu()
	{
		if (IsCutsceneMenuOpened)
			return;

		IsCutsceneMenuOpened = true;
		OpenAnyMenu();
		OnOpenCutsceneMenu?.Invoke();
		GD.Print("CutsceneMenu opened");
	}

	public void CloseCutsceneMenu()
	{
		if (!IsCutsceneMenuOpened)
			return;

		CloseAnyMenu();
		OnCloseCutsceneMenu?.Invoke();
		IsCutsceneMenuOpened = false;
		GD.Print("CutsceneMenu closed");
	}

	public void OpenWeaponWheelMenu()
	{
		if (IsWeaponWheelMenuOpened)
			return;

		OpenAnyMenu();
		IsWeaponWheelMenuOpened = true;
		OnOpenWeaponWheelMenu?.Invoke();
		Engine.TimeScale = 0.2f;
		GD.Print("WeaponWheelMenu opened");
	}

	public void CloseWeaponWheelMenu()
	{
		if (!IsWeaponWheelMenuOpened)
			return;

		CloseAnyMenu();
		IsWeaponWheelMenuOpened = false;
		OnCloseWeaponWheelMenu?.Invoke();
		Engine.TimeScale = 1f;
		GD.Print("WeaponWheelMenu closed");
	}

	public void OpenAnyMenu()
	{
		IsAnyMenuOpened = true;
		if (!IsDialogueMenuOpened && !IsCutsceneMenuOpened)
			OnOpenAnyMenu?.Invoke();

		if (!_gameController.IsMainMenuOrEndGameTitlesActive)
		{
			CloseInteractionHUD();
			if (!IsCutsceneMenuOpened)
				Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public void CloseAnyMenu()
	{
		IsAnyMenuOpened = false;
		OnCloseAnyMenu?.Invoke();
		if (_gameController.IsMainMenuOrEndGameTitlesActive)
			return;

		if (!IsMainMenuBeingLoaded)
			OpenInteractionHUD();

		if (!IsCutsceneMenuOpened)
			Input.MouseMode = Input.MouseModeEnum.Captured;
		if (IsDialogueMenuOpened)
			Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public void OpenInteractionHUD()
	{
		IsInteractionHUDOpened = true;
		OnOpenInteractionHUD?.Invoke();
		GD.Print("InteractionHUD opened");
	}

	public void CloseInteractionHUD()
	{
		OnCloseInteractionHUD?.Invoke();
		IsInteractionHUDOpened = false;
		GD.Print("InteractionHUD closed");
	}

	public void OpenInteractionMenu()
	{
		if (IsInteractionMenuOpened)
			return;

		IsInteractionMenuOpened = true;
		Engine.TimeScale = 0f;
		_gameController.MakePlayerNonControllable();
		OnOpenMenuBackground?.Invoke();
		OpenAnyMenu();
		OnOpenInteractionMenu?.Invoke();
		GD.Print("InteractionMenu opened");
	}

	public void CloseInteractionMenu()
	{
		if (!IsInteractionMenuOpened)
			return;

		IsInteractionMenuOpened = false;
		Engine.TimeScale = 1f;
		OnCloseInteractionMenu?.Invoke();
		_gameController.MakePlayerControllable();
		OnCloseMenuBackground?.Invoke();
		CloseAnyMenu();
		GD.Print("InteractionMenu closed");
	}

	public void OpenDialogueMenu()
	{
		if (IsDialogueMenuOpened)
			return;

		IsDialogueMenuOpened = true;
		Engine.TimeScale = 0f;
		_gameController.MakePlayerNonControllable();
		OpenAnyMenu();
		OnOpenDialogueMenu?.Invoke();
		GD.Print("DialogueMenu opened");
	}

	public void CloseDialogueMenu()
	{
		if (!IsDialogueMenuOpened)
			return;

		IsDialogueMenuOpened = false;
		Engine.TimeScale = 1f;
		OnCloseDialogueMenu?.Invoke();
		_gameController.MakePlayerControllable();
		CloseAnyMenu();
		GD.Print("DialogueMenu closed");
	}

	private void BeginMainMenuLoading() => IsMainMenuBeingLoaded = true;
	private void EndMainMenuLoading() => IsMainMenuBeingLoaded = false;

	private void UnsubscribeFromGameEvents()
	{
		_gameController.OnPlayerLateDeath -= OpenPauseMenu;

		_gameSceneManager.OnBeginLoadingGameplayScene -= ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene -= CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene -= CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingGameplayScene -= CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene -= CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene -= CloseCutsceneMenu;

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= ClosePauseMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CloseWeaponWheelMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CloseInteractionHUD;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CloseInteractionMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CloseDialogueMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= CloseCutsceneMenu;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= BeginMainMenuLoading;
		_gameSceneManager.OnEndLoadingMainMenuOrEndGameTitlesScene -= EndMainMenuLoading;
		_initialized = false;
	}
}