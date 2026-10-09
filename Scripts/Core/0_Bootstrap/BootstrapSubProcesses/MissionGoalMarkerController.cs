using Godot;

public partial class MissionGoalMarkerController : Node
{
	private MissionsManager _missionsManager;
	private Node _marker;

	public void Initialize(Bootstrap bootstrap, GameScenesManager gameSceneManager, MissionsManager missionsManager,
		Node playerCamera, Node imageMissionGoalMarker)
	{
		_missionsManager = missionsManager;
		_marker = imageMissionGoalMarker;
		if (_marker != null) _marker.SetVisible(false);
		if (_missionsManager != null)
			_missionsManager.OnCurrentStepChanged += UpdateMarkerVisibility;
		UpdateMarkerVisibility();
	}

	private void UpdateMarkerVisibility()
	{
		bool visible = _missionsManager?.ActiveMission?.MissionSteps != null &&
			_missionsManager.CurrentStepIndex >= 0 &&
			_missionsManager.CurrentStepIndex < _missionsManager.ActiveMission.MissionSteps.Count &&
			_missionsManager.ActiveMission.MissionSteps[_missionsManager.CurrentStepIndex]?.ShowMissionMarker == true;
		_marker?.SetVisible(visible);
	}

	public void CheckMissionStep() => UpdateMarkerVisibility();

	public override void _ExitTree()
	{
		if (_missionsManager != null) _missionsManager.OnCurrentStepChanged -= UpdateMarkerVisibility;
	}
}