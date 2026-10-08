using Godot;
public partial class PauseSubMenuTutorialController : Node
{
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;
	private IInputDevice _inputDevice;
	private Node _canvasPauseSubMenuTutorial;

	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;

	private GameTutorialsList _tutorialsList;

	private Node _imageTutorial;
	private TextureRect _imageComponentTutorial;
	private Node _textTutorial;
	private Label _textComponentTutorial;

	private Node _buttonNextTutorial;
	private Node _buttonPreviousTutorial;

	private Node _buttonClosePauseSubMenuTutorial;
	private Label _textButtonComponentClosePauseSubMenuTutorial;

	private bool _isPauseSubMenuTutorialOpened;

	private int _currentNoteIndex = 0;

	private bool _isInitialized;
	private bool _wasRightArrowPressed;
	private bool _wasLeftArrowPressed;

	public void Initialize(
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		Node canvasPauseSubMenuTutorial,
		GameTutorialsList tutorialsList,
		ViewModelPauseSubMenuTutorial viewModelPauseSubMenuTutorial)
	{
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_viewModelPauseSubMenuTutorial = viewModelPauseSubMenuTutorial;

		_tutorialsList = tutorialsList;

		_textTutorial = _viewModelPauseSubMenuTutorial.TextTutorial;
		_textComponentTutorial = _textTutorial.GetNodeOrNull<Label>();
		_imageTutorial = _viewModelPauseSubMenuTutorial.ImageTutorial;
		_imageComponentTutorial = _imageTutorial.GetNodeOrNull<TextureRect>();

		_buttonNextTutorial = _viewModelPauseSubMenuTutorial.ButtonNextTutorial;
		_buttonNextTutorial.GetNodeOrNull<Button>().onClick.AddListener(() => NextTutorial());
		_buttonPreviousTutorial = _viewModelPauseSubMenuTutorial.ButtonPreviousTutorial;
		_buttonPreviousTutorial.GetNodeOrNull<Button>().onClick.AddListener(() => PreviousTutorial());

		_buttonClosePauseSubMenuTutorial = _viewModelPauseSubMenuTutorial.ButtonClosePauseSubMenuTutorial;
		_buttonClosePauseSubMenuTutorial.GetNodeOrNull<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuTutorial = _viewModelPauseSubMenuTutorial.TextButtonClosePauseSubMenuTutorial.GetNodeOrNull<Label>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenTutorialSubMenu += ShowTutorialSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideTutorialSubMenuCanvas;

		_isInitialized = true;

		GD.Print("PauseSubMenuTutorialController Initialized");
	}

	private void Update()
	{
		if (!_isInitialized)
			return;

		if (_isPauseSubMenuTutorialOpened)
		{
			bool rightArrowPressed = Input.IsKeyPressed(Key.Right);
			bool leftArrowPressed = Input.IsKeyPressed(Key.Left);

			if (rightArrowPressed && !_wasRightArrowPressed)
			{
				NextTutorial();
			}

			if (leftArrowPressed && !_wasLeftArrowPressed)
			{
				PreviousTutorial();
			}

			_wasRightArrowPressed = rightArrowPressed;
			_wasLeftArrowPressed = leftArrowPressed;
		}
		else
		{
			_wasRightArrowPressed = Input.IsKeyPressed(Key.Right);
			_wasLeftArrowPressed = Input.IsKeyPressed(Key.Left);
		}
	}

	private void ShowTutorialSubMenuCanvas()
	{
		_isPauseSubMenuTutorialOpened = true;
		_canvasPauseSubMenuTutorial .Set("visible", true);

		if (_tutorialsList.Notes.Count > 0)
		{
			_currentNoteIndex = 0;
			UpdateUIWithCurrentNote();
		}
	}

	private void HideTutorialSubMenuCanvas()
	{
		if (_isPauseSubMenuTutorialOpened)
		{
			_isPauseSubMenuTutorialOpened = false;
			_canvasPauseSubMenuTutorial .Set("visible", false);
			GD.Print("TutorialSubMenu closed");
		}
	}

	private void NextTutorial()
	{
		_currentNoteIndex = (_currentNoteIndex + 1) % _tutorialsList.Notes.Count;

		UpdateUIWithCurrentNote();
	}

	private void PreviousTutorial()
	{
		_currentNoteIndex = (_currentNoteIndex - 1 + _tutorialsList.Notes.Count) % _tutorialsList.Notes.Count;

		UpdateUIWithCurrentNote();
	}

	private void UpdateUIWithCurrentNote()
	{
		InteractionObjectNoteData data = _tutorialsList.Notes[_currentNoteIndex];

		GD.Print($"Showing TutorialNote #{_currentNoteIndex + 1}");

		// 1. Получаем сырой текст (например: "Двигайтесь на кнопки {MoveForward}")
		string rawTextToShow = _localizationManager.GetNoteLanguageSuffix(data);

		// 2. ОБЯЗАТЕЛЬНО пропускаем его через ReplaceActionTags и сохраняем результат
		string finalText = ReplaceActionTags(rawTextToShow);

		// 3. Отдаем ВЕСЬ отформатированный текст компоненту TextMeshPro
		_textComponentTutorial.text = finalText;

		Texture2D spriteToShow = data.NoteImage;
		_imageComponentTutorial.Texture2D = spriteToShow;

		if (spriteToShow != null)
		{
			_imageComponentTutorial.Texture2D = spriteToShow;
			_imageTutorial .Set("visible", true);
		}
		else
		{
			_imageTutorial .Set("visible", false);
		}
	}

	private string ReplaceActionTags(string input)
	{
		// Ищем всё, что находится внутри фигурных скобок {здесь}
		System.Text.RegularExpressions.Regex tagRegex = new(@"\{([^}]+)\}");

		return tagRegex.Replace(input, match =>
		{
			string actionStringFromFile = match.Groups[1].Value;

			// Пытаемся превратить строку из файла (например, "Run") в наш Enum
			if (System.Enum.TryParse(typeof(InputControlsEnum), actionStringFromFile, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				// Спрашиваем у инпута актуальное имя кнопки
				string keyName = _inputDevice.GetNameOfKey(actionEnum);

				// Возвращаем готовую строку с кастомным цветом
				return _inputDevice.GetNameOfKey(actionEnum);
			}
			else
			{
				GD.PushWarning($"В туториале найден неизвестный тег {{{actionStringFromFile}}}. Проверьте .txt файл.");
				return match.Value;
			}
		});
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textButtonComponentClosePauseSubMenuTutorial.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Tutorial_ButtonClosePauseSubMenuTutorial");
	}
}