using Godot;
public partial class HUDhealthAndManaController : Node
{
    private MenuManager _menuManager;
    private Node _canvasHUDhealthAndMana;
    private GameScenesManager _gameSceneManager;
    private GameController _gameController;

	private Node _HUDhealthAndManaBars;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private Node _healthBar;
	private Node _manaBar;

	public void Initialize (
		GameController gameController,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		Node canvasHUDPlayerResources,
		ViewModelHUDHealthAndMana viewModelHUDHealthAndMana)
    {
        _gameSceneManager = gameSceneManager;
        _menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
        _canvasHUDhealthAndMana = canvasHUDPlayerResources;
        _healthBar = viewModelHUDHealthAndMana.HealthBar;
        _manaBar = viewModelHUDHealthAndMana.ManaBar;
		_HUDhealthAndManaBars = viewModelHUDHealthAndMana.HUDhealthAndManaBars;

		HideHealthBar();
		HideManaBar();

		_menuManager.OnOpenPauseMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDhealthAndMana;
		_menuManager.OnOpenCutsceneMenu += HideCanvasHUDhealthAndMana;
		_menuManager.OnCloseCutsceneMenu += ShowCanvasHUDhealthAndMana;
        _gameController = gameController;
        _gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasHUDhealthAndMana;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDhealthAndMana;
        _gameController.OnPlayerEarlyDeath += HideCanvasHUDhealthAndMana;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDhealthAndManaBars;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDhealthAndManaBars;

		GD.Print("HUDhealthAndManaController Initialized");
	}

    private void ShowCanvasHUDhealthAndMana()
    {
       if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_menuManager.IsCutsceneMenuOpened && !_gameController.IsMainMenuOrEndGameTitlesActive && !_menuManager.IsMainMenuBeingLoaded)
       {

		    SetVisible(_canvasHUDhealthAndMana, true);
           GD.Print("Show canvasHUDhealthAndMana");
       }
    }

	public void HideCanvasHUDhealthAndMana()
	{
		SetVisible(_canvasHUDhealthAndMana, false);
		GD.Print("Hide canvasHUDhealthAndMana");
	}

	private void ShowHUDhealthAndManaBars()
	{
		SetVisible(_HUDhealthAndManaBars, true);
	}

	private void HideHUDhealthAndManaBars()
	{
		SetVisible(_HUDhealthAndManaBars, false);
	}

	public void ShowHealthBar()
    {
		SetVisible(_healthBar, true);
    }

	public void HideHealthBar()
	{
		SetVisible(_healthBar, false);
	}

	public void ShowManaBar()
	{
		SetVisible(_manaBar, true);
	}

	public void HideManaBar()
	{
		SetVisible(_manaBar, false);
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}
}
