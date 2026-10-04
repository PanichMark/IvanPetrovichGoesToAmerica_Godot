using Godot;
public class ViewModelMenuLockpickElectronic
{
	public Node[] ButtonsLockElectronic;
	public Node ButtonCloseMenuLockpickElectronic;
	public Node TextButtonCloseMenuLockpickElectronic;

	public ViewModelMenuLockpickElectronic(Bootstrap bootstrap, Node canvas)
	{
		ButtonsLockElectronic = new Node[]
		{
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic1"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic2"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic3"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic4"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic5"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic6"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic7"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic8"),
			bootstrap.FindDeepNode(canvas, "ButtonLockElectronic9")
		};
		ButtonCloseMenuLockpickElectronic = bootstrap.FindDeepNode(canvas, "ButtonExitLockpickElectronicMenu");
		TextButtonCloseMenuLockpickElectronic = bootstrap.FindDeepNode(canvas, "TextButtonExitLockpickElectronicMenu");
	}
}