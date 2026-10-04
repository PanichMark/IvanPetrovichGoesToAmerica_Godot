using Godot;
public class ViewModelPauseSubMenuLoad
{
	public Node TextPauseSubMenuLoad;

	public Node[] ButtonsLoadGameFile;
	public Node[] TextGameFileMissionName;
	public Node[] TextGameFileSceneName;
	public Node[] TextGameFileDateAndTime;
	public Node[] ImageSceneGameFile;
	public Label[] TextGameFileSlotNumber;

	public Node ButtonClosePauseSubMenuLoad;
	public Node TextButtonClosePauseSubMenuLoad;

	public Node Scrollbar;
	public Node ScrollbarHandle;

	public ViewModelPauseSubMenuLoad(Bootstrap bootstrap, Node canvas)
	{
		ButtonsLoadGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileMissionName = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileSceneName = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileDateAndTime = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		ImageSceneGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];

		TextGameFileSlotNumber = new Label[bootstrap.GameData.NumberOfSafeFileSlots];

		TextPauseSubMenuLoad = bootstrap.FindDeepNode(canvas, "TextPauseSubMenuLoad");

		for (int i = 0; i < bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			string slotRootName = "ButtonLoadGameFile" + (i + 1);

			ButtonsLoadGameFile[i] = bootstrap.FindDeepNode(canvas, $"{slotRootName}");
			//Debug.Log(ButtonsLoadGameFile[i]);
			TextGameFileMissionName[i] = bootstrap.FindDeepNode(ButtonsLoadGameFile[i], "TextMissionName");
			TextGameFileSceneName[i] = bootstrap.FindDeepNode(ButtonsLoadGameFile[i], "TextSceneName");
			TextGameFileDateAndTime[i] = bootstrap.FindDeepNode(ButtonsLoadGameFile[i], "TextDateAndTime");
			ImageSceneGameFile[i] = bootstrap.FindDeepNode(ButtonsLoadGameFile[i], "ImageSceneGameFile");
			TextGameFileSlotNumber[i] = bootstrap.FindDeepNode(ButtonsLoadGameFile[i], "TextSlotNumber").FindNodeOfType<Label>();
			TextGameFileSlotNumber[i].Text = $"{i + 1}";
		}

		ButtonClosePauseSubMenuLoad = bootstrap.FindDeepNode(canvas, "ButtonClosePauseSubMenuLoad");
		TextButtonClosePauseSubMenuLoad = bootstrap.FindDeepNode(canvas, "TextButtonClosePauseSubMenuLoad");

		Scrollbar = bootstrap.FindDeepNode(canvas, "Scrollbar");
		ScrollbarHandle = bootstrap.FindDeepNode(canvas, "ScrollbarHandle");
	}
}