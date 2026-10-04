using Godot;
public class ViewModelSavingProcess
{
	public Node Gear;

	public ViewModelSavingProcess(Bootstrap bootstrap, Node canvas)
	{
		Gear = bootstrap.FindDeepNode(canvas, "Gear");
	}
}
