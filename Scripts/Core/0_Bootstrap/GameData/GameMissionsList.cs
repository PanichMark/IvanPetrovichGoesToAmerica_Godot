using Godot;

[GlobalClass]
public partial class GameMissionsList : Resource
{
	[Export] public Mission MissionTest { get; set; }
	[Export] public Godot.Collections.Array<Mission> MissionsInOrder { get; set; } = new();
}