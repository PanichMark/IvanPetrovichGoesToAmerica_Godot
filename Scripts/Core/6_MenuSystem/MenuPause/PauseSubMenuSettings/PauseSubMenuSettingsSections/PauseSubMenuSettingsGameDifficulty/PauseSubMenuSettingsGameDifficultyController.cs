using Godot;
public partial class PauseSubMenuSettingsGameDifficultyController : Node
{
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private LocalizationManager _localizationManager;

	private Node _canvasGameDifficulty;

	private GameDifficultiesList _difficultiesList;

	private Node _imageGameDifficulty;
	private TextureRect _imageComponentGameDifficulty;
	private Node _textGameDifficultyHeader;
	private Label _textComponentGameDifficultyHeader;
	private Node _textGameDifficultyDescription;
	private Label _textComponentGameDifficultyDescription;

	private Node _buttonNextGameDifficulty;
	private Node _buttonPreviousGameDifficulty;
	private Node _buttonCloseSettingsGameDifficulty;
	private Label _textComponentButtonCloseSettingsGameDifficulty;

	private Node _difficultyNotAvailavle;
	private Node _textDifficultyNotAvailavle;
	private Label _textComponentDifficultyNotAvailable;

	public bool IsChooseGameDifficultyMenuOpened { get; private set; }
	private int _currentIndex = 0;
	private bool _isInitialized;
	private bool _wasRightArrowPressed;
	private bool _wasLeftArrowPressed;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameDifficultiesList difficultiesList,
		Node canvasGameDifficulty,
		ViewModelPauseSubMenuSettingsGameDifficultyController viewModelPauseSubMenuSettingsGameDifficultyController)
	{
		_localizationManager = localizationManager;	
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasGameDifficulty = canvasGameDifficulty;

		_difficultiesList = difficultiesList;

		_textGameDifficultyHeader = viewModelPauseSubMenuSettingsGameDifficultyController.TextGameDifficultyHeader;
		_textComponentGameDifficultyHeader = _textGameDifficultyHeader.GetNodeOrNull<Label>();
		_textGameDifficultyDescription = viewModelPauseSubMenuSettingsGameDifficultyController.TextGameDifficultyDescription;
		_textComponentGameDifficultyDescription = _textGameDifficultyDescription.GetNodeOrNull<Label>();

		_imageGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ImageGameDifficulty;
		_imageComponentGameDifficulty = _imageGameDifficulty.GetNodeOrNull<TextureRect>();

		Texture2D spriteToShow = _difficultiesList.Notes[_currentIndex].NoteImage;
		_imageComponentGameDifficulty.Texture2D = spriteToShow;
		_imageGameDifficulty .Set("visible", spriteToShow != null);

		_buttonNextGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonNextGameDifficulty;
		_buttonNextGameDifficulty.GetNodeOrNull<Button>().onClick.AddListener(() => NextDifficulty());
		_buttonPreviousGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonPreviousGameDifficulty;
		_buttonPreviousGameDifficulty.GetNodeOrNull<Button>().onClick.AddListener(() => PreviousDifficulty());

		_buttonCloseSettingsGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonCloseSettingsGameDifficulty;
		_buttonCloseSettingsGameDifficulty.GetNodeOrNull<Button>().onClick.AddListener(() => _pauseSubMenuSettingsSectionGeneralController.CloseSubMenuChooseGameDifficulty());
		_textComponentButtonCloseSettingsGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.TextButtonCloseSettingsGameDifficulty.GetNodeOrNull<Label>();

		_difficultyNotAvailavle = viewModelPauseSubMenuSettingsGameDifficultyController.DifficultyNotAvailable;
		_textDifficultyNotAvailavle = viewModelPauseSubMenuSettingsGameDifficultyController.TextDifficultyNotAvailable;
		_textComponentDifficultyNotAvailable = viewModelPauseSubMenuSettingsGameDifficultyController.TextDifficultyNotAvailable.GetNodeOrNull<Label>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseSubMenuSettingsSectionGeneralController.OnOpenSubMenuGameDifficulty += ShowMenuGameDifficulty;
		_pauseSubMenuSettingsSectionGeneralController.OnCloseSubMenuGameDifficulty += HideMenuGameDifficulty;

		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized || !IsChooseGameDifficultyMenuOpened) return;

		bool rightArrowPressed = Input.IsKeyPressed(Key.Right);
		bool leftArrowPressed = Input.IsKeyPressed(Key.Left);

		if (rightArrowPressed && !_wasRightArrowPressed)
		{
			NextDifficulty();
		}

		if (leftArrowPressed && !_wasLeftArrowPressed)
		{
			PreviousDifficulty();
		}

		_wasRightArrowPressed = rightArrowPressed;
		_wasLeftArrowPressed = leftArrowPressed;
	}

	private void ShowMenuGameDifficulty()
	{
		IsChooseGameDifficultyMenuOpened = true;
		_canvasGameDifficulty .Set("visible", true);

		_currentIndex = 1;
		UpdateGameDifficultyUI();
	}

	public void HideMenuGameDifficulty()
	{
		if (IsChooseGameDifficultyMenuOpened)
		{
			IsChooseGameDifficultyMenuOpened = false;
			_canvasGameDifficulty .Set("visible", false);
		}
	}

	private void NextDifficulty()
	{
		_currentIndex = (_currentIndex + 1) % _difficultiesList.Notes.Count;
		UpdateGameDifficultyUI();
	}

	private void PreviousDifficulty()
	{
		_currentIndex = (_currentIndex - 1 + _difficultiesList.Notes.Count) % _difficultiesList.Notes.Count;
		UpdateGameDifficultyUI();
	}

	private void UpdateGameDifficultyUI()
	{
		if (_currentIndex == 1)
		{
			 _difficultyNotAvailavle .Set("visible", false);
			_imageComponentGameDifficulty.color = Color.white;
		}
		else
		{
			_difficultyNotAvailavle .Set("visible", true);
			_imageComponentGameDifficulty.color = Color.grey;
		}

		_textComponentGameDifficultyHeader.text = _localizationManager.GetLocalizedString($"UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty{_currentIndex + 1}");

		InteractionObjectNoteData data = _difficultiesList.Notes[_currentIndex];
		string textToShow = _localizationManager.GetNoteLanguageSuffix(data);
		_textComponentGameDifficultyDescription.text = textToShow;

		Texture2D spriteToShow = data.NoteImage;
		_imageComponentGameDifficulty.Texture2D = spriteToShow;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_textComponentButtonCloseSettingsGameDifficulty.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty_ButtonClose");
		_textComponentDifficultyNotAvailable.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty_THIS-GAME-DIFFICULTY-IS-NOT-AVAILABLE-IN-DEMO");

		UpdateGameDifficultyUI();
	}
}