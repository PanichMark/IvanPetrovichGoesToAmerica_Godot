using Godot;

[GlobalClass]
public partial class GameCanvasesList : Resource
{
	[Export] public PackedScene CanvasBootstrapInitialization { get; set; }
	[Export] public PackedScene CanvasBootstrapChooseFirstLanguage { get; set; }
	[Export] public PackedScene CanvasBootstrapSignTermsAndConditions { get; set; }

	[Export] public PackedScene CanvasSceneLoadingScreen { get; set; }

	[Export] public PackedScene CanvasSavingProcess { get; set; }

	[Export] public PackedScene CanvasMenuBackground { get; set; }

	[Export] public PackedScene CanvasPauseMenu { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuSave { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuLoad { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuAppearance { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuTutorial { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuSettings { get; set; }
	[Export] public PackedScene CanvasPauseSubMenuSettingsGameDifficulty { get; set; }
	[Export] public PackedScene CanvasPauseMenuConfirmAction { get; set; }

	[Export] public PackedScene CanvasMainMenuChooseMission { get; set; }
	[Export] public PackedScene CanvasMainMenuReadNews { get; set; }

	[Export] public PackedScene CanvasHUDinteraction { get; set; }
	[Export] public PackedScene CanvasHUDmission { get; set; }
	[Export] public PackedScene CanvasHUDhealthAndMana { get; set; }
	[Export] public PackedScene CanvasHUDweapons { get; set; }
	[Export] public PackedScene CanvasHUDmonocular { get; set; }

	[Export] public PackedScene CanvasMenuWeaponWheel { get; set; }

	[Export] public PackedScene CanvasMenuNote { get; set; }
	[Export] public PackedScene CanvasMenuLockpickMechanical { get; set; }
	[Export] public PackedScene CanvasMenuLockpickElectronic { get; set; }
	[Export] public PackedScene CanvasMenuDialogue { get; set; }
	[Export] public PackedScene CanvasMenuCutscene { get; set; }
}
