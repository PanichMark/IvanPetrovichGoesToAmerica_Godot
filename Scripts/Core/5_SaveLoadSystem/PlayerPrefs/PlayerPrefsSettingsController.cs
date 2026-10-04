using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>Godot ConfigFile-backed replacement for Unity PlayerPrefs settings.</summary>
public partial class PlayerPrefsSettingsController : Node
{
	private const string SettingsPath = "user://settings.cfg";
	private const string SettingsSection = "Settings";
	private const string ControlsSection = "Controls";
	private const string BootstrapSection = "Bootstrap";
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private readonly ConfigFile _config = new();

	public delegate void PlayerPrefsSettingsHandler(PlayerPrefsData data);
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionGeneralPlayerPrefs;
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionControlsPlayerPrefs;
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionGraphicsPlayerPrefs;
	public event PlayerPrefsSettingsHandler OnApplySettingsSectionAudioPlayerPrefs;

	public string ScreenResolution { get; } = PlayerPrefsSettingsSectionGeneralEnum.ScreenResolution.ToString();
	public string WindowType { get; } = PlayerPrefsSettingsSectionGeneralEnum.WindowType.ToString();
	public string FPSlimit { get; } = PlayerPrefsSettingsSectionGeneralEnum.FPSlimit.ToString();
	public string HUDtype { get; } = PlayerPrefsSettingsSectionGeneralEnum.HUDtype.ToString();
	public string WeaponWheelType { get; } = PlayerPrefsSettingsSectionGeneralEnum.WeaponWheelType.ToString();
	public string CameraFOV { get; } = PlayerPrefsSettingsSectionGeneralEnum.CameraFOV.ToString();
	public string ScreenBrightness { get; } = PlayerPrefsSettingsSectionGeneralEnum.ScreenBrightness.ToString();
	public string ShowIngameTutorials { get; } = PlayerPrefsSettingsSectionGeneralEnum.ShowIngameTutorials.ToString();
	public string ShowBlood { get; } = PlayerPrefsSettingsSectionGeneralEnum.ShowBlood.ToString();
	public string KeyBindingPrefix { get; } = "KeyBinding_";
	public string MouseSensitivityX { get; } = PlayerPrefsSettingsSectionControlsEnum.MouseSensitivityX.ToString();
	public string MouseSensitivityY { get; } = PlayerPrefsSettingsSectionControlsEnum.MouseSensitivityY.ToString();
	public string Language { get; } = PlayerPrefsSettingsSectionAudioEnum.Language.ToString();
	public string VolumeGeneral { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeGeneral.ToString();
	public string VolumeEnvironment { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeEnvironment.ToString();
	public string VolumeEffects { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeEffects.ToString();
	public string VolumeVoices { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeVoices.ToString();
	public string VolumeMusicAmbience { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeMusicAmbience.ToString();
	public string VolumeMusicIngame { get; } = PlayerPrefsSettingsSectionAudioEnum.VolumeMusicIngame.ToString();

	public void Initialize(Bootstrap bootstrap, IInputDevice inputDevice)
	{
		_bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
		_inputDevice = inputDevice ?? throw new ArgumentNullException(nameof(inputDevice));
		Error loadError = _config.Load(SettingsPath);
		if (loadError != Error.Ok && loadError != Error.FileNotFound)
			GD.PushWarning($"Could not read settings file '{SettingsPath}': {loadError}.");
		_bootstrap.OnLoadSettingsData -= LoadSettingsOnBootstrap;
		_bootstrap.OnLoadSettingsData += LoadSettingsOnBootstrap;
		GD.Print("PlayerPrefsSettingsController initialized.");
	}

	public bool BootstrapArePrerequisitesMet => _config.GetValue(BootstrapSection, PlayerPrefsGameInitializationEnum.BootstrapArePrerequisitesMet.ToString(), false).AsBool();

	public void SetBootstrapPrerequisitesMet()
	{
		_config.SetValue(BootstrapSection, PlayerPrefsGameInitializationEnum.BootstrapArePrerequisitesMet.ToString(), true);
		SaveConfig();
	}

	public void SaveSettingsGeneral(PlayerPrefsData data)
	{
		ArgumentNullException.ThrowIfNull(data);
		Set(SettingsSection, FPSlimit, data.FPSlimit);
		Set(SettingsSection, CameraFOV, data.CameraFOV);
		Set(SettingsSection, ScreenBrightness, data.ScreenBrightness);
		Set(SettingsSection, WeaponWheelType, data.WeaponWheelType);
		Set(SettingsSection, ShowIngameTutorials, data.ShowIngameTutorials);
		Set(SettingsSection, ShowBlood, data.ShowBlood);
		SaveConfig();
	}

	public void SaveSettingsControls(PlayerPrefsData data)
	{
		ArgumentNullException.ThrowIfNull(data);
		Set(ControlsSection, MouseSensitivityX, data.MouseSensitivityX);
		Set(ControlsSection, MouseSensitivityY, data.MouseSensitivityY);
		foreach ((InputControlsEnum action, Key key) in data.KeyBindings)
			Set(ControlsSection, KeyBindingPrefix + action, key.ToString());
		SaveConfig();
	}

	public void SaveSettingsGraphics(PlayerPrefsData data)
	{
		ArgumentNullException.ThrowIfNull(data);
		SaveConfig();
	}

	public void SaveSettingsAudio(PlayerPrefsData data)
	{
		ArgumentNullException.ThrowIfNull(data);
		Set(SettingsSection, Language, data.Language);
		Set(SettingsSection, VolumeGeneral, data.VolumeGeneral);
		Set(SettingsSection, VolumeEnvironment, data.VolumeEnvironment);
		Set(SettingsSection, VolumeEffects, data.VolumeEffects);
		Set(SettingsSection, VolumeVoices, data.VolumeVoices);
		Set(SettingsSection, VolumeMusicAmbience, data.VolumeMusicAmbience);
		Set(SettingsSection, VolumeMusicIngame, data.VolumeMusicIngame);
		SaveConfig();
	}

	public void LoadSettings(List<InputControlsEnum> actionNamesToLoad)
	{
		PlayerPrefsData data = new();
		data.SetBootstrapPrerequisitesMet(BootstrapArePrerequisitesMet);
		data.FPSlimit = GetInt(SettingsSection, FPSlimit, 60);
		data.CameraFOV = GetFloat(SettingsSection, CameraFOV, 60f);
		data.ScreenBrightness = GetFloat(SettingsSection, ScreenBrightness, 50f);
		data.WeaponWheelType = GetString(SettingsSection, WeaponWheelType, "_2D");
		data.ShowIngameTutorials = GetBool(SettingsSection, ShowIngameTutorials, true);
		data.ShowBlood = GetBool(SettingsSection, ShowBlood, true);
		data.MouseSensitivityX = GetFloat(ControlsSection, MouseSensitivityX, 1f);
		data.MouseSensitivityY = GetFloat(ControlsSection, MouseSensitivityY, 1f);

		IEnumerable<InputControlsEnum> actions = actionNamesToLoad ?? _inputDevice.GetDefaultKeyBindings().Keys;
		foreach (InputControlsEnum action in actions)
		{
			string savedKey = GetString(ControlsSection, KeyBindingPrefix + action, string.Empty);
			if (Enum.TryParse(savedKey, true, out Key key) && key != Key.None)
			{
				data.KeyBindings[action] = key;
				_inputDevice.RebindKey(action, key);
			}
		}

		data.Language = GetString(SettingsSection, Language, string.Empty);
		data.VolumeGeneral = GetInt(SettingsSection, VolumeGeneral, 50);
		data.VolumeEnvironment = GetInt(SettingsSection, VolumeEnvironment, 50);
		data.VolumeEffects = GetInt(SettingsSection, VolumeEffects, 50);
		data.VolumeVoices = GetInt(SettingsSection, VolumeVoices, 50);
		data.VolumeMusicAmbience = GetInt(SettingsSection, VolumeMusicAmbience, 50);
		data.VolumeMusicIngame = GetInt(SettingsSection, VolumeMusicIngame, 50);

		OnApplySettingsSectionGeneralPlayerPrefs?.Invoke(data);
		OnApplySettingsSectionControlsPlayerPrefs?.Invoke(data);
		OnApplySettingsSectionGraphicsPlayerPrefs?.Invoke(data);
		OnApplySettingsSectionAudioPlayerPrefs?.Invoke(data);
	}

	public void ResetSettingsGeneral()
	{
		foreach (string key in new[] { FPSlimit, WeaponWheelType, CameraFOV, ScreenBrightness, ShowIngameTutorials, ShowBlood })
			_config.EraseSectionKey(SettingsSection, key);
		SaveConfig();
	}

	public void ResetSettingsControls()
	{
		_config.EraseSectionKey(ControlsSection, MouseSensitivityX);
		_config.EraseSectionKey(ControlsSection, MouseSensitivityY);
		foreach (InputControlsEnum action in _inputDevice.GetDefaultKeyBindings().Keys)
			_config.EraseSectionKey(ControlsSection, KeyBindingPrefix + action);
		foreach ((InputControlsEnum action, Key key) in _inputDevice.GetDefaultKeyBindings())
			_inputDevice.RebindKey(action, key);
		SaveConfig();
	}

	public void ResetSettingsGraphics() => SaveConfig();

	public void ResetSettingsAudio()
	{
		foreach (string key in new[] { VolumeGeneral, VolumeEnvironment, VolumeEffects, VolumeVoices, VolumeMusicAmbience, VolumeMusicIngame })
			_config.EraseSectionKey(SettingsSection, key);
		SaveConfig();
	}

	public void ResetSettings()
	{
		_config.Clear();
		foreach ((InputControlsEnum action, Key key) in _inputDevice.GetDefaultKeyBindings())
			_inputDevice.RebindKey(action, key);
		SaveConfig();
	}

	private void LoadSettingsOnBootstrap() => LoadSettings(_inputDevice.GetDefaultKeyBindings().Keys.ToList());
	private void Set(string section, string key, object value)
	{
		Variant variant = value switch
		{
			string text => Variant.From(text),
			int integer => Variant.From(integer),
			float number => Variant.From(number),
			bool boolean => Variant.From(boolean),
			_ => throw new ArgumentException($"Unsupported settings value type: {value?.GetType().Name ?? "null"}.", nameof(value))
		};
		_config.SetValue(section, key, variant);
	}
	private string GetString(string section, string key, string fallback) => _config.GetValue(section, key, fallback).AsString();
	private int GetInt(string section, string key, int fallback) => (int)_config.GetValue(section, key, fallback).AsInt32();
	private float GetFloat(string section, string key, float fallback) => (float)_config.GetValue(section, key, fallback).AsDouble();
	private bool GetBool(string section, string key, bool fallback) => _config.GetValue(section, key, fallback).AsBool();
	private void SaveConfig()
	{
		Error error = _config.Save(SettingsPath);
		if (error != Error.Ok)
			GD.PushError($"Could not write settings file '{SettingsPath}': {error}.");
	}
}