using Godot;
public class ViewModelPauseSubMenuSettingsGameDifficultyController
{
	public Node ImageGameDifficulty;
	public Node TextGameDifficultyHeader;
	public Node TextGameDifficultyDescription;

	public Node ButtonNextGameDifficulty;
	public Node ButtonPreviousGameDifficulty;

	public Node ButtonCloseSettingsGameDifficulty;
	public Node TextButtonCloseSettingsGameDifficulty;

	public Node DifficultyNotAvailable;
	public Node TextDifficultyNotAvailable;

	public ViewModelPauseSubMenuSettingsGameDifficultyController(Bootstrap bootstrap, Node canvas)
	{
		ImageGameDifficulty = bootstrap.FindDeepNode(canvas, "ImageGameDifficulty");
		TextGameDifficultyHeader = bootstrap.FindDeepNode(canvas, "TextGameDifficultyHeader");
		TextGameDifficultyDescription = bootstrap.FindDeepNode(canvas, "TextGameDifficultyDescription");

		ButtonNextGameDifficulty = bootstrap.FindDeepNode(canvas, "ButtonNextGameDifficulty");
		ButtonPreviousGameDifficulty = bootstrap.FindDeepNode(canvas, "ButtonPreviousGameDifficulty");

		ButtonCloseSettingsGameDifficulty = bootstrap.FindDeepNode(canvas, "ButtonCloseSettingsGameDifficulty");
		TextButtonCloseSettingsGameDifficulty = bootstrap.FindDeepNode(canvas, "TextButtonCloseSettingsGameDifficulty");

		DifficultyNotAvailable = bootstrap.FindDeepNode(canvas, "DifficultyNotAvailable");
		TextDifficultyNotAvailable = bootstrap.FindDeepNode(canvas, "TextDifficultyNotAvailable");
	}
}