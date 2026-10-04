using Godot;
public class ViewModelHUDMission
{
	public Node HUDmission;

	public Node ImageMissionGoalMarker;

	public Node TextNewMissionGoal;
	public Node TextNewMissionGoalDisplay;

	public ViewModelHUDMission(Bootstrap bootstrap, Node canvas)
	{
		HUDmission = bootstrap.FindDeepNode(canvas, "HUDmission");

		ImageMissionGoalMarker = bootstrap.FindDeepNode(canvas, "MissionGoalMarker");

		TextNewMissionGoal = bootstrap.FindDeepNode(canvas, "TextNewMissionGoal");
		TextNewMissionGoalDisplay = bootstrap.FindDeepNode(canvas, "TextNewMissionGoalDisplay");
	}
}