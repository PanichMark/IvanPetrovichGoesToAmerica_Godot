using Godot;
public class ViewModelPauseMenu
{
	public Node[] ButtonsPauseMenu;
	public Node[] TextButtonsPauseMenu;

	public Node CurrentMission;
	public Node TextCurrentMissionGoal;
	public Node TextCurrentMissionGoalDisplay;

	public Node PlayerMoney;
	public Node TextCurrentPlayerMoney;
	public Node TextCurrentPlayerMoneyDisplay;

	public Node TextDeathMessage;

	public ViewModelPauseMenu(Bootstrap bootstrap, Node canvas)
	{
		ButtonsPauseMenu = new[]
		{
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuResume"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuSave"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuLoad"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuAppearance"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuTutorial"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuSettings"),
			bootstrap.FindDeepNode(canvas, "ButtonPauseMenuExitToMainMenu")
		};
		TextButtonsPauseMenu = new[]
		{
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuResume"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuSave"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuLoad"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuAppearance"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuTutorial"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuSettings"),
			bootstrap.FindDeepNode(canvas, "TextButtonPauseMenuExitToMainMenu")
		};

		CurrentMission = bootstrap.FindDeepNode(canvas, "CurrentMission");
		TextCurrentMissionGoal = bootstrap.FindDeepNode(canvas, "TextCurrentMissionGoal");
		TextCurrentMissionGoalDisplay = bootstrap.FindDeepNode(canvas, "TextCurrentMissionGoalDisplay");

		PlayerMoney = bootstrap.FindDeepNode(canvas, "PlayerMoney");
		TextCurrentPlayerMoney = bootstrap.FindDeepNode(canvas, "TextCurrentPlayerMoney");
		TextCurrentPlayerMoneyDisplay = bootstrap.FindDeepNode(canvas, "TextCurrentPlayerMoneyDisplay");

		TextDeathMessage = bootstrap.FindDeepNode(canvas, "TextDeathMessage");
	}
}