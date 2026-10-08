using Godot;
public partial class CutsceneMenuController : Node
{
	private MenuManager _menuManager;
	private Node _canvasCutscene;
	private GameScenesManager _gameSceneManager;

	public void Initialize(GameScenesManager gameSceneManager, MenuManager menuManager, Node canvasCutscene)
	{
		_menuManager = menuManager;

		_canvasCutscene = canvasCutscene;
		_gameSceneManager = gameSceneManager;

		_menuManager.OnOpenCutsceneMenu += ShowCanvasCutscene;
		_menuManager.OnCloseCutsceneMenu += HideCanvasCutscene;

		_menuManager.OnOpenPauseMenu += HideCanvasCutscene;
		_menuManager.OnClosePauseMenu += ShowCanvasCutscene;

		_gameSceneManager.OnBeginLoadingGameplayScene += HideCanvasCutscene;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasCutscene;

		GD.Print("CutsceneMenuController Initialized");
	}

	private void ShowCanvasCutscene()
	{
		if (_menuManager.IsCutsceneMenuOpened)
		{
			SetVisible(_canvasCutscene, true);
			GD.Print("Show CutsceneMenu");
		}
	}

	private void HideCanvasCutscene()
	{
		if (_menuManager.IsCutsceneMenuOpened)
		{
			SetVisible(_canvasCutscene, false);
			GD.Print("Hide CutsceneMenu");
		}
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}
}
