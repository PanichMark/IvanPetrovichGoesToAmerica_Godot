using Godot;
public class ViewModelMainMenuReadNews
{
	public Node ButtonCloseMainMenuReadNews;
	public Node TextButtonCloseMainMenuReadNews;

	public Node ButtonYouTube;
	public Node ButtonGitHub;
	public Node ButtonDTF;

	public Node TextReadNews;

	public ViewModelMainMenuReadNews(Bootstrap bootstrap, Node canvas)
	{
		ButtonCloseMainMenuReadNews = bootstrap.FindDeepNode(canvas, "ButtonCloseMainMenuReadNews");
		TextButtonCloseMainMenuReadNews = bootstrap.FindDeepNode(canvas, "TextButtonCloseMainMenuReadNews");

		ButtonYouTube = bootstrap.FindDeepNode(canvas, "YouTube");
		ButtonGitHub = bootstrap.FindDeepNode(canvas, "GitHub");
		ButtonDTF = bootstrap.FindDeepNode(canvas, "DTF");

		TextReadNews = bootstrap.FindDeepNode(canvas, "TextReadNews");
	}
}
