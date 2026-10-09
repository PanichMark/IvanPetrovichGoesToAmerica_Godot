using Godot;

/// <summary>Registers this node with a mission step for deactivation when that step begins.</summary>
public partial class MissionStepObjectRegistractionTurnOff : Node
{
	[Export] public MissionStep LinkedMissionStep { get; set; }

	public override void _Ready()
	{
		if (LinkedMissionStep == null)
		{
			GD.PushWarning($"'{GetPath()}' has no linked mission step for turn-off registration.");
			return;
		}
		LinkedMissionStep.RegisterObjectsTurnOff(GetParent() ?? this);
	}
}