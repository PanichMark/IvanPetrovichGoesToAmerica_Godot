using System;
using Godot;

public interface IMissionStepCondition
{
	bool IsConditionMet { get; }
	Node StepConditionOwner { get; }
}

public interface IMissionStepConditionWithProgress
{
	event Action<int, int> OnStepConditionProgressUpdated;
}

public abstract partial class MissionStepConditionAbstract : Resource, IMissionStepCondition
{
	public MissionStep LinkedMissionStep { get; private set; }
	public virtual Node StepConditionOwner => null;
	public virtual bool IsConditionMet => false;

	public virtual void Initialize(MissionStep missionStep)
	{
		LinkedMissionStep = missionStep;
	}

	public virtual void ResetStepCondition() { }
}