using Godot;
public partial class PauseSubMenuAppearanceController : Node
{
	private bool _isPauseSubMenuAppearanceOpened;
	private Node _canvasPauseSubMenuAppearance;
	private PauseMenuController _pauseMenuController;
	private Node _buttonClosePauseSubMenuAppearance;
	public void Initialize(PauseMenuController pauseMenuController, Node canvasPauseSubMenuAppearance, ViewModelPauseSubMenuAppearance viewModelPauseSubMenuAppearance)
	{
		_buttonClosePauseSubMenuAppearance = viewModelPauseSubMenuAppearance.ButtonClosePauseSubMenuAppearance;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_pauseMenuController.OnOpenAppearanceSubMenu += ShowAppearanceSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideAppearanceSubMenuCanvas;

		_buttonClosePauseSubMenuAppearance.GetNodeOrNull<Button>().Pressed += _pauseMenuController.ClosePauseSubMenu;

		GD.Print("PauseSubMenuAppearanceController");
	}

	private void ShowAppearanceSubMenuCanvas()
	{
		_isPauseSubMenuAppearanceOpened = true;
		SetVisible(_canvasPauseSubMenuAppearance, true);
	}

	private void HideAppearanceSubMenuCanvas()
	{
		if (_isPauseSubMenuAppearanceOpened)
		{
			_isPauseSubMenuAppearanceOpened = false;
			SetVisible(_canvasPauseSubMenuAppearance, false);
			GD.Print("AppearanceSubMenu closed");
		}
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}
}