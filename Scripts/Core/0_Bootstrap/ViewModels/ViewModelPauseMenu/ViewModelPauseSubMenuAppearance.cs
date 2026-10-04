using Godot;
public class ViewModelPauseSubMenuAppearance
{
	public Node ButtonClosePauseSubMenuAppearance;

	public ViewModelPauseSubMenuAppearance(Bootstrap bootstrap, Node canvas)
	{
		ButtonClosePauseSubMenuAppearance = bootstrap.FindDeepNode(canvas, "ButtonClosePauseSubMenuAppearance");
	}
}