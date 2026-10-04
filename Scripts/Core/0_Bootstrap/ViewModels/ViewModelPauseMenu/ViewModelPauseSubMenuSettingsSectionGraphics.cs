using Godot;
public class ViewModelPauseSubMenuSettingsSectionGraphics
{
	public Node TEXT_NO_GRAPHICS_SETTINGS_YET;

	public ViewModelPauseSubMenuSettingsSectionGraphics(Bootstrap bootstrap, Node canvas)
	{
		TEXT_NO_GRAPHICS_SETTINGS_YET = bootstrap.FindDeepNode(canvas, "TEXT_NO_GRAPHICS_SETTINGS_YET");
	}
}
