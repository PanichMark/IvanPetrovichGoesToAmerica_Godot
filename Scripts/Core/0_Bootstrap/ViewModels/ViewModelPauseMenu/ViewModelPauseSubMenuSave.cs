using Godot;
public class ViewModelPauseSubMenuSave
{
	public Node TextPauseSubMenuSave;

	public Node ButtonBoxCreateNewGameFile;
	public Node ButtonCreateNewGameFile;
	public Node TextButtonCreateNewGameFile;

	public Node[] ContainersSaveGameFile;

	public Node[] ButtonsRewriteGameFile;
	public Node[] TextGameFileMissionName;
	public Node[] TextGameFileSceneName;
	public Node[] TextGameFileDateAndTime;
	public Node[] ImageSceneGameFile;

	public Label[] TextGameFileSlotNumber;

	public Node[] ButtonsDeleteGameFile;
	public Node[] TextButtonsDeleteGameFile;

	public Node ButtonClosePauseSubMenuSave;
	public Node TextButtonClosePauseSubMenuSave;

	public Node Scrollbar;
	public Node ScrollbarHandle;

	public ViewModelPauseSubMenuSave(Bootstrap bootstrap, Node canvas)
	{
		ContainersSaveGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];

		ButtonsRewriteGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileSceneName = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileDateAndTime = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextGameFileMissionName = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		ImageSceneGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];

		TextGameFileSlotNumber = new Label[bootstrap.GameData.NumberOfSafeFileSlots];

		ButtonsDeleteGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];
		TextButtonsDeleteGameFile = new Node[bootstrap.GameData.NumberOfSafeFileSlots];

		TextPauseSubMenuSave = bootstrap.FindDeepNode(canvas, "TextPauseSubMenuSave");

		ButtonBoxCreateNewGameFile = bootstrap.FindDeepNode(canvas, "ButtonBoxCreateNewGameFile");
		ButtonCreateNewGameFile = bootstrap.FindDeepNode(canvas, "ButtonCreateNewGameFile");
		TextButtonCreateNewGameFile = bootstrap.FindDeepNode(canvas, "TextCreateNewGameFile");

		string textMissionNameGameFile = "TextMissionName";
		string textSceneNameGameFile = "TextSceneName";
		string textDateAndTimeGameFile = "TextDateAndTime";
		string imageSceneGameFile = "ImageSceneGameFile";

		string textButtonDeleteGameFileName = "TextButtonDeleteGameFile";

		for (int i = 0; i < bootstrap.GameData.NumberOfSafeFileSlots; i++)
		{
			string containerSaveGameFile = "ButtonSave" + (i + 1);

			string buttonRewriteGameFile = "ButtonSaveGameFile" + (i + 1);
			string buttonDeleteGameFileName = "ButtonDeleteGameFile" + (i + 1);

			ContainersSaveGameFile[i] = bootstrap.FindDeepNode(canvas, containerSaveGameFile);

			ButtonsRewriteGameFile[i] = bootstrap.FindDeepNode(canvas, buttonRewriteGameFile);
			TextGameFileSceneName[i] = bootstrap.FindDeepNode(ButtonsRewriteGameFile[i], textSceneNameGameFile);
			TextGameFileDateAndTime[i] = bootstrap.FindDeepNode(ButtonsRewriteGameFile[i], textDateAndTimeGameFile);
			TextGameFileMissionName[i] = bootstrap.FindDeepNode(ButtonsRewriteGameFile[i], textMissionNameGameFile);
			ImageSceneGameFile[i] = bootstrap.FindDeepNode(ButtonsRewriteGameFile[i], imageSceneGameFile);

			ButtonsDeleteGameFile[i] = bootstrap.FindDeepNode(canvas, buttonDeleteGameFileName);
			TextButtonsDeleteGameFile[i] = bootstrap.FindDeepNode(ButtonsDeleteGameFile[i], textButtonDeleteGameFileName);

			TextGameFileSlotNumber[i] = bootstrap.FindDeepNode(ButtonsRewriteGameFile[i], "TextSlotNumber").FindNodeOfType<Label>();
			TextGameFileSlotNumber[i].Text = $"{i + 1}";
		}

		ButtonClosePauseSubMenuSave = bootstrap.FindDeepNode(canvas, "ButtonClosePauseSubMenuSave");
		TextButtonClosePauseSubMenuSave = bootstrap.FindDeepNode(canvas, "TextButtonClosePauseSubMenuSave");

		Scrollbar = bootstrap.FindDeepNode(canvas, "Scrollbar");
		ScrollbarHandle = bootstrap.FindDeepNode(canvas, "ScrollbarHandle");
	}
}