using Godot;
public class ViewModelMainMenuChooseMission
{
	public Node TextMainMenuChooseMission;

	public Node[] Missions = new Node[3];
	public Node[] TextsMissionsNames = new Node[3];
	public Node[] TextsScenesNames = new Node[3];

	public Node ButtonCloseMainMenuChooseMission;
	public Node TextButtonCloseMainMenuChooseMission;

	public ViewModelMainMenuChooseMission(Bootstrap bootstrap, Node canvas)
	{
		TextMainMenuChooseMission = bootstrap.FindDeepNode(canvas, "TextMainMenuChooseMission");

		for (int i = 0; i < Missions.Length; i++)
		{
			string missions = "Mission0." + (i + 1);
			string textsMissionsNames = "TextMissionName";
			string textsScenesNames = "TextSceneName";

			Missions[i] = bootstrap.FindDeepNode(canvas, missions);
			TextsMissionsNames[i] = bootstrap.FindDeepNode(Missions[i], textsMissionsNames);
			TextsScenesNames[i] = bootstrap.FindDeepNode(Missions[i], textsScenesNames);
		}

		ButtonCloseMainMenuChooseMission = bootstrap.FindDeepNode(canvas, "ButtonCloseMainMenuChooseMission");
		TextButtonCloseMainMenuChooseMission = bootstrap.FindDeepNode(canvas, "TextButtonCloseMainMenuChooseMission");
	}
}
