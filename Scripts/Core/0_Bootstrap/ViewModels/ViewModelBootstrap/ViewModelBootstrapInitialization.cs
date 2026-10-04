using Godot;
public class ViewModelBootstrapInitialization
{
	public Node BootstrapInitializationPart1;
	public Node BootstrapInitializationPart2;
	public Node BootstrapInitializationPart3;

	public Node TextSavingProcessIcon;
	public Node Gear;

	public ViewModelBootstrapInitialization(Bootstrap bootstrap, Node canvas)
	{
		BootstrapInitializationPart1 = bootstrap.FindDeepNode(canvas, "BootstrapInitializationPart1");
		BootstrapInitializationPart2 = bootstrap.FindDeepNode(canvas, "BootstrapInitializationPart2");
		BootstrapInitializationPart3 = bootstrap.FindDeepNode(canvas, "BootstrapInitializationPart3");

		TextSavingProcessIcon = bootstrap.FindDeepNode(canvas, "TextSavingProcessIcon");
		Gear = bootstrap.FindDeepNode(canvas, "Gear");
	}

}