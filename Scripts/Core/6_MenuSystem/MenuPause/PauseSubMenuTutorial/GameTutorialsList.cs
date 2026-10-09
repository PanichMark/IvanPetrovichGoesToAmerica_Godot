using Godot;

[GlobalClass]
public partial class GameTutorialsList : Resource
{
	[Export] public Godot.Collections.Array<InteractionObjectNoteData> Notes { get; set; } = new();
}