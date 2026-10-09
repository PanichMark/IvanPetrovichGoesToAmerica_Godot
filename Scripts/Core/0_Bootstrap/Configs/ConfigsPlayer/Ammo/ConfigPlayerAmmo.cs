using Godot;
using System.Linq;

[GlobalClass]
public partial class ConfigPlayerAmmo : Resource
{
	[Export] public Godot.Collections.Array<AmmoGive> AmmoEntries { get; set; } = new();

	public AmmoGive[] GetStartAmmoEntries()
	{
		return AmmoEntries.ToArray();
	}
}