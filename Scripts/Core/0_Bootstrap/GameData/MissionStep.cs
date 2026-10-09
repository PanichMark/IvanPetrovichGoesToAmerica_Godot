using Godot;

[GlobalClass]
public partial class MissionStep : Resource
{
	private readonly Godot.Collections.Array<Node> _objectsToTurnOn = new();
	private readonly Godot.Collections.Array<Node> _objectsToTurnOff = new();
	private MissionsManager _missionsManager;

	[Export] public string MissionStepGoal_RU { get; set; } = string.Empty;
	[Export] public string MissionStepGoal_EN { get; set; } = string.Empty;
	[Export(PropertyHint.MultilineText)] public string StepDescription { get; set; } = string.Empty;
	[Export] public bool ShowMissionMarker { get; set; } = true;
	[Export] public int MinStepIndexIgnore { get; set; }
	[Export] public int MaxStepIndexIgnore { get; set; } = int.MaxValue;
	[Export] public Godot.Collections.Array<MissionStepConditionAbstract> StepConditions { get; set; } = new();
	public Godot.Collections.Array<MissionStepConditionAbstract> Conditions => StepConditions;

	public void Initialize(MissionsManager missionsManager)
	{
		_missionsManager = missionsManager;
		foreach (MissionStepConditionAbstract condition in StepConditions)
			condition?.Initialize(this);
	}

	public void OnStepStarted()
	{
		foreach (Node node in _objectsToTurnOn)
			if (GodotObject.IsInstanceValid(node)) node.SetActive(true);
		foreach (Node node in _objectsToTurnOff)
			if (GodotObject.IsInstanceValid(node)) node.SetActive(false);
	}

	public void OnStepCompleted(int goToNextStep)
	{
		if (_missionsManager == null)
			return;
		if (_missionsManager.CurrentStepIndex >= MinStepIndexIgnore && _missionsManager.CurrentStepIndex <= MaxStepIndexIgnore)
			_missionsManager.GoToNextStep(goToNextStep);
	}

	public void RegisterObjectsTurnOn(Node node)
	{
		if (node != null && !_objectsToTurnOn.Contains(node)) _objectsToTurnOn.Add(node);
	}

	public void RegisterObjectsTurnOff(Node node)
	{
		if (node != null && !_objectsToTurnOff.Contains(node)) _objectsToTurnOff.Add(node);
	}

	public void ClearTurnOnOffLists()
	{
		_objectsToTurnOn.Clear();
		_objectsToTurnOff.Clear();
	}
}