using Godot;
public class ViewModelPauseSubMenuSettingsSectionControls
{
	public Node SliderMouseSensitivityX;
	public Node NumberSliderMouseSensitivityX;
	public Node TextSliderMouseSensitivityX;

	public Node SliderMouseSensitivityY;
	public Node NumberSliderMouseSensitivityY;
	public Node TextSliderMouseSensitivityY;

	public Node[] InputFieldsControls = new Node[16];
	public Node[] TextControls = new Node[16];

	public Node Scrollbar;

	public ViewModelPauseSubMenuSettingsSectionControls(Bootstrap bootstrap, Node canvas)
	{
		SliderMouseSensitivityX = bootstrap.FindDeepNode(canvas, "SliderMouseSensitivityX");
		NumberSliderMouseSensitivityX = bootstrap.FindDeepNode(canvas, "NumberMouseSensitivityX");
		TextSliderMouseSensitivityX = bootstrap.FindDeepNode(canvas, "TextMouseSensitivityX");

		SliderMouseSensitivityY = bootstrap.FindDeepNode(canvas, "SliderMouseSensitivityY");
		NumberSliderMouseSensitivityY = bootstrap.FindDeepNode(canvas, "NumberMouseSensitivityY");
		TextSliderMouseSensitivityY = bootstrap.FindDeepNode(canvas, "TextMouseSensitivityY");

		string[] nputFieldControlsNames = {
			"MoveForward",
			"MoveBackward",
			"MoveRight",
			"MoveLeft",
			"Run",
			"Jump",
			"Crouch",
			"Interact",
			"ChangeCameraView",
			"ChangeCameraShoulder",
			"WeaponWheelRightHand",
			"WeaponWheelLeftHand",
			"WeaponAttackRightHand",
			"WeaponAttackLeftHand",
			"WeaponReload",
			"LegKick"};

		for (int i = 0; i < nputFieldControlsNames.Length; i++)
		{
			InputFieldsControls[i] = bootstrap.FindDeepNode(canvas, $"KeyBinding_{nputFieldControlsNames[i]}");
		}

		for (int i = 0; i < nputFieldControlsNames.Length; i++)
		{
			TextControls[i] = bootstrap.FindDeepNode(canvas, $"TextControl{nputFieldControlsNames[i]}");
		}

		Scrollbar = bootstrap.FindDeepNode(canvas, "Scrollbar");
	}
}
