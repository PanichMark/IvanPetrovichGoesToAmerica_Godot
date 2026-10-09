using Godot;

[GlobalClass]
public partial class Mission : Resource
{
	[Export] public string MissionName { get; set; } = string.Empty;
	[Export(PropertyHint.MultilineText)] public string MissionDescription { get; set; } = string.Empty;
	[Export] public MissionResourcesData MissionResources { get; set; }
	[Export] public Godot.Collections.Array<MissionStep> MissionSteps { get; set; } = new();
}