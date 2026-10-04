using System.Collections.Generic;
using Godot;

public sealed class PlayerPrefsData
{
	public bool BootstrapArePrerequisitesMet { get; private set; }
	public string ScreenResolution { get; set; } = string.Empty;
	public string WindowType { get; set; } = string.Empty;
	public int FPSlimit { get; set; } = 60;
	public string HUDtype { get; set; } = string.Empty;
	public string WeaponWheelType { get; set; } = "_2D";
	public float CameraFOV { get; set; } = 60f;
	public float ScreenBrightness { get; set; } = 50f;
	public bool ShowIngameTutorials { get; set; } = true;
	public bool ShowBlood { get; set; } = true;
	public Dictionary<InputControlsEnum, Key> KeyBindings { get; set; } = new();
	public float MouseSensitivityX { get; set; } = 1f;
	public float MouseSensitivityY { get; set; } = 1f;
	public string Language { get; set; } = string.Empty;
	public int VolumeGeneral { get; set; } = 50;
	public int VolumeEnvironment { get; set; } = 50;
	public int VolumeEffects { get; set; } = 50;
	public int VolumeVoices { get; set; } = 50;
	public int VolumeMusicAmbience { get; set; } = 50;
	public int VolumeMusicIngame { get; set; } = 50;

	internal void SetBootstrapPrerequisitesMet(bool value) => BootstrapArePrerequisitesMet = value;
}

public enum PlayerPrefsGameInitializationEnum
{
	BootstrapArePrerequisitesMet
}