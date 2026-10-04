using Godot;
public class ViewModelBootstrapChooseFirstLanguage
{
	public Node ButtonRussianLangauge;
	public Node ButtonEnglishLanguage;

	public ViewModelBootstrapChooseFirstLanguage(Bootstrap bootstrap, Node canvas)
	{
		ButtonRussianLangauge = bootstrap.FindDeepNode(canvas, "Russian");
		ButtonEnglishLanguage = bootstrap.FindDeepNode(canvas, "English");
	}
}
