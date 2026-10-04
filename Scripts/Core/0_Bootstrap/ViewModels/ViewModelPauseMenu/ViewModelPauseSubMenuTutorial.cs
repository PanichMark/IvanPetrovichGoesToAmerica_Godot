using Godot;
public class ViewModelPauseSubMenuTutorial
{
	public Node ImageTutorial;
	public Node TextTutorial;

	public Node ButtonNextTutorial;
	public Node ButtonPreviousTutorial;

	public Node ButtonClosePauseSubMenuTutorial;
	public Node TextButtonClosePauseSubMenuTutorial;

	public ViewModelPauseSubMenuTutorial(Bootstrap bootstrap, Node canvas)
	{
		ImageTutorial = bootstrap.FindDeepNode(canvas, "ImageTutorial");
		TextTutorial = bootstrap.FindDeepNode(canvas, "TextTutorial");

		ButtonNextTutorial = bootstrap.FindDeepNode(canvas, "ButtonNextTutorial");
		ButtonPreviousTutorial = bootstrap.FindDeepNode(canvas, "ButtonPreviousTutorial");

		ButtonClosePauseSubMenuTutorial = bootstrap.FindDeepNode(canvas, "ButtonClosePauseSubMenuTutorial");
		TextButtonClosePauseSubMenuTutorial = bootstrap.FindDeepNode(canvas, "TextButtonClosePauseSubMenuTutorial");
	}
}