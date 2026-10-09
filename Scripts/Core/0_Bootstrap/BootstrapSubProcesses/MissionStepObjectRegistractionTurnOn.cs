using Godot;

/// <summary>Registers this node with a mission step for activation when that step begins.</summary>
public partial class MissionStepObjectRegistractionTurnOn : Node
{
	[Export] public MissionStep LinkedMissionStep { get; set; }

	public override void _Ready()
	{
		if (LinkedMissionStep == null)
		{
			GD.PushWarning($"'{GetPath()}' has no linked mission step for turn-on registration.");
			return;
		}
		LinkedMissionStep.RegisterObjectsTurnOn(GetParent() ?? this);
	}
}