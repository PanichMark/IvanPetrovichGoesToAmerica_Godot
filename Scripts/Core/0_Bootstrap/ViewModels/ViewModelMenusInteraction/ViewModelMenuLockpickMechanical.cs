using Godot;
public class ViewModelMenuLockpickMechanical
{
	public Node ButtonCloseMenuLockpickMechanical;
	public Node TextButtonCloseMenuLockpickMechanical;
	public Node ButtonMoveLockMechanismUp;
	public Node ButtonMoveLockMechanismDown;
	public Node ButtonMoveLockMechanismRight;
	public Node ButtonMoveLockMechanismLeft;


	public ViewModelMenuLockpickMechanical(Bootstrap bootstrap, Node canvas)
	{
		ButtonCloseMenuLockpickMechanical = bootstrap.FindDeepNode(canvas, "ButtonExitLockpickMechanicalMenu");
		TextButtonCloseMenuLockpickMechanical = bootstrap.FindDeepNode(canvas, "TextButtonExitLockpickMechanicalMenu");

		ButtonMoveLockMechanismUp = bootstrap.FindDeepNode(canvas, "ButtonUp");
		ButtonMoveLockMechanismDown = bootstrap.FindDeepNode(canvas, "ButtonDown");
		ButtonMoveLockMechanismRight = bootstrap.FindDeepNode(canvas, "ButtonRight");
		ButtonMoveLockMechanismLeft = bootstrap.FindDeepNode(canvas, "ButtonLeft");
	}
}