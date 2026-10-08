using Godot;
public partial class PauseSubMenuSettingsSectionAudioController : Node
{
	private Bootstrap _bootstrap;
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;
	private AudioBusLayout _audioBusLayout;

	private Node[] _buttonsChangeLanguage;
	private Button[] _buttonsComponentsChangeLanguage;
	private Node _textChangeLanguage;
	private Label _textComponentChangeLanguage;

	private Node _sliderVolumeGeneral;
	private HSlider _sliderComponentVolumeGeneral;
	private float _currentValueVolumeGeneral;
	private Node _textNumberSliderVolumeGeneral;
	private Label _textComponentNumberSliderVolumeGeneral;
	private Node _textSliderVolumeGeneral;
	private Label _textComponentSliderVolumeGeneral;

	private Node _sliderVolumeEnvironment;
	private HSlider _sliderComponentVolumeEnvironment;
	private float _currentValueVolumeEnvironment;
	private Node _textNumberSliderVolumeEnvironment;
	private Label _textComponentNumberSliderVolumeEnvironment;
	private Node _textSliderVolumeEnvironment;
	private Label _textComponentSliderVolumeEnvironment;

	private Node _sliderVolumeEffects;
	private HSlider _sliderComponentVolumeEffects;
	private float _currentValueVolumeEffects;
	private Node _textNumberSliderVolumeEffects;
	private Label _textComponentNumberSliderVolumeEffects;
	private Node _textSliderVolumeEffects;
	private Label _textComponentSliderVolumeEffects;

	private Node _sliderVolumeVoices;
	private HSlider _sliderComponentVolumeVoices;
	private float _currentValueVolumeVoices;
	private Node _textNumberSliderVolumeVoices;
	private Label _textComponentNumberSliderVolumeVoices;
	private Node _textSliderVolumeVoices;
	private Label _textComponentSliderVolumeVoices;

	private Node _sliderVolumeMusicAmbience;
	private HSlider _sliderComponentVolumeMusicAmbience;
	private float _currentValueVolumeMusicAmbience;
	private Node _textNumberSliderVolumeMusicAmbience;
	private Label _textComponentNumberSliderVolumeMusicAmbience;
	private Node _textSliderVolumeMusicAmbience;
	private Label _textComponentSliderVolumeMusicAmbience;

	private Node _sliderVolumeMusicIngame;
	private HSlider _sliderComponentVolumeMusicIngame;
	private float _currentValueVolumeMusicIngame;
	private Node _textNumberSliderVolumeMusicIngame;
	private Label _textComponentNumberSliderVolumeMusicIngame;
	private Node _textSliderVolumeMusicIngame;
	private Label _textComponentSliderVolumeMusicIngame;

	private const float _MIN_VALUE_VOLUME = 0f;
	private const float _MAX_VALUE_VOLUME = 100f;
	private const float _DEFAULT_VALUE_VOLUME = 50f;

	/*
	public delegate void VolumeEventHandle(float newVolumeValue, float MIN_VALUE_VOLUME, float MAX_VALUE_VOLUME);
	public event VolumeEventHandle OnVolumeGeneralChanged;
	public event VolumeEventHandle OnVolumeEnvironmentChanged;
	public event VolumeEventHandle OnVolumeEffectsChanged;
	public event VolumeEventHandle OnVolumeVoicesChanged;
	public event VolumeEventHandle OnVolumeMusicAmbienceChanged;
	public event VolumeEventHandle OnVolumeMusicIngameChanged;
	*/

	private PlayerPrefsSettingsController _playerPrefsSettingsController;

	public void Initialize(
		Bootstrap bootstrap,
		LocalizationManager localizationManager,
		PlayerPrefsSettingsController playerPrefsSettingsController,
		PauseMenuController pauseMenuController,
		ViewModelPauseSubMenuSettingsSectionAudio viewModelPauseSubMenuSettingsAudio,
		AudioBusLayout audioBusLayout)
	{
		_bootstrap = bootstrap;
		_localizationManager = localizationManager;
		_playerPrefsSettingsController = playerPrefsSettingsController;
		_pauseMenuController = pauseMenuController;
		_audioBusLayout = audioBusLayout;
		if (_audioBusLayout != null)
			AudioServer.SetBusLayout(_audioBusLayout);

		_buttonsChangeLanguage = new Node[viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage.Length];
		_buttonsComponentsChangeLanguage = new Button[viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage.Length];
		for (int i = 0; i < _buttonsChangeLanguage.Length; i++)
		{
			_buttonsComponentsChangeLanguage[i] = viewModelPauseSubMenuSettingsAudio.ButtonsChangeLanguage[i].GetNodeOrNull<Button>();
		}
		_buttonsComponentsChangeLanguage[0].Pressed += () => ChangeLanguage(LanguagesEnum.Russian);
		_buttonsComponentsChangeLanguage[1].Pressed += () => ChangeLanguage(LanguagesEnum.English);
		_textChangeLanguage = viewModelPauseSubMenuSettingsAudio.TextChangeLanguage;
		_textComponentChangeLanguage = viewModelPauseSubMenuSettingsAudio.TextChangeLanguage.GetNodeOrNull<Label>();

		_sliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.SliderVolumeGeneral;
		_sliderComponentVolumeGeneral = viewModelPauseSubMenuSettingsAudio.SliderVolumeGeneral.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeGeneral.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeGeneral.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeGeneral.onValueChanged.AddListener(SetVolumeGeneral);
		_textNumberSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeGeneral;
		_textComponentNumberSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeGeneral.GetNodeOrNull<Label>();
		_textSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeGeneral;
		_textComponentSliderVolumeGeneral = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeGeneral.GetNodeOrNull<Label>();

		_sliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.SliderVolumeEnvironment;
		_sliderComponentVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.SliderVolumeEnvironment.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeEnvironment.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.onValueChanged.AddListener(SetVolumeEnvironment);
		_textNumberSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEnvironment;
		_textComponentNumberSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEnvironment.GetNodeOrNull<Label>();
		_textSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEnvironment;
		_textComponentSliderVolumeEnvironment = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEnvironment.GetNodeOrNull<Label>();

		_sliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.SliderVolumeEffects;
		_sliderComponentVolumeEffects = viewModelPauseSubMenuSettingsAudio.SliderVolumeEffects.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeEffects.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeEffects.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeEffects.onValueChanged.AddListener(SetVolumeEffects);
		_textNumberSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEffects;
		_textComponentNumberSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeEffects.GetNodeOrNull<Label>();
		_textSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEffects;
		_textComponentSliderVolumeEffects = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeEffects.GetNodeOrNull<Label>();

		_sliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.SliderVolumeVoices;
		_sliderComponentVolumeVoices = viewModelPauseSubMenuSettingsAudio.SliderVolumeVoices.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeVoices.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeVoices.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeVoices.onValueChanged.AddListener(SetVolumeVoices);
		_textNumberSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeVoices;
		_textComponentNumberSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeVoices.GetNodeOrNull<Label>();
		_textSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeVoices;
		_textComponentSliderVolumeVoices = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeVoices.GetNodeOrNull<Label>();

		_sliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicAmbience;
		_sliderComponentVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicAmbience.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeMusicAmbience.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.onValueChanged.AddListener(SetVolumeMusicAmbience);
		_textNumberSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicAmbience;
		_textComponentNumberSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicAmbience.GetNodeOrNull<Label>();
		_textSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicAmbience;
		_textComponentSliderVolumeMusicAmbience = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicAmbience.GetNodeOrNull<Label>();

		_sliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicIngame;
		_sliderComponentVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.SliderVolumeMusicIngame.GetNodeOrNull<HSlider>();
		_sliderComponentVolumeMusicIngame.minValue = _MIN_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.maxValue = _MAX_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.onValueChanged.AddListener(SetVolumeMusicIngame);
		_textNumberSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicIngame;
		_textComponentNumberSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.NumberSliderVolumeMusicIngame.GetNodeOrNull<Label>();
		_textSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicIngame;
		_textComponentSliderVolumeMusicIngame = viewModelPauseSubMenuSettingsAudio.TextSliderVolumeMusicIngame.GetNodeOrNull<Label>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_playerPrefsSettingsController.OnApplySettingsSectionGeneralPlayerPrefs += ApplySystemLoadedSettings;

		GD.Print("SettingsSectionAudioController Initialized");
	}

	public void ApplySystemLoadedSettings(PlayerPrefsData data)
	{
		SetVolumeGeneral(data.VolumeGeneral);
		_sliderComponentVolumeGeneral.value = data.VolumeGeneral;

		SetVolumeEnvironment(data.VolumeEnvironment);
		_sliderComponentVolumeEnvironment.value = data.VolumeEnvironment;

		SetVolumeEffects(data.VolumeEffects);
		_sliderComponentVolumeEffects.value = data.VolumeEffects;

		SetVolumeVoices(data.VolumeVoices);
		_sliderComponentVolumeVoices.value = data.VolumeVoices;

		SetVolumeMusicAmbience(data.VolumeMusicAmbience);
		_sliderComponentVolumeMusicAmbience.value = data.VolumeMusicAmbience;

		SetVolumeMusicIngame(data.VolumeMusicIngame);
		_sliderComponentVolumeMusicIngame.value = data.VolumeMusicIngame;
	}

	private void ChangeLanguage(LanguagesEnum language)
	{
		_bootstrap.ChangeLanguage(language);
		GD.Print("Changed Language to: " + language);
	}

	public void SetVolumeGeneral(float newVolumeGeneral)
	{
		_currentValueVolumeGeneral = newVolumeGeneral;
		_textComponentNumberSliderVolumeGeneral.text = ((int)newVolumeGeneral).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.Master, newVolumeGeneral);
		//OnVolumeGeneralChanged?.Invoke(newVolumeGeneral, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEnvironment(float newVolumeEnvironment)
	{
		_currentValueVolumeEnvironment = newVolumeEnvironment;
		_textComponentNumberSliderVolumeEnvironment.text = ((int)newVolumeEnvironment).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeEnvironment, newVolumeEnvironment);
		//OnVolumeEnvironmentChanged?.Invoke(newVolumeEnvironment, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeEffects(float newVolumeEffects)
	{
		_currentValueVolumeEffects = newVolumeEffects;
		_textComponentNumberSliderVolumeEffects.text = ((int)newVolumeEffects).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeEffects, newVolumeEffects);
		//OnVolumeEffectsChanged?.Invoke(newVolumeEffects, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeVoices(float newVolumeVoices)
	{
		_currentValueVolumeVoices = newVolumeVoices;
		_textComponentNumberSliderVolumeVoices.text = ((int)newVolumeVoices).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeVoices, newVolumeVoices);
		//OnVolumeVoicesChanged?.Invoke(newVolumeVoices, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicAmbience(float newVolumeMusicAmbience)
	{
		_currentValueVolumeMusicAmbience = newVolumeMusicAmbience;
		_textComponentNumberSliderVolumeMusicAmbience.text = ((int)newVolumeMusicAmbience).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeMusicAmbience, newVolumeMusicAmbience);
		//OnVolumeMusicAmbienceChanged?.Invoke(newVolumeMusicAmbience, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SetVolumeMusicIngame(float newVolumeMusicIngame)
	{
		_currentValueVolumeMusicIngame = newVolumeMusicIngame;
		_textComponentNumberSliderVolumeMusicIngame.text = ((int)newVolumeMusicIngame).ToString();
		ApplyMixerVolume(AudioMixerGroupsEnum.VolumeMusicIngame, newVolumeMusicIngame);
		//OnVolumeMusicIngameChanged?.Invoke(newVolumeMusicIngame, _MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME);
	}

	public void SaveSettingsAudio()
	{
		var currentData = new PlayerPrefsData();

		currentData.Language = _localizationManager.CurrentLanguage.ToString();
		currentData.VolumeGeneral = (int)_currentValueVolumeGeneral;
		currentData.VolumeEnvironment = (int)_currentValueVolumeEnvironment;
		currentData.VolumeEffects = (int)_currentValueVolumeEffects;
		currentData.VolumeVoices = (int)_currentValueVolumeVoices;
		currentData.VolumeMusicAmbience = (int)_currentValueVolumeMusicAmbience;
		currentData.VolumeMusicIngame = (int)_currentValueVolumeMusicIngame;

		_playerPrefsSettingsController.SaveSettingsAudio(currentData);
	}

	public void ResetSettingsAudio()
	{
		_playerPrefsSettingsController.ResetSettingsAudio();

		PlayerPrefsData defaultData = new PlayerPrefsData
		{
			Language = _localizationManager.CurrentLanguage.ToString(),
			VolumeGeneral = (int)_DEFAULT_VALUE_VOLUME,
			VolumeEnvironment = (int)_DEFAULT_VALUE_VOLUME,
			VolumeEffects = (int)_DEFAULT_VALUE_VOLUME,
			VolumeVoices = (int)_DEFAULT_VALUE_VOLUME,
			VolumeMusicAmbience = (int)_DEFAULT_VALUE_VOLUME,
			VolumeMusicIngame = (int)_DEFAULT_VALUE_VOLUME,
		};

		_playerPrefsSettingsController.SaveSettingsAudio(defaultData);

		_sliderComponentVolumeGeneral.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeEnvironment.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeEffects.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeVoices.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeMusicAmbience.value = _DEFAULT_VALUE_VOLUME;
		_sliderComponentVolumeMusicIngame.value = _DEFAULT_VALUE_VOLUME;

		SetVolumeGeneral(_DEFAULT_VALUE_VOLUME);
		SetVolumeEnvironment(_DEFAULT_VALUE_VOLUME);
		SetVolumeEffects(_DEFAULT_VALUE_VOLUME);
		SetVolumeVoices(_DEFAULT_VALUE_VOLUME);
		SetVolumeMusicAmbience(_DEFAULT_VALUE_VOLUME);
		SetVolumeMusicIngame(_DEFAULT_VALUE_VOLUME);
	}

	private void ApplyMixerVolume(AudioMixerGroupsEnum group, float value)
	{
		float normalized = Mathf.InverseLerp(_MIN_VALUE_VOLUME, _MAX_VALUE_VOLUME, value);
		float db = (normalized > Mathf.Epsilon) ? Mathf.Log10(normalized) * 20f : -80f;
		int busIndex = AudioServer.GetBusIndex(group.ToString());
		if (busIndex < 0)
		{
			GD.PushWarning($"Audio bus '{group}' is not present in the active Godot audio bus layout.");
			return;
		}

		AudioServer.SetBusVolumeDb(busIndex, db);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentChangeLanguage.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextChangeLanguage");

		_textComponentSliderVolumeGeneral.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeGeneral");
		_textComponentSliderVolumeEnvironment.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeEnvironment");
		_textComponentSliderVolumeEffects.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeEffects");
		_textComponentSliderVolumeVoices.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeVoices");
		_textComponentSliderVolumeMusicAmbience.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeMusicAmbience");
		_textComponentSliderVolumeMusicIngame.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionAudio_TextSliderVolumeMusicIngame");
	}
}