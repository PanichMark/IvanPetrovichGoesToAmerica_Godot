using Godot;
public class ViewModelPauseMenuConfirmAction
{
	public Node TextActionMessage;

	public Node ButtonConfirmAction;
	public Node TextButtonConfirmAction;

	public Node ButtonCancelAction;
	public Node TextButtonCancelAction;


	public ViewModelPauseMenuConfirmAction(Bootstrap bootstrap, Node canvas)
	{
		TextActionMessage = bootstrap.FindDeepNode(canvas, "TextActionMessage");

		ButtonConfirmAction = bootstrap.FindDeepNode(canvas, "ButtonConfirmAction");
		TextButtonConfirmAction = bootstrap.FindDeepNode(canvas, "TextButtonConfirmAction");

		ButtonCancelAction = bootstrap.FindDeepNode(canvas, "ButtonCancelAction");
		TextButtonCancelAction = bootstrap.FindDeepNode(canvas, "TextButtonCancelAction");
	}
}
