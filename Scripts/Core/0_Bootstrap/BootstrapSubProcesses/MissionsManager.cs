using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class MissionsManager : Node, IJsonSaveLoad
{
	private LocalizationManager _localizationManager;
	private GameScenesManager _gameSceneManager;
	private JsonSaveLoadController _saveLoadController;
	private HUDmissionsController _hudMissionsController;
	private GameMissionsList _gameMissions;
	private IMissionStepConditionWithProgress _progressCondition;
	private bool _initialized;

	public Mission ActiveMission { get; private set; }
	public int ActiveMissionIndex { get; private set; }
	public int CurrentStepIndex { get; private set; }
	public string LocalizedGoalText { get; private set; } = string.Empty;
	public event Action OnCurrentStepChanged;
	public event Action<Node> OnAnyObjectInteracted;
	public event Action<Node, bool> OnAnyObjectDestroyed;

	public void Initialize(LocalizationManager localizationManager, GameScenesManager gameSceneManager,
		JsonSaveLoadController jsonSaveLoadController, HUDmissionsController hudMissionsController,
		GameMissionsList gameMissions)
	{
		_localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
		_gameSceneManager = gameSceneManager ?? throw new ArgumentNullException(nameof(gameSceneManager));
		_saveLoadController = jsonSaveLoadController ?? throw new ArgumentNullException(nameof(jsonSaveLoadController));
		_hudMissionsController = hudMissionsController ?? throw new ArgumentNullException(nameof(hudMissionsController));
		_gameMissions = gameMissions ?? throw new ArgumentNullException(nameof(gameMissions));

		foreach (Mission mission in _gameMissions.MissionsInOrder)
			foreach (MissionStep step in mission?.MissionSteps ?? new Godot.Collections.Array<MissionStep>())
			{
				step?.Initialize(this);
				foreach (MissionStepConditionAbstract condition in step?.Conditions ?? new Godot.Collections.Array<MissionStepConditionAbstract>())
					condition?.ResetStepCondition();
			}

		_localizationManager.OnLanguageChanged -= HandleLanguageChanged;
		_localizationManager.OnLanguageChanged += HandleLanguageChanged;
		_gameSceneManager.OnEndLoadingGameplayScene -= HandleGameplaySceneLoaded;
		_gameSceneManager.OnEndLoadingGameplayScene += HandleGameplaySceneLoaded;
		_gameSceneManager.OnBeginLoadingGameplayScene -= ClearRegistrationLists;
		_gameSceneManager.OnBeginLoadingGameplayScene += ClearRegistrationLists;
	
		ActiveMissionIndex = 0;
		ActiveMission = GetMissionAt(ActiveMissionIndex) ?? _gameMissions.MissionTest;
		CurrentStepIndex = 0;
		_initialized = true;
		if (ActiveMission?.MissionSteps.Count > 0)
			SetCurrentStep(0, false);
		else
			_hudMissionsController.SetCurrentMissionGoalText(string.Empty);
		GD.Print("MissionsManager initialized.");
	}

	public void GoToNextStep(int goToNextStep)
	{
		if (!_initialized || ActiveMission == null)
			return;
		if (goToNextStep == -1)
			return;

		if (goToNextStep >= ActiveMission.MissionSteps.Count)
		{
			StartNextMission();
			return;
		}
		if (goToNextStep < 0)
		{
			GD.PushWarning($"Ignoring invalid mission step index {goToNextStep}.");
			return;
		}
		SetCurrentStep(goToNextStep, true);
	}

	public void ApplyMissionConfig(Mission mission, int missionStep)
	{
		if (mission == null)
			return;
		ActiveMission = mission;
		int index = _gameMissions.MissionsInOrder.IndexOf(mission);
		if (index >= 0) ActiveMissionIndex = index;
		GoToNextStep(missionStep);
	}

	public void NotifyObjectInteracted(Node node) => OnAnyObjectInteracted?.Invoke(node);
	public void NotifyObjectDestroyed(Node node, bool wasLethal) => OnAnyObjectDestroyed?.Invoke(node, wasLethal);

	public Task SaveJsonData(JsonGameData data)
	{
		if (data?.MissionData != null && ActiveMission != null)
		{
			string name = ActiveMission.ResourceName;
			if (name.StartsWith("Mission_", StringComparison.Ordinal)) name = name["Mission_".Length..];
			if (float.TryParse(name, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float missionNumber))
				data.MissionData.Mission = missionNumber;
			else
				data.MissionData.Mission = ActiveMissionIndex;
			data.MissionData.MissionStep = CurrentStepIndex;
		}
		return Task.CompletedTask;
	}

	public Task LoadJsonData(JsonGameData data)
	{
		if (data?.MissionData == null || _gameMissions?.MissionsInOrder == null || _gameMissions.MissionsInOrder.Count == 0)
			return Task.CompletedTask;
		string target = data.MissionData.Mission.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
		int missionIndex = -1;
		for (int i = 0; i < _gameMissions.MissionsInOrder.Count; i++)
		{
			string resourceName = _gameMissions.MissionsInOrder[i]?.ResourceName ?? string.Empty;
			if (resourceName.StartsWith("Mission_", StringComparison.Ordinal)) resourceName = resourceName["Mission_".Length..];
			if (resourceName == target) { missionIndex = i; break; }
		}
		if (missionIndex < 0 && data.MissionData.Mission >= 0 && data.MissionData.Mission < _gameMissions.MissionsInOrder.Count)
			missionIndex = (int)data.MissionData.Mission;
		if (missionIndex >= 0)
		{
			ActiveMissionIndex = missionIndex;
			ActiveMission = _gameMissions.MissionsInOrder[missionIndex];
			int lastStep = Math.Max(0, (ActiveMission?.MissionSteps.Count ?? 0) - 1);
			SetCurrentStep(Math.Clamp(data.MissionData.MissionStep, 0, lastStep), true);
		}
		return Task.CompletedTask;
	}

	private void SetCurrentStep(int index, bool notify)
	{
		UnsubscribeProgress();
		CurrentStepIndex = index;
		MissionStep step = ActiveMission?.MissionSteps.ElementAtOrDefault(index);
		if (step == null)
		{
			LocalizedGoalText = string.Empty;
			_hudMissionsController.SetCurrentMissionGoalText(string.Empty);
			OnCurrentStepChanged?.Invoke();
			return;
		}

		step.OnStepStarted();
		LocalizedGoalText = GetLocalizedGoalText(step);
		_hudMissionsController.SetCurrentMissionGoalText(LocalizedGoalText);
		if (notify)
			_hudMissionsController.ShowNewMissionGoalHUDnotification(LocalizedGoalText, true);
		if (step.Conditions.Count > 0 && step.Conditions[0] is IMissionStepConditionWithProgress progress)
		{
			_progressCondition = progress;
			_progressCondition.OnStepConditionProgressUpdated += HandleStepProgress;
		}
		OnCurrentStepChanged?.Invoke();
	}

	private void StartNextMission()
	{
		int nextIndex = ActiveMissionIndex + 1;
		Mission nextMission = GetMissionAt(nextIndex);
		if (nextMission == null)
		{
			ActiveMissionIndex = _gameMissions.MissionsInOrder.Count;
			CurrentStepIndex = 0;
			ActiveMission = null;
			LocalizedGoalText = string.Empty;
			UnsubscribeProgress();
			_hudMissionsController.SetCurrentMissionGoalText(string.Empty);
			OnCurrentStepChanged?.Invoke();
			return;
		}
		ActiveMissionIndex = nextIndex;
		ActiveMission = nextMission;
		SetCurrentStep(0, true);
	}

	private Mission GetMissionAt(int index) => _gameMissions?.MissionsInOrder != null && index >= 0 && index < _gameMissions.MissionsInOrder.Count
		? _gameMissions.MissionsInOrder[index] : null;

	private string GetLocalizedGoalText(MissionStep step) =>
		_localizationManager.CurrentLanguage == LanguagesEnum.Russian ? step.MissionStepGoal_RU : step.MissionStepGoal_EN;

	private void HandleLanguageChanged(LocalizationManager manager)
	{
		if (ActiveMission?.MissionSteps.ElementAtOrDefault(CurrentStepIndex) is MissionStep step)
		{
			LocalizedGoalText = GetLocalizedGoalText(step);
			_hudMissionsController.SetCurrentMissionGoalText(LocalizedGoalText);
		}
	}

	private void HandleGameplaySceneLoaded()
	{
		if (ActiveMission?.MissionSteps.ElementAtOrDefault(CurrentStepIndex) is MissionStep step)
		{
			LocalizedGoalText = GetLocalizedGoalText(step);
			_hudMissionsController.SetCurrentMissionGoalText(LocalizedGoalText);
			_hudMissionsController.ShowNewMissionGoalHUDnotification(LocalizedGoalText, false);
		}
	}

	private void HandleStepProgress(int currentAmount, int requiredAmount)
	{
		if (ActiveMission?.MissionSteps.ElementAtOrDefault(CurrentStepIndex) is MissionStep step)
			_hudMissionsController.ShowNewMissionGoalHUDnotification($"{GetLocalizedGoalText(step)}: {currentAmount}/{requiredAmount}", false);
	}

	private void ClearRegistrationLists()
	{
		foreach (Mission mission in _gameMissions.MissionsInOrder)
			foreach (MissionStep step in mission?.MissionSteps ?? new Godot.Collections.Array<MissionStep>())
				step?.ClearTurnOnOffLists();
	}

	private void UnsubscribeProgress()
	{
		if (_progressCondition != null)
			_progressCondition.OnStepConditionProgressUpdated -= HandleStepProgress;
		_progressCondition = null;
	}

	public override void _ExitTree()
	{
		UnsubscribeProgress();
		if (_localizationManager != null) _localizationManager.OnLanguageChanged -= HandleLanguageChanged;
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnEndLoadingGameplayScene -= HandleGameplaySceneLoaded;
			_gameSceneManager.OnBeginLoadingGameplayScene -= ClearRegistrationLists;
		}
	}
}