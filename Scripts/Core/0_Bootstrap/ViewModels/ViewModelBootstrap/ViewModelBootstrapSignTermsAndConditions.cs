using Godot;
public class ViewModelBootstrapSignTermsAndConditions
{
	public Node TextHeaderTermsAndConditions;
	public Node TextTermsAndConditions;

	public Node ButtonSign;
	public Node TextButtonSign;
	public Node ButtonRefuse;
	public Node TextButtonRefuse;

	public Node ToggleAgreeWithTerms;
	public Node TextToggleAgreeWithTerms;

	public ViewModelBootstrapSignTermsAndConditions(Bootstrap bootstrap, Node canvas)
	{
		TextHeaderTermsAndConditions = bootstrap.FindDeepNode(canvas, "TextHeaderTermsAndConditions");
		TextTermsAndConditions = bootstrap.FindDeepNode(canvas, "TextTermsAndConditions");

		ButtonSign = bootstrap.FindDeepNode(canvas, "ButtonSign");
		TextButtonSign = bootstrap.FindDeepNode(canvas, "TextButtonSign");
		ButtonRefuse = bootstrap.FindDeepNode(canvas, "ButtonRefuse");
		TextButtonRefuse = bootstrap.FindDeepNode(canvas, "TextButtonRefuse");

		ToggleAgreeWithTerms = bootstrap.FindDeepNode(canvas, "ToggleAgreeWithTerms");
		TextToggleAgreeWithTerms = bootstrap.FindDeepNode(canvas, "TextToggleAgreeWithTerms");
	}
}
