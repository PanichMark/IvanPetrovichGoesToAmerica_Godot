using System;
using Godot;

/// <summary>Shows and hides the shared menu backdrop in response to menu-manager events.</summary>
public partial class MenuBackgroundController : Node
{
	private MenuManager _menuManager;
	private Node _canvasMenuBackground;
	private bool _initialized;

	public void Initialize(MenuManager menuManager, Node canvasMenuBackground)
	{
		if (_initialized)
			UnsubscribeFromMenuEvents();

		_menuManager = menuManager ?? throw new ArgumentNullException(nameof(menuManager));
		_canvasMenuBackground = canvasMenuBackground ?? throw new ArgumentNullException(nameof(canvasMenuBackground));

		_menuManager.OnOpenMenuBackground += ShowCanvasMenuBackground;
		_menuManager.OnCloseMenuBackground += HideCanvasMenuBackground;
		_initialized = true;
	}

	public override void _ExitTree()
	{
		if (_initialized)
			UnsubscribeFromMenuEvents();
	}

	public void ShowCanvasMenuBackground()
	{
		SetVisible(_canvasMenuBackground, true);
		GD.Print("Show MenuBackground");
	}

	public void HideCanvasMenuBackground()
	{
		SetVisible(_canvasMenuBackground, false);
		GD.Print("Hide MenuBackground");
	}

	private void UnsubscribeFromMenuEvents()
	{
		_menuManager.OnOpenMenuBackground -= ShowCanvasMenuBackground;
		_menuManager.OnCloseMenuBackground -= HideCanvasMenuBackground;
		_initialized = false;
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}
}