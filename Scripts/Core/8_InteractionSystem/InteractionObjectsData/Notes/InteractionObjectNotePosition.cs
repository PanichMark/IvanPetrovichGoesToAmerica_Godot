using Godot;

[GlobalClass]
public partial class InteractionObjectNotePosition : Resource
{
	[Export] public Vector3 ImagePosition { get; set; }
	[Export] public Vector2 ImageRotation { get; set; }
	[Export] public float ImageWidth { get; set; }
	[Export] public float ImageHeight { get; set; }
	[Export] public Vector3 TextPosition { get; set; }
	[Export] public Vector2 TextRotation { get; set; }
	[Export] public float TextWidth { get; set; }
	[Export] public float TextHeight { get; set; }
}