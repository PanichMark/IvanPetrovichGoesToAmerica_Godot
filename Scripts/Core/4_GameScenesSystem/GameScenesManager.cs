using System;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

/// <summary>Coordinates scene changes and the loading screen while keeping bootstrap systems alive.</summary>
public partial class GameScenesManager : Node
{
	public bool IsWaitingForNewGameplayDataToLoad { get; private set; }
	public bool IsWaitingForOldGameplayDataToSave { get; private set; }
	public bool HasLoadedGameplayScene { get; private set; }
	public bool WasInitialSceneLoaded { get; private set; }
	public GameScenesSystemEnum PreviousScene { get; private set; }
	public GameScenesSystemEnum? CurrentScene { get; private set; }

	public event Action OnBeginLoadingMainMenuScene;
	public event Action OnBeginLoadingMainMenuOrEndGameTitlesScene;
	public event Action OnEndLoadingMainMenuOrEndGameTitlesScene;
	public event Action OnBeginLoadingGameplayScene;
	public event Action OnEndLoadingGameplayScene;
	public event Action OnRequestSaveOldGameplayData;

	private GameController _gameController;
	private LocalizationManager _localizationManager;
	private GameScenesList _gameScenesList;
	private Node _canvasLoadingScreen;
	private ViewModelSceneLoadingScreen _viewModel;
	private Node _loadedSceneNode;
	private bool _wasPreviouslyCopiedToTEMP;
	private bool _initialized;

	public override void _EnterTree()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameScenesList gameScenesList,
		Node canvasLoadingScreen,
		ViewModelSceneLoadingScreen viewModelSceneLoadingScreen)
	{
		_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
		_localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
		_gameScenesList = gameScenesList ?? throw new ArgumentNullException(nameof(gameScenesList));
		_canvasLoadingScreen = canvasLoadingScreen ?? throw new ArgumentNullException(nameof(canvasLoadingScreen));
		_viewModel = viewModelSceneLoadingScreen ?? throw new ArgumentNullException(nameof(viewModelSceneLoadingScreen));

		_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_initialized = true;
		GD.Print("GameScenesManager initialized.");
	}

	public async Task LoadGameplayScene(GameScenesGameplayEnum scene)
	{
		if (!TryGetSceneData((GameScenesSystemEnum)scene, out GameSceneData sceneData))
		{
			return;
		}

		EnsureInitialized();
		await LoadScene(sceneData, isGameplay: true);
	}

	public async Task LoadMainMenuScene()
	{
		if (!TryGetSceneData(GameScenesSystemEnum.Scene_System_MainMenu, out GameSceneData sceneData))
		{
			return;
		}

		EnsureInitialized();
		OnBeginLoadingMainMenuScene?.Invoke();
		await LoadScene(sceneData, isGameplay: false);
	}

	public async Task LoadEndGameTitlesScene()
	{
		if (!TryGetSceneData(GameScenesSystemEnum.Scene_System_EndGameTitles, out GameSceneData sceneData))
		{
			return;
		}

		EnsureInitialized();
		await LoadScene(sceneData, isGameplay: false);
	}

	private async Task LoadScene(GameSceneData sceneData, bool isGameplay)
	{
		EnsureInitialized();
		GameScenesSystemEnum nextScene = sceneData.GameScene;
		Engine.TimeScale = 0f;
		if (isGameplay)
		{
			HasLoadedGameplayScene = false;
			_gameController.GameplaySceneLoadBegan();
			OnBeginLoadingGameplayScene?.Invoke();
		}
		else
		{
			_gameController.MainMenuOrEndGameTitlesSceneLoadBegan();
			OnBeginLoadingMainMenuOrEndGameTitlesScene?.Invoke();
		}

		SetVisible(_canvasLoadingScreen, true);
		SetGameplayLoadingUi(sceneData, isGameplay);
		Input.MouseMode = isGameplay || nextScene == GameScenesSystemEnum.Scene_System_EndGameTitles
			? Input.MouseModeEnum.Captured
			: Input.MouseModeEnum.Visible;

		if (CurrentScene.HasValue && _loadedSceneNode != null && IsInstanceValid(_loadedSceneNode))
		{
			PreviousScene = CurrentScene.Value;
			if (isGameplay && WasInitialSceneLoaded && !_wasPreviouslyCopiedToTEMP)
			{
				if (OnRequestSaveOldGameplayData != null)
				{
					IsWaitingForOldGameplayDataToSave = true;
					OnRequestSaveOldGameplayData.Invoke();
					await ToSignalWhen(() => !IsWaitingForOldGameplayDataToSave);
				}
			}
			_wasPreviouslyCopiedToTEMP = false;

			SetLoadingSliderValue(isGameplay ? 0.25f : 0f);
			Node oldScene = _loadedSceneNode;
			oldScene.QueueFree();
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			if (isGameplay)
			{
				SetLoadingSliderValue(0.25f);
			}
		}

		PackedScene packedScene = sceneData.SceneResource;
		Node newScene = packedScene.Instantiate();
		newScene.Name = nextScene.ToString();
		GetTree().Root.AddChild(newScene);
		_loadedSceneNode = newScene;
		CurrentScene = nextScene;
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		if (isGameplay)
		{
			SetLoadingSliderValue(0.5f);
			HasLoadedGameplayScene = true;
			IsWaitingForNewGameplayDataToLoad = OnEndLoadingGameplayScene != null;
			OnEndLoadingGameplayScene?.Invoke();
			_gameController.BlockInput();
			if (IsWaitingForNewGameplayDataToLoad)
			{
				await ToSignalWhen(() => !IsWaitingForNewGameplayDataToLoad);
			}

			SetLoadingSliderValue(1f);
			SetVisible(_viewModel.SliderSceneLoadingStatus, false);
			SetVisible(_viewModel.TextLoadingIsReady, true);
			SetLabelText(_viewModel.TextLoadingIsReady, _localizationManager.GetLocalizedString("UI_LoadingScreen_LoadingIsReady"));
			await WaitForAnyKeyPress();

			_gameController.GameplaySceneLoadEnded();
			_gameController.UnblockInput();
			WasInitialSceneLoaded = true;
		}
		else
		{
			OnEndLoadingMainMenuOrEndGameTitlesScene?.Invoke();
			_gameController.MainMenuOrEndGameTitlesSceneLoadEnded();
			WasInitialSceneLoaded = true;
		}

		Engine.TimeScale = 1f;
		SetVisible(_canvasLoadingScreen, false);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
	}

	private void SetGameplayLoadingUi(GameSceneData sceneData, bool isGameplay)
	{
		SetTexture(_viewModel.ImageScene, sceneData.SceneLoadingScreenImage);
		SetLabelText(_viewModel.TextSceneName, _localizationManager.GetLocalizedString(sceneData.GameScene.ToString()));
		SetLabelText(_viewModel.TextMissionName, _localizationManager.GetLocalizedString(GetMissionName(sceneData)));
		SetLabelText(_viewModel.TextSceneDescription, _localizationManager.CurrentLanguage == LanguagesEnum.Russian
			? sceneData.SceneDescription_RU
			: sceneData.SceneDescription_EN);

		SetVisible(_viewModel.SliderSceneLoadingStatus, isGameplay);
		SetVisible(_viewModel.TextSceneName, isGameplay);
		SetVisible(_viewModel.TextSceneDescription, isGameplay);
		SetVisible(_viewModel.TextMissionName, isGameplay);
		SetVisible(_viewModel.TextLoadingIsReady, false);
		SetLoadingSliderValue(0f);
	}

	private string GetMissionName(GameSceneData sceneData)
	{
		if (sceneData.SceneGameMission == null)
		{
			return sceneData.GameScene == GameScenesSystemEnum.Scene_System_Test ? "Mission_Test" : string.Empty;
		}

		PropertyInfo missionNameProperty = sceneData.SceneGameMission.GetType().GetProperty("MissionName");
		return missionNameProperty?.GetValue(sceneData.SceneGameMission)?.ToString() ?? sceneData.GameScene.ToString();
	}

	private bool TryGetSceneData(GameScenesSystemEnum scene, out GameSceneData sceneData)
	{
		sceneData = null;
		if (_gameScenesList?.GameScenes == null)
		{
			GD.PushError("Cannot transition scenes: GameScenesList is not configured.");
			return false;
		}

		foreach (GameSceneData candidate in _gameScenesList.GameScenes)
		{
			if (candidate?.GameScene == scene)
			{
				sceneData = candidate;
				if (candidate.SceneResource == null)
				{
					GD.PushError($"Cannot transition to '{scene}': assign its SceneResource PackedScene in GameSceneData.");
					return false;
				}

				return true;
			}
		}

		GD.PushError($"Cannot transition to '{scene}': no matching entry exists in GameScenesList.");
		return false;
	}

	private void EnsureInitialized()
	{
		if (!_initialized)
		{
			throw new InvalidOperationException("GameScenesManager.Initialize must be called before requesting a scene transition.");
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
	}

	private async Task ToSignalWhen(Func<bool> condition)
	{
		while (!condition())
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
	}

	private async Task WaitForAnyKeyPress()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		while (!Input.IsAnythingPressed())
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (node is CanvasItem canvasItem && GodotObject.IsInstanceValid(canvasItem))
		{
			canvasItem.Visible = visible;
		}
	}

	private static void SetLabelText(Node node, string text)
	{
		if (node is Label label && GodotObject.IsInstanceValid(label))
		{
			label.Text = text ?? string.Empty;
		}
		else if (node is RichTextLabel richTextLabel && GodotObject.IsInstanceValid(richTextLabel))
		{
			richTextLabel.Text = text ?? string.Empty;
		}
	}

	private static void SetTexture(Node node, Texture2D texture)
	{
		if (node is TextureRect textureRect && GodotObject.IsInstanceValid(textureRect))
		{
			textureRect.Texture = texture;
		}
		else if (node is Sprite2D sprite && GodotObject.IsInstanceValid(sprite))
		{
			sprite.Texture = texture;
		}
	}

	public void ApplyGameplayDataLoadingFinished()
	{
		IsWaitingForNewGameplayDataToLoad = false;
	}

	public void SavedOldGameplayData()
	{
		IsWaitingForOldGameplayDataToSave = false;
	}

	public void SkipWaitingDueToTEMPcopied()
	{
		_wasPreviouslyCopiedToTEMP = true;
	}

	public void SetLoadingSliderValue(float value)
	{
		if (_viewModel?.SliderSceneLoadingStatus is Range loadingProgress && GodotObject.IsInstanceValid(loadingProgress))
		{
			float normalizedValue = Mathf.Clamp(value, 0f, 1f);
			loadingProgress.Value = loadingProgress.MinValue + normalizedValue * (loadingProgress.MaxValue - loadingProgress.MinValue);
		}
	}
}