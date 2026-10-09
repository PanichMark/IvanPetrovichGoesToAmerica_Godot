using Godot;

[GlobalClass]
public partial class GameDifficultiesList : Resource
{
	[Export] public Godot.Collections.Array<InteractionObjectNoteData> Notes { get; set; } = new();
}