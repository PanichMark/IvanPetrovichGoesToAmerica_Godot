using Godot;
[GlobalClass]
public partial class BootstrapGameDataList : Resource
{
	[Export] public int NumberOfSafeFileSlots { get; set; } = 20;
	[Export] public Godot.Collections.Array<Texture2D> NPCdetectionSignFrames { get; set; } = new();
	[Export] public AudioBusLayout AudioBusLayout { get; set; }
	[Export(PropertyHint.File, "*.csv")] public string LocalizationMain { get; set; } = "res://Localization/LocalizationMain.csv";
	[Export] public TermsAndConditions TermsAndConditions { get; set; }
	[Export] public GameNews GameNews { get; set; }
    [Export] public GameCanvasesList GameCanvasesList { get; set; }
	[Export] public GameScenesList GameScenesList { get; set; }
	[Export] public GameMissionsList GameMissionsList { get; set; }
	[Export] public GameObjectPoolsList GameObjectPoolsList { get; set; }
	[Export] public GameTutorialsList GameTutorialsList { get; set; }
    [Export] public GameDifficultiesList GameDifficultiesList { get; set; }
}
