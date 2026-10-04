using Godot;
public class ViewModelPauseSubMenuSettingsSectionGeneral
{
	public Node DropdownScreenResolution;
	public Node TextDropdownScreenResolution;

	public Node DropdownWindowType;
	public Node TextDropdownWindowType;

	public Node DropdownLimitFPS;
	public Node TextDropdownLimitFPS;

	public Node DropdownHUDType;
	public Node TextDropdownHUDType;

	public Node DropdownWeaponWheelType;
	public Node TextDropdownWeaponWheelType;

	public Node SliderCameraFOV;
	public Node NumberSliderCameraFOV;
	public Node TextSliderCameraFOV;

	public Node SliderScreenBrightness;
	public Node NumberSliderScreenBrightness;
	public Node TextSliderScreenBrightness;

	public Node ButtonGameDifficulty;
	public Node TextButtonGameDifficulty;
	public Node TextButtonDifficultyNormal;

	public Node ToggleShowIngameHints;
	public Node TextToggleShowIngameHints;

	public Node ToggleShowBlood;
	public Node TextToggleShowBlood;

	public ViewModelPauseSubMenuSettingsSectionGeneral(Bootstrap bootstrap, Node canvas)
	{
		DropdownScreenResolution = bootstrap.FindDeepNode(canvas, "DropdownScreenResolution");
		TextDropdownScreenResolution = bootstrap.FindDeepNode(canvas, "TextScreenResolution");

		DropdownWindowType = bootstrap.FindDeepNode(canvas, "DropdownWindowType");
		TextDropdownWindowType = bootstrap.FindDeepNode(canvas, "TextWindowType");

		DropdownLimitFPS = bootstrap.FindDeepNode(canvas, "DropdownLimitFPS");
		TextDropdownLimitFPS = bootstrap.FindDeepNode(canvas, "TextLimitFPS");

		DropdownHUDType = bootstrap.FindDeepNode(canvas, "DropdownHUDtype");
		TextDropdownHUDType = bootstrap.FindDeepNode(canvas, "TextHUDtype");

		DropdownWeaponWheelType = bootstrap.FindDeepNode(canvas, "DropdownWeaponWheelType");
		TextDropdownWeaponWheelType = bootstrap.FindDeepNode(canvas, "TextWeaponWheelType");

		SliderCameraFOV = bootstrap.FindDeepNode(canvas, "SliderCameraFOV");
		NumberSliderCameraFOV = bootstrap.FindDeepNode(canvas, "NumberCameraFOV");
		TextSliderCameraFOV = bootstrap.FindDeepNode(canvas, "TextCameraFOV");

		SliderScreenBrightness = bootstrap.FindDeepNode(canvas, "SliderScreenBrightness");
		NumberSliderScreenBrightness = bootstrap.FindDeepNode(canvas, "NumberScreenBrightness");
		TextSliderScreenBrightness = bootstrap.FindDeepNode(canvas, "TextScreenBrightness");

		ButtonGameDifficulty = bootstrap.FindDeepNode(canvas, "ButtonGameDifficulty");
		TextButtonGameDifficulty = bootstrap.FindDeepNode(canvas, "TextGameDifficulty");
		TextButtonDifficultyNormal = bootstrap.FindDeepNode(canvas, "TextButtonDifficultyNormal");

		ToggleShowIngameHints = bootstrap.FindDeepNode(canvas, "ToggleShowIngameHints");
		TextToggleShowIngameHints = bootstrap.FindDeepNode(canvas, "TextShowIngameHints");

		ToggleShowBlood = bootstrap.FindDeepNode(canvas, "ToggleShowBlood");
		TextToggleShowBlood = bootstrap.FindDeepNode(canvas, "TextShowBlood");
	}
}
