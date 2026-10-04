using Godot;

[GlobalClass]
public partial class GameScenesList : Resource
{
	[Export] public Godot.Collections.Array<GameSceneData> GameScenes { get; set; } = new();
}