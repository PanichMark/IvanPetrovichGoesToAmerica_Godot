using Godot;

/// <summary>Applies camera blur/brightness effects through Godot's Environment resources.</summary>
public partial class PlayerCameraVolumeController : Node
{
	private MenuManager _menuManager;
	private WorldEnvironment _thirdPersonEnvironment;
	private WorldEnvironment _firstPersonEnvironment;
	private PauseSubMenuSettingsSectionGeneralController _settings;

	public void Initialize(MenuManager manager, PauseSubMenuSettingsSectionGeneralController settings, Node firstPersonCamera)
	{
		_menuManager = manager;
		_settings = settings;
		_thirdPersonEnvironment = GetParent()?.FindDeepNodeOfType<WorldEnvironment>();
		_firstPersonEnvironment = firstPersonCamera?.FindDeepNodeOfType<WorldEnvironment>();
		_menuManager.OnOpenAnyMenu += ActivateCameraBlur;
		_menuManager.OnCloseAnyMenu += DeactivateCameraBlur;
		_menuManager.OnOpenPauseMenu += ActivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedDialogueMenu += DeactivateCameraBlur;
		_menuManager.OnClosePauseMenuDuringOpenedCutsceneMenu += DeactivateCameraBlur;
		settings.OnScreenBrightnessChanged += ChangeCameraBrightness;
		DeactivateCameraBlur();
	}

	public override void _ExitTree()
	{
		if (_menuManager != null)
		{
			_menuManager.OnOpenAnyMenu -= ActivateCameraBlur;
			_menuManager.OnCloseAnyMenu -= DeactivateCameraBlur;
			_menuManager.OnOpenPauseMenu -= ActivateCameraBlur;
			_menuManager.OnClosePauseMenuDuringOpenedDialogueMenu -= DeactivateCameraBlur;
			_menuManager.OnClosePauseMenuDuringOpenedCutsceneMenu -= DeactivateCameraBlur;
		}
		if (_settings != null) _settings.OnScreenBrightnessChanged -= ChangeCameraBrightness;
	}

	public void ActivateCameraBlur() => SetBlurEnabled(true);
	public void DeactivateCameraBlur() => SetBlurEnabled(false);
	private void SetBlurEnabled(bool enabled)
	{
		SetFog(_thirdPersonEnvironment, enabled);
		SetFog(_firstPersonEnvironment, enabled);
	}
	private static void SetFog(WorldEnvironment environment, bool enabled)
	{
		if (environment?.Environment != null) environment.Environment.GlowEnabled = enabled;
	}
	public void ChangeCameraBrightness(int value)
	{
		float t = value / 100f * 2f - 1f;
		float exposure = Mathf.Clamp(Mathf.Sign(t) * (Mathf.Pow(Mathf.Abs(t) + 1f, 1.5f) - 1f), -3f, 3f);
		if (_thirdPersonEnvironment?.Environment != null) _thirdPersonEnvironment.Environment.AdjustmentEnabled = true;
		if (_firstPersonEnvironment?.Environment != null) _firstPersonEnvironment.Environment.AdjustmentEnabled = true;
		if (_thirdPersonEnvironment?.Environment != null) _thirdPersonEnvironment.Environment.AdjustmentBrightness = Mathf.Clamp(1f + exposure * 0.15f, 0f, 2f);
		if (_firstPersonEnvironment?.Environment != null) _firstPersonEnvironment.Environment.AdjustmentBrightness = Mathf.Clamp(1f + exposure * 0.15f, 0f, 2f);
	}
}