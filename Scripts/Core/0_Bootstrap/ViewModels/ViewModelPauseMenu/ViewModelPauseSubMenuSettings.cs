using Godot;
public class ViewModelPauseSubMenuSettings
{
	public Node ButtonSaveGameSettings;
	public Node TextButtonSaveGameSettings;

	public Node ButtonResetGameSettings;
	public Node TextButtonResetGameSettings;

	public Node ButtonClosePauseSubMenuSettings;
	public Node TextButtonClosePauseSubMenuSettings;

	public Node SubSettingsSectionGeneral;
	public Node ImageBackgroundSectionGeneral;
	public Node ButtonSubSettingsSectionGeneral;
	public Node TextButtonSubSettingsSectionGeneral;

	public Node SubSettingsSectionControls;
	public Node ImageBackgroundSectionControls;
	public Node ButtonSubSettingsSectionControls;
	public Node TextButtonSubSettingsSectionControls;

	public Node SubSettingsSectionGraphics;
	public Node ImageBackgroundSectionGraphics;
	public Node ButtonSubSettingsSectionGraphics;
	public Node TextButtonSubSettingsSectionGraphics;

	public Node SubSettingsSectionAudio;
	public Node ImageBackgroundSectionAudio;
	public Node ButtonSubSettingsSectionAudio;
	public Node TextButtonSubSettingsSectionAudio;

	public ViewModelPauseSubMenuSettings(Bootstrap bootstrap, Node canvas)
	{
		ButtonSaveGameSettings = bootstrap.FindDeepNode(canvas, "ButtonSaveGameSettings");
		TextButtonSaveGameSettings = bootstrap.FindDeepNode(canvas, "TextButtonSaveGameSettings");

		ButtonResetGameSettings = bootstrap.FindDeepNode(canvas, "ButtonResetGameSettings");
		TextButtonResetGameSettings = bootstrap.FindDeepNode(canvas, "TextButtonResetGameSettings");

		ButtonClosePauseSubMenuSettings = bootstrap.FindDeepNode(canvas, "ButtonClosePauseSubMenuSettings");
		TextButtonClosePauseSubMenuSettings = bootstrap.FindDeepNode(canvas, "TextButtonClosePauseSubMenuSettings");

		SubSettingsSectionGeneral = bootstrap.FindDeepNode(canvas, "SubSettingsSectionGeneral");
		ImageBackgroundSectionGeneral = bootstrap.FindDeepNode(canvas, "ImageBackgroundSectionGeneral");
		ButtonSubSettingsSectionGeneral = bootstrap.FindDeepNode(canvas, "ButtonSubSettingsSectionGeneral");
		TextButtonSubSettingsSectionGeneral = bootstrap.FindDeepNode(canvas, "TextButtonSubSettingsSectionGeneral");

		SubSettingsSectionControls = bootstrap.FindDeepNode(canvas, "SubSettingsSectionControls");
		ImageBackgroundSectionControls = bootstrap.FindDeepNode(canvas, "ImageBackgroundSectionControls");
		ButtonSubSettingsSectionControls = bootstrap.FindDeepNode(canvas, "ButtonSubSettingsSectionControls");
		TextButtonSubSettingsSectionControls = bootstrap.FindDeepNode(canvas, "TextButtonSubSettingsSectionControls");

		SubSettingsSectionGraphics = bootstrap.FindDeepNode(canvas, "SubSettingsSectionGraphics");
		ImageBackgroundSectionGraphics = bootstrap.FindDeepNode(canvas, "ImageBackgroundSectionGraphics");
		ButtonSubSettingsSectionGraphics = bootstrap.FindDeepNode(canvas, "ButtonSubSettingsSectionGraphics");
		TextButtonSubSettingsSectionGraphics = bootstrap.FindDeepNode(canvas, "TextButtonSubSettingsSectionGraphics");

		SubSettingsSectionAudio = bootstrap.FindDeepNode(canvas, "SubSettingsSectionAudio");
		ImageBackgroundSectionAudio = bootstrap.FindDeepNode(canvas, "ImageBackgroundSectionAudio");
		ButtonSubSettingsSectionAudio = bootstrap.FindDeepNode(canvas, "ButtonSubSettingsSectionAudio");
		TextButtonSubSettingsSectionAudio = bootstrap.FindDeepNode(canvas, "TextButtonSubSettingsSectionAudio");
	}
}