using Godot;
using System;
public partial class PauseSubMenuLoadController : Node
{
	private Bootstrap _bootstrap;
	public event Action<int> OnRequestLoadSaveFileConfirmation;

	private LocalizationManager _localizationManager;
	private JsonSaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private GameScenesList _gameScenesList;
	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;

	private Node _canvasPauseSubMenuLoad;

	private Label _textComponentPauseSubMenuLoad;

	private Node[] _buttonsLoadGameFile;
	private Button[] _buttonsComponentsLoadGameFile;
	private Label[] _textComponentsGameFileDateAndTime;
	private Label[] _textComponentsGameFileMissionName;
	private Label[] _textComponentsGameFileSceneName;
	private TextureRect[] _imagesComponentsSceneGameFile;

	private Node _buttonClosePauseSubMenuLoad;
	private Button _buttonComponentClosePauseSubMenuLoad;
	private Label _textButtonComponentClosePauseSubMenuLoad;

	private Node _scrollbar;
	private VScrollBar _scrollbarComponent;
	private float _scrollbarHandleSize = 0.11f;
	private Node _scrollbarHandle;

	private bool _isPauseSubMenuLoadOpened;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		JsonSaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameScenesList gameScenesList,
		Node canvasPauseSubMenuLoad,
		ViewModelPauseSubMenuLoad viewModelPauseSubMenuLoad)
	{
		_bootstrap = bootstrap;
		_gameScenesList	= gameScenesList;
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_viewModelPauseSubMenuLoad = viewModelPauseSubMenuLoad;

		_textComponentPauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextPauseSubMenuLoad.GetNodeOrNull<Label>();

		_buttonsLoadGameFile = _viewModelPauseSubMenuLoad.ButtonsLoadGameFile;
		_buttonsComponentsLoadGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];

		_textComponentsGameFileDateAndTime = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileMissionName = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileSceneName = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imagesComponentsSceneGameFile = new TextureRect[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			int slot = i + 1;

			_buttonsComponentsLoadGameFile[i] = _buttonsLoadGameFile[i].GetNodeOrNull<Button>();
			_buttonsComponentsLoadGameFile[i].onClick.AddListener(() => OnRequestLoadSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuLoad.TextGameFileDateAndTime[i].GetNodeOrNull<Label>();
			_textComponentsGameFileMissionName[i] = _viewModelPauseSubMenuLoad.TextGameFileMissionName[i].GetNodeOrNull<Label>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuLoad.TextGameFileSceneName[i].GetNodeOrNull<Label>();
			_imagesComponentsSceneGameFile[i] = _viewModelPauseSubMenuLoad.ImageSceneGameFile[i].GetNodeOrNull<TextureRect>();
		}

		_buttonClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.ButtonClosePauseSubMenuLoad;
		_buttonComponentClosePauseSubMenuLoad = _buttonClosePauseSubMenuLoad.GetNodeOrNull<Button>();
		_buttonComponentClosePauseSubMenuLoad.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuLoad = _viewModelPauseSubMenuLoad.TextButtonClosePauseSubMenuLoad.GetNodeOrNull<Label>();

		_scrollbar = _viewModelPauseSubMenuLoad.VScrollBar;
		_scrollbarComponent = _viewModelPauseSubMenuLoad.VScrollBar.GetNodeOrNull<VScrollBar>();
		Canvas.willRenderCanvases += EnforceFixedHandleSize;
		_scrollbarHandle = _viewModelPauseSubMenuLoad.ScrollbarHandle;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileSaved += RefreshButtonLabelsAndVisibility; 
		_saveLoadController.OnSafeFileDelete += RefreshButtonLabelsAndVisibility;

		_pauseMenuController.OnOpenLoadSubMenu += ShowLoadSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideLoadSubMenuCanvas;

		GD.Print("PauseSubMenuLoadController");
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
	}

	public void ShowLoadSubMenuCanvas()
	{
		_isPauseSubMenuLoadOpened = true;
		_canvasPauseSubMenuLoad .Set("visible", true);

		RefreshButtonLabelsAndVisibility();
	}

	public void HideLoadSubMenuCanvas()
	{
		if (_isPauseSubMenuLoadOpened)
		{
			_isPauseSubMenuLoadOpened = false;
			_canvasPauseSubMenuLoad .Set("visible", false);
			GD.Print("New Load SubMenu closed");
		}
	}

	public void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		int activeLoadButtonsCount = 0;

		for (int safeFileIndex = 0; safeFileIndex < extendedSaveInfos.Length; safeFileIndex++)
		{
			string currentDateAndTime = extendedSaveInfos[safeFileIndex].SavefileDateAndTime;
			GameScenesGameplayEnum currentSceneNameSystem = extendedSaveInfos[safeFileIndex].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentDateAndTime))
			{
				_buttonsLoadGameFile[safeFileIndex] .Set("visible", true);

				activeLoadButtonsCount++;

				_textComponentsGameFileDateAndTime[safeFileIndex].text = currentDateAndTime;
				_textComponentsGameFileSceneName[safeFileIndex].text = _localizationManager.GetLocalizedString(currentSceneNameSystem.ToString());

				if (_gameScenesList.GameScenes[(int)currentSceneNameSystem].SceneGameMission != null)
				{
					_textComponentsGameFileMissionName[safeFileIndex].text = _localizationManager.GetLocalizedString(_gameScenesList.GameScenes[(int)currentSceneNameSystem].SceneGameMission.MissionName.ToString());
				}
				else
				{
					_textComponentsGameFileMissionName[safeFileIndex].text = _localizationManager.GetLocalizedString(GameMissionsNamesEnum.Mission_Test.ToString());
				}

				_imagesComponentsSceneGameFile[safeFileIndex].Texture2D = _gameScenesList.GameScenes[(int)currentSceneNameSystem + 2].SceneLoadingScreenImage;
			}
			else
			{
				_buttonsLoadGameFile[safeFileIndex] .Set("visible", false);
			}
		}

		_scrollbarHandle.Node .Set("visible", activeLoadButtonsCount > 6);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentPauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Load_TextPauseSubMenuLoad");

		_textButtonComponentClosePauseSubMenuLoad.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Load_ButtonClosePauseSubMenuLoad");
	}
}