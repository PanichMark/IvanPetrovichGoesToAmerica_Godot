using Godot;

[GlobalClass]
public partial class GameSceneData : Resource
{
	[Export] public GameScenesSystemEnum GameScene { get; set; }
	[Export(PropertyHint.MultilineText)] public string SceneDescription_RU { get; set; } = string.Empty;
	[Export(PropertyHint.MultilineText)] public string SceneDescription_EN { get; set; } = string.Empty;
	[Export] public Texture2D SceneLoadingScreenImage { get; set; }
	[Export] public Resource SceneGameMission { get; set; }
	[Export] public PackedScene SceneResource { get; set; }
}