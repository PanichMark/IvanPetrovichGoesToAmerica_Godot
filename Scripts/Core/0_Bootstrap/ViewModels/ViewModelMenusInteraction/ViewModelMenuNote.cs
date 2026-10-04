using Godot;
public class ViewModelMenuNote
{
	public Node TextNote;
	public Node ImageNote;
	public Node ImageNoteBlackBackground;
	public Node ButtonCloseMenuNote;
	public Node TextButtonCloseMenuNote;

	public ViewModelMenuNote(Bootstrap bootstrap, Node canvas)
	{
		ButtonCloseMenuNote = bootstrap.FindDeepNode(canvas, "ButtonExitReadNoteMenu");
		TextButtonCloseMenuNote = bootstrap.FindDeepNode(canvas, "TextButtonExitReadNoteMenu");
		ImageNote = bootstrap.FindDeepNode(canvas, "ImageNote");
		TextNote = bootstrap.FindDeepNode(canvas, "TextNote");
		ImageNoteBlackBackground = bootstrap.FindDeepNode(canvas, "ImageNoteBlackBackground");
	}
}