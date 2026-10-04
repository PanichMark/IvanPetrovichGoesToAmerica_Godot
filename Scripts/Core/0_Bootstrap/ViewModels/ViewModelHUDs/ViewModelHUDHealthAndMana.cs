using Godot;
public class ViewModelHUDHealthAndMana
{
	public Node HealthBar;
	public Node SliderHealthBar;
	public Node ManaBar;
	public Node SliderManaBar;
	public Node SliderManaBarFillArea;
	public Node HUDhealthAndManaBars;
	

	public ViewModelHUDHealthAndMana(Bootstrap bootstrap, Node canvas)
	{
		HealthBar = bootstrap.FindDeepNode(canvas, "HealthBar");
		SliderHealthBar = bootstrap.FindDeepNode(canvas, "SliderHealthBar");

		ManaBar = bootstrap.FindDeepNode(canvas, "ManaBar");
		SliderManaBar = bootstrap.FindDeepNode(canvas, "SliderManaBar");
		SliderManaBarFillArea = bootstrap.FindDeepNode(canvas, "SliderManaBarFillArea");

		HUDhealthAndManaBars = bootstrap.FindDeepNode(canvas, "HUDhealthAndManaBars");
	}
}