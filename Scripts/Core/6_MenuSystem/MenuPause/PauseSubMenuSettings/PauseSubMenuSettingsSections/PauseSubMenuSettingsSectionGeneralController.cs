using Godot;
using System.Collections.Generic;
public partial class PauseSubMenuSettingsSectionGeneralController : Node
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private MenuManager _menuManager;
	private PauseMenuController _pausedMenuController;
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;

	private Node _dropdownScreenResolution;
	private OptionButton _dropdownComponentScreenResolution;
	private Node _textDropdownScreenResolution;
	private Label _textComponentDropdownScreenResolution;

	private Node _dropdownWindowType;
	private OptionButton _dropdownComponentWindowType;
	private Node _textDropdownWindowType;
	private Label _textComponentDropdownWindowType;

	private Node _dropdownLimitFPS;
	private OptionButton _dropdownComponentLimitFPS;
	private Node _textDropdownLimitFPS;
	private Label _textComponentDropdownLimitFPS;
	private int _currentFPSlimit;

	private Node _dropdownHUDType;
	private OptionButton _dropdownComponentHUDType;
	private Node _textDropdownHUDType;
	private Label _textComponentDropdownHUDType;
	public delegate void HUDtypeHandler();
	public event HUDtypeHandler OnHUDfull;
	public event HUDtypeHandler OnHUDdialoguesOnly;
	public event HUDtypeHandler OnHUDdialoguesHide;
	public event HUDtypeHandler OnHUDturnOff;

	private Node _dropdownWeaponWheelType;
	private OptionButton _dropdownComponentWeaponWheelType;
	private Node _textDropdownWeaponWheelType;
	private Label _textComponentDropdownWeaponWheelType;
	private WeaponWheelMenuTypes _currentWeaponWheelMenuType;

	private Node _sliderCameraFOV;
	private HSlider _sliderComponentCameraFOV;
	public float CurrentValueCameraFOV { get; private set; }
	private const float _MIN_VALUE_CAMERA_FOV = 60f;
	public float MIN_VALUE_CAMERA_FOV => _MIN_VALUE_CAMERA_FOV;
	private const float _MAX_VALUE_CAMERA_FOV = 120f;
	public float MAX_VALUE_CAMERA_FOV => _MAX_VALUE_CAMERA_FOV;
	private Node _textNumberSliderCameraFOV;
	private Label _textComponentNumberSliderCameraFOV;
	private Node _textSliderCameraFOV;
	private Label _textComponentSliderCameraFOV;
	public delegate void CameraFOVeventHandler(float newCameraFOV, float MIN_VALUE_CAMERA_FOV, float MAX_VALUE_CAMERA_FOV);
	public event CameraFOVeventHandler OnCameraFOVchanged;

	private Node _sliderScreenBrightness;
	private HSlider _sliderComponentScreenBrightness;
	private float _currentValueScreenBrightness;
	private const float _MIN_VALUE_SCREEN_BRIGHTNESS = 0f;
	private const float _MAX_VALUE_SCREEN_BRIGHTNESS = 100f;
	private const float _DEFAULT_VALUE_SCREEN_BRIGHTNESS = 50f;
	private Node _textNumberSliderScreenBrightness;
	private Label _textComponentNumberSliderScreenBrightness;
	private Node _textSliderScreenBrightness;
	private Label _textComponentSliderScreenBrightness;
	public delegate void ScreenBrightnessHandler(int brightnessValue);
	public event ScreenBrightnessHandler OnScreenBrightnessChanged;

	private Node _buttonGameDifficulty;
	private Button _buttonComponentGameDifficulty;
	private Node _textButtonGameDifficulty;
	private Label _textComponentButtonGameDifficulty;
	private Node _textButtonDifficultyNormal;
	private Label _textComponentButtonDifficultyNormal;
	public delegate void SubMenuChooseGameDifficultyHandler();
	public event SubMenuChooseGameDifficultyHandler OnOpenSubMenuGameDifficulty;
	public event SubMenuChooseGameDifficultyHandler OnCloseSubMenuGameDifficulty;

	private Node _toggleShowIngameHints;
	private CheckBox _toggleComponentShowIngameHints;
	private Node _textToggleShowIngameHints;
	private Label _textComponentToggleShowIngameHints;
	public bool AreIngameTutorialsEnabled { get; private set; }

	private Node _toggleShowBlood;
	private CheckBox _toggleComponentShowBlood;
	private Node _textToggleShowBlood;
	private Label _textComponentToggleShowBlood;
	private bool _isBloodEnabled;
	public delegate void BloodVisibilityHandler();
	public event BloodVisibilityHandler OnShowBlood;
	public event BloodVisibilityHandler OnHideBlood;

	public delegate void SavePlayerPrefsCameraSettingsEventHandler();
	public event SavePlayerPrefsCameraSettingsEventHandler OnSaveCameraSettingsData;

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		MenuManager menuManager,
		PauseMenuController pausedMenuController,
		PauseSubMenuSettingsController pauseSubMenuSettingsController,
		ViewModelPauseSubMenuSettingsSectionGeneral viewModelPauseSubMenuSettings)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;
		_menuManager = menuManager;
		_pausedMenuController = pausedMenuController;
		_pauseSubMenuSettingsController = pauseSubMenuSettingsController;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetNodeOrNull<Label>();

		_dropdownScreenResolution = viewModelPauseSubMenuSettings.DropdownScreenResolution;
		_dropdownComponentScreenResolution = viewModelPauseSubMenuSettings.DropdownScreenResolution.GetNodeOrNull<OptionButton>();
		_dropdownComponentScreenResolution.ItemSelected += index => SetScreenResolution((int)index);
		_textDropdownScreenResolution = viewModelPauseSubMenuSettings.TextDropdownScreenResolution;
		_textComponentDropdownScreenResolution = viewModelPauseSubMenuSettings.TextDropdownScreenResolution.GetNodeOrNull<Label>();

		_dropdownWindowType = viewModelPauseSubMenuSettings.DropdownWindowType;
		_dropdownComponentWindowType = viewModelPauseSubMenuSettings.DropdownWindowType.GetNodeOrNull<OptionButton>();
		_dropdownComponentWindowType.ItemSelected += index => SetWindowType((int)index);
		_textDropdownWindowType = viewModelPauseSubMenuSettings.TextDropdownWindowType;
		_textComponentDropdownWindowType = viewModelPauseSubMenuSettings.TextDropdownWindowType.GetNodeOrNull<Label>();

		_dropdownLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS;
		_dropdownComponentLimitFPS = viewModelPauseSubMenuSettings.DropdownLimitFPS.GetNodeOrNull<OptionButton>();
		_dropdownComponentLimitFPS.ItemSelected += index => SetFPSlimit((int)index);
		_textDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS;
		_textComponentDropdownLimitFPS = viewModelPauseSubMenuSettings.TextDropdownLimitFPS.GetNodeOrNull<Label>();

		_dropdownHUDType = viewModelPauseSubMenuSettings.DropdownHUDType;
		_dropdownComponentHUDType = viewModelPauseSubMenuSettings.DropdownHUDType.GetNodeOrNull<OptionButton>();
		_dropdownComponentHUDType.ItemSelected += index => SetHUDType((int)index);
		_textDropdownHUDType = viewModelPauseSubMenuSettings.TextDropdownHUDType;
		_textComponentDropdownHUDType = viewModelPauseSubMenuSettings.TextDropdownHUDType.GetNodeOrNull<Label>();

		_dropdownWeaponWheelType = viewModelPauseSubMenuSettings.DropdownWeaponWheelType;
		_dropdownComponentWeaponWheelType = viewModelPauseSubMenuSettings.DropdownWeaponWheelType.GetNodeOrNull<OptionButton>();
		_dropdownComponentWeaponWheelType.onValueChanged.AddListener(SetWeaponWheelType);
		_textDropdownWeaponWheelType = viewModelPauseSubMenuSettings.TextDropdownWeaponWheelType;
		_textComponentDropdownWeaponWheelType = viewModelPauseSubMenuSettings.TextDropdownWeaponWheelType.GetNodeOrNull<Label>();

		_sliderCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV;
		_sliderComponentCameraFOV = viewModelPauseSubMenuSettings.SliderCameraFOV.GetNodeOrNull<HSlider>();
		_sliderComponentCameraFOV.minValue = _MIN_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.maxValue = _MAX_VALUE_CAMERA_FOV;
		_sliderComponentCameraFOV.ValueChanged += value => SetCameraFOV((float)value);
		_textNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV;
		_textComponentNumberSliderCameraFOV = viewModelPauseSubMenuSettings.NumberSliderCameraFOV.GetNodeOrNull<Label>();
		_textSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV;
		_textComponentSliderCameraFOV = viewModelPauseSubMenuSettings.TextSliderCameraFOV.GetNodeOrNull<Label>();

		_sliderScreenBrightness = viewModelPauseSubMenuSettings.SliderScreenBrightness;
		_sliderComponentScreenBrightness = viewModelPauseSubMenuSettings.SliderScreenBrightness.GetNodeOrNull<HSlider>();
		_sliderComponentScreenBrightness.minValue = _MIN_VALUE_SCREEN_BRIGHTNESS;
		_sliderComponentScreenBrightness.maxValue = _MAX_VALUE_SCREEN_BRIGHTNESS;
		_sliderComponentScreenBrightness.ValueChanged += value => SetScreenBrightness((float)value);
		_textNumberSliderScreenBrightness = viewModelPauseSubMenuSettings.NumberSliderScreenBrightness;
		_textComponentNumberSliderScreenBrightness = viewModelPauseSubMenuSettings.NumberSliderScreenBrightness.GetNodeOrNull<Label>();
		_textSliderScreenBrightness = viewModelPauseSubMenuSettings.TextSliderScreenBrightness;
		_textComponentSliderScreenBrightness = viewModelPauseSubMenuSettings.TextSliderScreenBrightness.GetNodeOrNull<Label>();

		_buttonGameDifficulty = viewModelPauseSubMenuSettings.ButtonGameDifficulty;
		_buttonComponentGameDifficulty = viewModelPauseSubMenuSettings.ButtonGameDifficulty.GetNodeOrNull<Button>();
		_buttonComponentGameDifficulty.Pressed += OpenSubMenuChooseGameDifficulty;
		_textButtonGameDifficulty = viewModelPauseSubMenuSettings.TextButtonGameDifficulty;
		_textComponentButtonGameDifficulty = viewModelPauseSubMenuSettings.TextButtonGameDifficulty.GetNodeOrNull<Label>();
		_textButtonDifficultyNormal = viewModelPauseSubMenuSettings.TextButtonDifficultyNormal;
		_textComponentButtonDifficultyNormal = viewModelPauseSubMenuSettings.TextButtonDifficultyNormal.GetNodeOrNull<Label>();

		_toggleShowIngameHints = viewModelPauseSubMenuSettings.ToggleShowIngameHints;
		_toggleComponentShowIngameHints = viewModelPauseSubMenuSettings.ToggleShowIngameHints as CheckBox
			?? viewModelPauseSubMenuSettings.ToggleShowIngameHints.GetNodeOrNull<CheckBox>();
		_toggleComponentShowIngameHints.Toggled += SetShowIngameTutorials;
		_textToggleShowIngameHints = viewModelPauseSubMenuSettings.TextToggleShowIngameHints;
		_textComponentToggleShowIngameHints = viewModelPauseSubMenuSettings.TextToggleShowIngameHints.GetNodeOrNull<Label>();

		_toggleShowBlood = viewModelPauseSubMenuSettings.ToggleShowBlood;
		_toggleComponentShowBlood = viewModelPauseSubMenuSettings.ToggleShowBlood as CheckBox
			?? viewModelPauseSubMenuSettings.ToggleShowBlood.GetNodeOrNull<CheckBox>();
		_toggleComponentShowBlood.Toggled += SetShowBlood;
		_textToggleShowBlood = viewModelPauseSubMenuSettings.TextToggleShowBlood;
		_textComponentToggleShowBlood = viewModelPauseSubMenuSettings.TextToggleShowBlood.GetNodeOrNull<Label>();

		_gameController.OnActivateMainMenuEndGameTitlesActive += () => OnCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
	
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_playerPrefsSettingsController.OnApplySettingsSectionGeneralPlayerPrefs += ApplySystemLoadedSettings;

		GD.Print("SettingsSectionGeneralController Initialized");
	}

	public override void _Process(double delta)
	{
		if (_inputDevice.GetKeyPauseMenu() && _menuManager.PauseMenuLevel.Count == 3 && !_menuManager.IsConfirmationOnExitToMainMenuOpened && !_pausedMenuController.IsPauseConfirmMenuOpened)
		{
			CloseSubMenuChooseGameDifficulty();
		}
	}

	public void SaveSettingsGeneral()
	{
		var currentData = new PlayerPrefsData();

		OnSaveCameraSettingsData?.Invoke();

		currentData.CameraFOV = CurrentValueCameraFOV;
		currentData.ScreenBrightness = _currentValueScreenBrightness;
		currentData.FPSlimit = _currentFPSlimit;
		currentData.WeaponWheelType = _currentWeaponWheelMenuType.ToString();
		currentData.ShowIngameTutorials = _toggleComponentShowIngameHints.isOn;
		currentData.ShowBlood = _toggleComponentShowBlood.isOn;

		_playerPrefsSettingsController.SaveSettingsGeneral(currentData);
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetFPSlimit(data, data.FPSlimit);

		//GD.Print(data.WeaponWheelType.ToString());

		if (data.WeaponWheelType == WeaponWheelMenuTypes._2D.ToString())
		{
			SetWeaponWheelType(0);
		}
		else
		{
			SetWeaponWheelType(1);
		}

		SetCameraFOV(data.CameraFOV);
		_sliderComponentCameraFOV.value = data.CameraFOV;
		CurrentValueCameraFOV = data.CameraFOV;
		SetScreenBrightness(data.ScreenBrightness);
		_sliderComponentScreenBrightness.value = data.ScreenBrightness;
		_currentValueScreenBrightness = data.ScreenBrightness;
		SetShowIngameTutorials(data.ShowIngameTutorials);
		SetShowBlood(data.ShowBlood);
	}

	public void ResetSettingsGeneral()
	{
		_playerPrefsSettingsController.ResetSettingsGeneral();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			FPSlimit = 60,
			WeaponWheelType = WeaponWheelMenuTypes._2D.ToString(),
			CameraFOV = _MIN_VALUE_CAMERA_FOV,
			ScreenBrightness = _DEFAULT_VALUE_SCREEN_BRIGHTNESS,
			ShowIngameTutorials = true,
			ShowBlood = true,
		};

		_playerPrefsSettingsController.SaveSettingsGeneral(defaultData);

		SetFPSlimit(defaultData, 60);

		SetWeaponWheelType(0);

		SetCameraFOV(_MIN_VALUE_CAMERA_FOV);
		_sliderComponentCameraFOV.value = _MIN_VALUE_CAMERA_FOV;
		SetScreenBrightness(_DEFAULT_VALUE_SCREEN_BRIGHTNESS);
		_sliderComponentScreenBrightness.value = _DEFAULT_VALUE_SCREEN_BRIGHTNESS;
		SetShowIngameTutorials(true);
		SetShowBlood(true);
	}

	public void SetScreenResolution(int dropdownScreenResolutionSlot)
	{
		if (dropdownScreenResolutionSlot == 0)
		{
		SetWindowSize(1920, 1080);
		}
		else if (dropdownScreenResolutionSlot == 1)
		{
		SetWindowSize(2560, 1440);
		}
		else if (dropdownScreenResolutionSlot == 2)
		{
		SetWindowSize(3840, 2160);
		}
		else if (dropdownScreenResolutionSlot == 3)
		{
		SetWindowSize(1920, 1200);
		}
		else if (dropdownScreenResolutionSlot == 4)
		{
		SetWindowSize(1080, 1440);
		}
	}

	public void SetWindowType(int dropdownWindowTypeSlot)
	{
		if (dropdownWindowTypeSlot == 0)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else if (dropdownWindowTypeSlot == 1)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else if (dropdownWindowTypeSlot == 2)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
		}
	}

	public void SetFPSlimit(int dropdownFPSlimitSlot)
	{
		int newFPSlimit;

		if (dropdownFPSlimitSlot == 0)
		{
			newFPSlimit = 30;
		}
		else if (dropdownFPSlimitSlot == 1)
		{
			newFPSlimit = 60;
		}
		else if (dropdownFPSlimitSlot == 2)
		{
			newFPSlimit = 90;
		}
		else if (dropdownFPSlimitSlot == 3)
		{
			newFPSlimit = 144;
		}
		else
		{
			newFPSlimit = 999;
		}

		_currentFPSlimit = newFPSlimit;
		Engine.MaxFps = newFPSlimit >= 999 ? 0 : newFPSlimit;
	}

	public void SetFPSlimit(PlayerPrefsData data, int newFPSlimit)
	{
		if (newFPSlimit == 30)
		{
			_dropdownComponentLimitFPS.value = 0;
		}
		if (newFPSlimit == 60)
		{
			_dropdownComponentLimitFPS.value = 1;
		}
		if (newFPSlimit == 90)
		{
			_dropdownComponentLimitFPS.value = 2;
		}
		if (newFPSlimit == 144)
		{
			_dropdownComponentLimitFPS.value = 3;
		}
		if (newFPSlimit == 999)
		{
			_dropdownComponentLimitFPS.value = 4;
		}

		_currentFPSlimit = newFPSlimit;
		Engine.MaxFps = newFPSlimit >= 999 ? 0 : newFPSlimit;
	}

	public void SetHUDType(int dropdownHUDTypeSlot)
	{
		if (dropdownHUDTypeSlot == 0)
		{
			OnHUDfull?.Invoke();
		}
		else if (dropdownHUDTypeSlot == 1)
		{
			OnHUDdialoguesOnly?.Invoke();	
		}
		else if (dropdownHUDTypeSlot == 2)
		{
			OnHUDdialoguesHide?.Invoke();
		}
		else if (dropdownHUDTypeSlot == 3)
		{
			OnHUDturnOff?.Invoke();
		}
	}

	public void SetWeaponWheelType(int dropdownWeaponWheelTypeSlot)
	{
		if (dropdownWeaponWheelTypeSlot == 0)
		{
			_bootstrap.ChangeWeaponWheelType(WeaponWheelMenuTypes._2D);

			_currentWeaponWheelMenuType = WeaponWheelMenuTypes._2D;

			_dropdownComponentWeaponWheelType.value = 0;
		}
		else if (dropdownWeaponWheelTypeSlot == 1)
		{
			_bootstrap.ChangeWeaponWheelType(WeaponWheelMenuTypes._3D);

			_currentWeaponWheelMenuType = WeaponWheelMenuTypes._3D;

			_dropdownComponentWeaponWheelType.value = 1;
		}
	}

	public void SetCameraFOV(float newCameraFOV)
	{
		CurrentValueCameraFOV = newCameraFOV;

		_textComponentNumberSliderCameraFOV.text = ((int)newCameraFOV).ToString();

		if (!_gameController.IsMainMenuOrEndGameTitlesActive)
		{
			OnCameraFOVchanged?.Invoke(newCameraFOV, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
		else
		{
			OnCameraFOVchanged?.Invoke(60, _MIN_VALUE_CAMERA_FOV, _MAX_VALUE_CAMERA_FOV);
		}
	}

	public void GetCameraCurrentFOV(float FOV)
	{
		CurrentValueCameraFOV = FOV;
	}

	public void SetScreenBrightness(float newScreenBrightness)
	{
		_currentValueScreenBrightness = newScreenBrightness;
		_textComponentNumberSliderScreenBrightness.text = ((int)newScreenBrightness).ToString();

		OnScreenBrightnessChanged?.Invoke((int)newScreenBrightness);
	}

	private void OpenSubMenuChooseGameDifficulty()
	{
		GD.Print("OPEN DIFFICULTY");
		_menuManager.PushPauseMenuLevel();
		_pauseSubMenuSettingsController.HideSettingsSubMenuCanvas();
		OnOpenSubMenuGameDifficulty?.Invoke();
	}

	public void CloseSubMenuChooseGameDifficulty()
	{
		GD.Print("CLOSE DIFFICULTY");
		_menuManager.PopPauseMenuLevel();
		_pauseSubMenuSettingsController.ShowSettingsSubMenuCanvas();
		OnCloseSubMenuGameDifficulty?.Invoke();
	}

	public void SetShowIngameTutorials(bool isOn)
	{
		if (isOn)
		{
			AreIngameTutorialsEnabled = true;
		}
		else
		{
			AreIngameTutorialsEnabled = false;
		}

		_toggleComponentShowIngameHints.isOn = isOn;
	}

	public void SetShowBlood(bool isOn)
	{
		if (isOn)
		{
			OnShowBlood?.Invoke();
		}
		else
		{
			OnHideBlood?.Invoke();
		}

		_toggleComponentShowBlood.isOn = isOn;
	}

	private static void SetWindowSize(int width, int height)
	{
		DisplayServer.WindowSetSize(new Vector2I(width, height));
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentDropdownScreenResolution.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextDropdownScreenResolution");

		List<string> dropdownWindowTypelocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownWindowTypeFullscreen"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownWindowTypeBorderlessWindowed"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownWindowTypeWindowed"),
		};
		_dropdownComponentWindowType.ClearOptions();
		_dropdownComponentWindowType.AddOptions(dropdownWindowTypelocalizedOptions);
		_textComponentDropdownWindowType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextDropdownWindowType");

		List<string> dropdownLimitFPSlocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownLimitFPS30"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownLimitFPS60"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownLimitFPS90"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownLimitFPS144"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownLimitFPSunlimited")
		};
		_dropdownComponentLimitFPS.ClearOptions();
		_dropdownComponentLimitFPS.AddOptions(dropdownLimitFPSlocalizedOptions);
		_textComponentDropdownLimitFPS.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextDropdownFPSlimit");

		List<string> dropdownHUDTypelocalizedOptions = new List<string>
		{
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownHUDTypeFull"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownHUDTypeDialoguesOnly"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownHUDTypeDialoguesTurnOff"),
			_localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_DropdownHUDTypeTurnedOff")
		};
		_dropdownComponentHUDType.ClearOptions();
		_dropdownComponentHUDType.AddOptions(dropdownHUDTypelocalizedOptions);
		_textComponentDropdownHUDType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextDropdownHUDType");

		_textComponentDropdownWeaponWheelType.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextDropdownWeaponWheelType");

		_textComponentSliderCameraFOV.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextSliderCameraFOV");

		_textComponentSliderScreenBrightness.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextSliderScreenBrightness");

		_textComponentButtonGameDifficulty.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextButtonShowIngameHints");
		_textComponentButtonDifficultyNormal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_ButtonDifficultyNormal");

		_textComponentToggleShowIngameHints.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextToggleShowIngameHints");

		_textComponentToggleShowBlood.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_TextToggleShowBlood");
	}
}