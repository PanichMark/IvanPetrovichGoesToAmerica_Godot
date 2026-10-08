using Godot;
public partial class PauseSubMenuSettingsController : Node
{
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private Node _canvasPauseSubMenuSettings;

	private Node _buttonSaveSettings;
	private Button _buttonComponentSaveSettings;
	private Node _textButtonSaveSettings;
	private Label _textComponentButtonSaveSettings;

	private Node _buttonResetSettings;
	private Button _buttonComponentResetSettings;
	private Node _textButtonResetSettings;
	private Label _textComponentButtonResetSettings;

	private Node _buttonClosePauseSubMenuSettings;
	private Button _buttonComponentClosePauseSubMenuSettings;
	private Node _textButtonClosePauseSubMenuSettings;
	private Label _textComponentButtonClosePauseSubMenuSettings;

	private Node _subSettingsSectionGeneral;
	private Node _imageBackgroundSectionGeneral;
	private Node _buttonSubSettingsSectionGeneral;
	private Button _buttonComponentSubSettingsSectionGeneral;
	private Node _textButtonSubSettingsSectionGeneral;
	private Label _textComponentButtonSubSettingsSectionGeneral;

	private Node _subSettingsSectionControls;
	private Node _imageBackgroundSectionControls;
	private Node _buttonSubSettingsSectionControls;
	private Button _buttonComponentSubSettingsSectionControls;
	private Node _textButtonSubSettingsSectionControls;
	private Label _textComponentButtonSubSettingsSectionControls;

	private Node _subSettingsSectionGraphics;
	private Node _imageBackgroundSectionGraphics;
	private Node _buttonSubSettingsSectionGraphics;
	private Button _buttonComponentSubSettingsSectionGraphics;
	private Node _textButtonSubSettingsSectionGraphics;
	private Label _textComponentButtonSubSettingsSectionGraphics;

	private Node _subSettingsSectionAudio;
	private Node _imageBackgroundSectionAudio;
	private Node _buttonSubSettingsSectionAudio;
	private Button _buttonComponentSubSettingsSectionAudio;
	private Node _textButtonSubSettingsSectionAudio;
	private Label _textComponentButtonSubSettingsSectionAudio;

	private string _currentOpenedSubSettingsSection;
	private bool _isPauseSubMenuSettingsOpened;

	public delegate void ConfirmChangeSettingsEventHandler();
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGeneralConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsControlsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsGraphicsConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestSaveSettingsAudioConfirmation;
	public event ConfirmChangeSettingsEventHandler OnRequestResetSettingsAudioConfirmation;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		Node canvasPauseSubMenuSettings,
		ViewModelPauseSubMenuSettings viewModelPauseSubMenuSettings)
	{
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;

		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;

		_buttonSaveSettings = viewModelPauseSubMenuSettings.ButtonSaveGameSettings;
		_buttonComponentSaveSettings = viewModelPauseSubMenuSettings.ButtonSaveGameSettings.GetNodeOrNull<Button>();
		_buttonComponentSaveSettings.onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestSaveSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestSaveSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestSaveSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestSaveSettingsAudioConfirmation();
			}
		});
		_textButtonSaveSettings = viewModelPauseSubMenuSettings.TextButtonSaveGameSettings;
		_textComponentButtonSaveSettings = viewModelPauseSubMenuSettings.TextButtonSaveGameSettings.GetNodeOrNull<Label>();

		_buttonResetSettings = viewModelPauseSubMenuSettings.ButtonResetGameSettings;
		_buttonComponentResetSettings = viewModelPauseSubMenuSettings.ButtonResetGameSettings.GetNodeOrNull<Button>();
		_buttonComponentResetSettings.GetNodeOrNull<Button>().onClick.AddListener(() =>
		{
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.General.ToString())
			{
				OnRequestResetSettingsGeneralConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Controls.ToString())
			{
				OnRequestResetSettingsControlsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Graphics.ToString())
			{
				OnRequestResetSettingsGraphicsConfirmation();
			}
			if (_currentOpenedSubSettingsSection == PauseSubMenuSettingsSectionTypes.Audio.ToString())
			{
				OnRequestResetSettingsAudioConfirmation();
			}
		});
		_textButtonResetSettings = viewModelPauseSubMenuSettings.TextButtonResetGameSettings;
		_textComponentButtonResetSettings = viewModelPauseSubMenuSettings.TextButtonResetGameSettings.GetNodeOrNull<Label>();

		_buttonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.ButtonClosePauseSubMenuSettings;
		_buttonComponentClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.ButtonClosePauseSubMenuSettings.GetNodeOrNull<Button>();
		_buttonComponentClosePauseSubMenuSettings.onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.TextButtonClosePauseSubMenuSettings;
		_textComponentButtonClosePauseSubMenuSettings = viewModelPauseSubMenuSettings.TextButtonClosePauseSubMenuSettings.GetNodeOrNull<Label>();

		_subSettingsSectionGeneral = viewModelPauseSubMenuSettings.SubSettingsSectionGeneral;
		_imageBackgroundSectionGeneral = viewModelPauseSubMenuSettings.ImageBackgroundSectionGeneral;
		_buttonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGeneral;
		_buttonComponentSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGeneral.GetNodeOrNull<Button>();
		_buttonComponentSubSettingsSectionGeneral.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGeneral));
		_textButtonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGeneral;
		_textComponentButtonSubSettingsSectionGeneral = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGeneral.GetNodeOrNull<Label>();

		_subSettingsSectionControls = viewModelPauseSubMenuSettings.SubSettingsSectionControls;
		_imageBackgroundSectionControls = viewModelPauseSubMenuSettings.ImageBackgroundSectionControls;
		_buttonSubSettingsSectionControls = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionControls;
		_buttonComponentSubSettingsSectionControls = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionControls.GetNodeOrNull<Button>();
		_buttonComponentSubSettingsSectionControls.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionControls));
		_textButtonSubSettingsSectionControls = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionControls;
		_textComponentButtonSubSettingsSectionControls = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionControls.GetNodeOrNull<Label>();

		_subSettingsSectionGraphics = viewModelPauseSubMenuSettings.SubSettingsSectionGraphics;
		_imageBackgroundSectionGraphics = viewModelPauseSubMenuSettings.ImageBackgroundSectionGraphics;
		_buttonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGraphics;
		_buttonComponentSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGraphics.GetNodeOrNull<Button>();
		_buttonComponentSubSettingsSectionGraphics.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionGraphics));
		_textButtonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGraphics;
		_textComponentButtonSubSettingsSectionGraphics = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionGraphics.GetNodeOrNull<Label>();

		_subSettingsSectionAudio = viewModelPauseSubMenuSettings.SubSettingsSectionAudio;
		_imageBackgroundSectionAudio = viewModelPauseSubMenuSettings.ImageBackgroundSectionAudio;
		_buttonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionAudio;
		_buttonComponentSubSettingsSectionAudio = viewModelPauseSubMenuSettings.ButtonSubSettingsSectionAudio.GetNodeOrNull<Button>();
		_buttonComponentSubSettingsSectionAudio.onClick.AddListener(() => OpenSubSettingsSection(_subSettingsSectionAudio));
		_textButtonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionAudio;
		_textComponentButtonSubSettingsSectionAudio = viewModelPauseSubMenuSettings.TextButtonSubSettingsSectionAudio.GetNodeOrNull<Label>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenSettingsSubMenu += () => OpenSubSettingsSection(_subSettingsSectionGeneral);
		_pauseMenuController.OnOpenSettingsSubMenu += () =>
		{
			ShowSettingsSubMenuCanvas();
			_isPauseSubMenuSettingsOpened = true;
		};
		_pauseMenuController.OnCloseAnyPauseSubMenu += () =>
		{
			HideSettingsSubMenuCanvas();
			_isPauseSubMenuSettingsOpened = false;
		};
		// do not open Settings Sub Menu while closing Confirmation menu

		GD.Print("PauseSubMenuSettingsController Initialized");
	}

	public void ShowSettingsSubMenuCanvas()
	{
		_canvasPauseSubMenuSettings.Node .Set("visible", true);

		GD.Print("Opened SettingsSubMenu");
	}

	public void HideSettingsSubMenuCanvas()
	{
		if (_isPauseSubMenuSettingsOpened)
		{
			_canvasPauseSubMenuSettings.Node .Set("visible", false);

			GD.Print("Closed SettingsSubMenu");
		}
	}

	private void OpenSubSettingsSection(Node subSettingsSection)
	{
		subSettingsSection .Set("visible", true);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionGeneral.interactable = false;
			_imageBackgroundSectionGeneral .Set("visible", true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.General.ToString();
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionGraphics);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionControls.interactable = false;
			_imageBackgroundSectionControls .Set("visible", true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Controls.ToString();
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionAudio);

			_buttonComponentSubSettingsSectionGraphics.interactable = false;
			_imageBackgroundSectionGraphics .Set("visible", true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Graphics.ToString();
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			CloseSubSettingsSection(_subSettingsSectionGeneral);
			CloseSubSettingsSection(_subSettingsSectionControls);
			CloseSubSettingsSection(_subSettingsSectionGraphics);

			_buttonComponentSubSettingsSectionAudio.interactable = false;
			_imageBackgroundSectionAudio .Set("visible", true);

			_currentOpenedSubSettingsSection = PauseSubMenuSettingsSectionTypes.Audio.ToString();
		}
	}

	private void CloseSubSettingsSection(Node subSettingsSection)
	{
		subSettingsSection .Set("visible", false);

		if (subSettingsSection == _subSettingsSectionGeneral)
		{
			_buttonComponentSubSettingsSectionGeneral.interactable = true;
			_imageBackgroundSectionGeneral .Set("visible", false);
		}
		if (subSettingsSection == _subSettingsSectionControls)
		{
			_buttonComponentSubSettingsSectionControls.interactable = true;
			_imageBackgroundSectionControls .Set("visible", false);
		}
		if (subSettingsSection == _subSettingsSectionGraphics)
		{
			_buttonComponentSubSettingsSectionGraphics.interactable = true;
			_imageBackgroundSectionGraphics .Set("visible", false);
		}
		if (subSettingsSection == _subSettingsSectionAudio)
		{
			_buttonComponentSubSettingsSectionAudio.interactable = true;
			_imageBackgroundSectionAudio .Set("visible", false);
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentButtonSaveSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonSaveSettings");
		_textComponentButtonResetSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonResetSettings");
		_textComponentButtonClosePauseSubMenuSettings.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonClosePauseSubMenuSettings");

		_textComponentButtonSubSettingsSectionGeneral.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonSubSettingsSectionGeneral");
		_textComponentButtonSubSettingsSectionControls.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonSubSettingsSectionControls");
		_textComponentButtonSubSettingsSectionGraphics.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonSubSettingsSectionGraphics");
		_textComponentButtonSubSettingsSectionAudio.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_ButtonSubSettingsSectionAudio");
	}
}