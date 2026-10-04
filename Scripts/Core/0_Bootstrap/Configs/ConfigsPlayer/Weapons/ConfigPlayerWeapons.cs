using System;
using System.Collections.Generic;
using Godot;
[GlobalClass]
public partial class ConfigPlayerWeapons : Resource
{
	[Export] public Godot.Collections.Array<PlayerWeaponGive> AvailableWeapons { get; set; } = new();

	public Node[] GetAvailableWeapons()
	{
		List<Node> result = new List<Node>();
		foreach (var entry in AvailableWeapons)
		{
			if (entry.WeaponScene is not null)
			{
				result.Add(entry.WeaponScene.Instantiate());
			}
		}
		return result.ToArray();
	}
}