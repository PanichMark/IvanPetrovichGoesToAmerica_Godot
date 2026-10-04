using Godot;
[GlobalClass]
public partial class GameNews : Resource
{
	[Export] public string GameNews_RU { get; set; } = string.Empty;
	[Export] public string GameNews_EN { get; set; } = string.Empty;
}
