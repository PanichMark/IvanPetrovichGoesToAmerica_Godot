using Godot;
public class ViewModelMenuCutscene
{
	public Node TextCutsceneDialogue;

	public Node BlackLineUp;
	public Node BlackLineDown;

	public ViewModelMenuCutscene(Bootstrap bootstrap, Node canvas)
	{
		TextCutsceneDialogue = bootstrap.FindDeepNode(canvas, "TextCutsceneDialogue");

		BlackLineUp = bootstrap.FindDeepNode(canvas, "BlackLineUp");
		BlackLineDown = bootstrap.FindDeepNode(canvas, "BlackLineDown");
	}
}