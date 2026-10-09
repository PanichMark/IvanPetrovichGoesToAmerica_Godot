using Godot;

[GlobalClass]
public partial class ObjectPoolSprite : ObjectPoolAbstract
{
	[Export] public Texture2D ObjectPoolTexture { get; set; }
}