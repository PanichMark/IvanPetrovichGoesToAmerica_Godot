using Godot;
using System;
public partial class PauseSubMenuSaveController : Node
{
	public event Action<int> OnRequestRewriteSaveFileConfirmation;
	public event Action<int> OnRequestNewSaveFileConfirmation;
	public event Action<int> OnRequestDeleteSaveFileConfirmation;

	private Bootstrap _bootstrap;
	private GameScenesList _gameScenesList;
	private LocalizationManager _localizationManager;
	private JsonSaveLoadController _saveLoadController;
	private PauseMenuController _pauseMenuController;

	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;

	private Node _canvasPauseSubMenuSave;

	private Label _textComponentPauseSubMenuSave;

	private Node _buttonBoxCreateNewGameFile;
	private Node _buttonCreateNewGameFile;
	private Button _buttonComponentCreateNewGameFile;
	private Label _textButtonComponentCreateNewGameFile;

	private Node[] _containersSaveGameFile;

	private Node[] _buttonsRewriteGameFile;
	private Button[] _buttonsComponentsRewriteGameFile;
	private Node[] _buttonsDeleteGameFile;
	private Button[] _buttonsComponentsDeleteGameFile;
	private Label[] _textButtonsComponentsDeleteGameFile;

	private Label[] _textComponentsGameFileDateAndTime;
	private Label[] _textComponentsGameFileMissionName;
	private Label[] _textComponentsGameFileSceneName;
	private TextureRect[] _imagesComponentsSceneGameFile;

	private Node _buttonClosePauseSubMenuSave;
	private Button _buttonComponentClosePauseSubMenuSave;
	private Label _textButtonComponentClosePauseSubMenuSave;

	private Node _scrollbar;
	private VScrollBar _scrollbarComponent;
	private float _scrollbarHandleSize = 0.11f;
	private Node _scrollbarHandle;

	private bool _isPauseSubMenuSaveOpened;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		JsonSaveLoadController saveLoadController,
		PauseMenuController pauseMenuController,
		GameScenesList gameScenesList,
		Node canvasPauseSubMenuSave,
		ViewModelPauseSubMenuSave viewModelPauseSubMenuSave)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_saveLoadController = saveLoadController;
		_pauseMenuController = pauseMenuController;
		_gameScenesList = gameScenesList;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_viewModelPauseSubMenuSave = viewModelPauseSubMenuSave;

		_textComponentPauseSubMenuSave = _viewModelPauseSubMenuSave.TextPauseSubMenuSave.GetNodeOrNull<Label>();

		_buttonBoxCreateNewGameFile = _viewModelPauseSubMenuSave.ButtonBoxCreateNewGameFile;
		_buttonCreateNewGameFile = _viewModelPauseSubMenuSave.ButtonCreateNewGameFile;
		_buttonComponentCreateNewGameFile = _buttonCreateNewGameFile.GetNodeOrNull<Button>();
		_buttonComponentCreateNewGameFile.onClick.AddListener(() =>
		{
			int slotToUse = FindFirstEmptySlot();
			if (slotToUse != -1)
			{
				OnRequestNewSaveFileConfirmation?.Invoke(slotToUse);
			}
		});
		_textButtonComponentCreateNewGameFile = _viewModelPauseSubMenuSave.TextButtonCreateNewGameFile.GetNodeOrNull<Label>();

		_containersSaveGameFile = new Node[_bootstrap.GameData.NumberOfSafeFileSlots];
		_containersSaveGameFile = _viewModelPauseSubMenuSave.ContainersSaveGameFile;

		_buttonsRewriteGameFile = new Node[_bootstrap.GameData.NumberOfSafeFileSlots];
		_buttonsRewriteGameFile = _viewModelPauseSubMenuSave.ButtonsRewriteGameFile;
		_buttonsComponentsRewriteGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];

		_textComponentsGameFileDateAndTime = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileMissionName = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textComponentsGameFileSceneName = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];
		_imagesComponentsSceneGameFile = new TextureRect[_bootstrap.GameData.NumberOfSafeFileSlots];

		_buttonsDeleteGameFile = new Node[_bootstrap.GameData.NumberOfSafeFileSlots];
		_buttonsDeleteGameFile = _viewModelPauseSubMenuSave.ButtonsDeleteGameFile;
		_buttonsComponentsDeleteGameFile = new Button[_bootstrap.GameData.NumberOfSafeFileSlots];
		_textButtonsComponentsDeleteGameFile = new Label[_bootstrap.GameData.NumberOfSafeFileSlots];

		for (int i = 0; i < _bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			int slot = i + 1;

			_buttonsComponentsRewriteGameFile[i] = _buttonsRewriteGameFile[i].GetNodeOrNull<Button>();
			_buttonsComponentsRewriteGameFile[i].onClick.AddListener(() => OnRequestRewriteSaveFileConfirmation?.Invoke(slot));

			_textComponentsGameFileDateAndTime[i] = _viewModelPauseSubMenuSave.TextGameFileDateAndTime[i].GetNodeOrNull<Label>();
			_textComponentsGameFileMissionName[i] = _viewModelPauseSubMenuSave.TextGameFileMissionName[i].GetNodeOrNull<Label>();
			_textComponentsGameFileSceneName[i] = _viewModelPauseSubMenuSave.TextGameFileSceneName[i].GetNodeOrNull<Label>();
			_imagesComponentsSceneGameFile[i] = _viewModelPauseSubMenuSave.ImageSceneGameFile[i].GetNodeOrNull<TextureRect>();

			_buttonsComponentsDeleteGameFile[i] = _buttonsDeleteGameFile[i].GetNodeOrNull<Button>();
			_buttonsComponentsDeleteGameFile[i].onClick.AddListener(() => OnRequestDeleteSaveFileConfirmation?.Invoke(slot));
			_textButtonsComponentsDeleteGameFile[i] = _viewModelPauseSubMenuSave.TextButtonsDeleteGameFile[i].GetNodeOrNull<Label>();
		}

		_buttonClosePauseSubMenuSave = _viewModelPauseSubMenuSave.ButtonClosePauseSubMenuSave;
		_buttonComponentClosePauseSubMenuSave = _buttonClosePauseSubMenuSave.GetNodeOrNull<Button>();
		_buttonComponentClosePauseSubMenuSave.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuSave = _viewModelPauseSubMenuSave.TextButtonClosePauseSubMenuSave.GetNodeOrNull<Label>();

		_scrollbar = _viewModelPauseSubMenuSave.VScrollBar;
		_scrollbarComponent = _viewModelPauseSubMenuSave.VScrollBar.GetNodeOrNull<VScrollBar>();
		Canvas.willRenderCanvases += EnforceFixedHandleSize;
		_scrollbarHandle = _viewModelPauseSubMenuSave.ScrollbarHandle;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_saveLoadController.OnSafeFileDelete += UpdateAllUIElements;
		_saveLoadController.OnSafeFileSaved += UpdateAllUIElements;

		_pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideSaveSubMenuCanvas;

		GD.Print("PauseSubMenuSaveController Initialized");
	}

	private void EnforceFixedHandleSize()
	{
		_scrollbarComponent.size = Mathf.Clamp(_scrollbarHandleSize, 0f, 1f);
	}

	private int FindFirstEmptySlot()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		for (int i = 0; i < extendedSaveInfos.Length; i++)
		{
			if (string.IsNullOrEmpty(extendedSaveInfos[i].SavefileDateAndTime)) 
				return i + 1;
		}
		return -1;
	}

	public void UpdateAllUIElements()
	{
		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;

		if (_buttonBoxCreateNewGameFile.activeSelf != hasEmptySlot)
		{
			_buttonBoxCreateNewGameFile .Set("visible", hasEmptySlot);
		}
	}

	private void RefreshButtonLabelsAndVisibility()
	{
		var extendedSaveInfos = _saveLoadController.GetExtendedSaveInfo();

		int activeSaveButtonsCount = 0;

		for (int safeFileIndex = 0; safeFileIndex < extendedSaveInfos.Length; safeFileIndex++)
		{
			string currentDateAndTime = extendedSaveInfos[safeFileIndex].SavefileDateAndTime;
			GameScenesGameplayEnum currentSceneNameSystem = extendedSaveInfos[safeFileIndex].SafefileSceneNameSystem;

			if (!string.IsNullOrEmpty(currentDateAndTime))
			{
				_containersSaveGameFile[safeFileIndex] .Set("visible", true);

				activeSaveButtonsCount++;

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
				_containersSaveGameFile[safeFileIndex] .Set("visible", false);
			}
		}

		_scrollbarHandle.Node .Set("visible", activeSaveButtonsCount > 5);
	}

	private void ShowSaveSubMenuCanvas()
	{
		_isPauseSubMenuSaveOpened = true;
		_canvasPauseSubMenuSave .Set("visible", true);

		RefreshButtonLabelsAndVisibility();

		bool hasEmptySlot = FindFirstEmptySlot() != -1;
		_buttonBoxCreateNewGameFile .Set("visible", hasEmptySlot);
	}

	private void HideSaveSubMenuCanvas()
	{
		if (_isPauseSubMenuSaveOpened)
		{
			_isPauseSubMenuSaveOpened = false;
			_canvasPauseSubMenuSave .Set("visible", false);
			GD.Print("SaveSubMenu closed");
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentPauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_TextPauseSubMenuSave");

		_textButtonComponentCreateNewGameFile.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonCreateNewGameFile");

		for (int i = 0; i < _viewModelPauseSubMenuSave.ButtonsRewriteGameFile.Length; i++)
		{
			_textButtonsComponentsDeleteGameFile[i].text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonDeleteGameFile");
		}

		_textButtonComponentClosePauseSubMenuSave.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Save_ButtonClosePauseSubMenuSave");
	}
}