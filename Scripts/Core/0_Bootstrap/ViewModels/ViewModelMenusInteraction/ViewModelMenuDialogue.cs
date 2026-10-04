using Godot;
public class ViewModelMenuDialogue
{
	public Node TextDialogueLine;
	public Node ButtonDialogueYes;
	public Node ButtonDialogueNo;
	public Node TextDialogueYes;
	public Node TextDialogueNo;

	public ViewModelMenuDialogue(Bootstrap bootstrap, Node canvas)
	{
		
		TextDialogueLine = bootstrap.FindDeepNode(canvas, "TextDialogue");
		
		ButtonDialogueYes = bootstrap.FindDeepNode(canvas, "ButtonDialogueYes");
		ButtonDialogueNo = bootstrap.FindDeepNode(canvas, "ButtonDialogueNo");
		TextDialogueYes = bootstrap.FindDeepNode(canvas, "TextDialogueYes");
		TextDialogueNo = bootstrap.FindDeepNode(canvas, "TextDialogueNo");
	}
}