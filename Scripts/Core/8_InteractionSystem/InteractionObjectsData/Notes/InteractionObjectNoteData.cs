using Godot;

[GlobalClass]
public partial class InteractionObjectNoteData : Resource
{
	[Export(PropertyHint.File, "*.txt,*.md,*.csv")]
	public string NoteText_RU { get; set; } = string.Empty;

	[Export(PropertyHint.File, "*.txt,*.md,*.csv")]
	public string NoteText_EN { get; set; } = string.Empty;

	[Export] public Texture2D NoteImage { get; set; }
	[Export] public InteractionObjectNotePosition NotePosition { get; set; }
	[Export] public bool IsNoteToGlanceAt { get; set; }
}