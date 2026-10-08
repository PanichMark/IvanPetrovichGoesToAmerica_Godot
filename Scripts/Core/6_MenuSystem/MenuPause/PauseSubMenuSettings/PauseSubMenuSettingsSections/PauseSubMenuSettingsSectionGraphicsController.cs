using Godot;
using System;
using System.Drawing;
public partial class PauseSubMenuSettingsSectionGraphicsController : Node
{
	private LocalizationManager _localizationManager;

	private Node TEXT_NO_GRAPHICS_SETTINGS_YET;
	private Label TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET;

	public delegate void SavePlayerPrefsSettingsEventHandler(PlayerPrefsData data);
	public event SavePlayerPrefsSettingsEventHandler OnSaveSettingsGraphicsData;

	public delegate void ResetPlayerPrefsSettingsEventHandler();
	public event ResetPlayerPrefsSettingsEventHandler OnResetSettingsGraphicsData;

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		ViewModelPauseSubMenuSettingsSectionGraphics viewModelPauseSubMenuSettingsSectionGraphics)
	{
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;

		TEXT_NO_GRAPHICS_SETTINGS_YET = viewModelPauseSubMenuSettingsSectionGraphics.TEXT_NO_GRAPHICS_SETTINGS_YET;
		TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET = viewModelPauseSubMenuSettingsSectionGraphics.TEXT_NO_GRAPHICS_SETTINGS_YET.GetNodeOrNull<Label>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_playerPrefsSettingsController.OnApplySettingsSectionGraphicsPlayerPrefs += ApplySystemLoadedSettings;

		GD.Print("SettingsSectionGraphicsController Initialized");
	}

	public void SaveSettingsGraphics()
	{
		var currentData = new PlayerPrefsData();

		OnSaveSettingsGraphicsData?.Invoke(currentData);
	}

	public void ResetSettingsGraphics()
	{
		OnResetSettingsGraphicsData?.Invoke();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{

		};

		OnSaveSettingsGraphicsData?.Invoke(defaultData);
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{

	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		TEXT_COMPONENT_NO_GRAPHICS_SETTINGS_YET.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGraphics_NO-GRAPHICS-SETTINGS-IN-DEMO-VERSION");
	}
}
