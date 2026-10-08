using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerMovementStateMachineController : Node, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private GameScenesManager _sceneManager;
	private PlayerMovementController _movement;
	private PlayerHealthController _health;
	private bool _initialized;
	private float _stateTime;
	private Action _beginMainMenuHandler;

	public event Action<PlayerMovementStateTypes> OnChangeMovementState;
	public PlayerMovementStateTypes CurrentPlayerMovementStateType { get; private set; } = PlayerMovementStateTypes.PlayerIdleStanding;

	public void Initialize(Bootstrap bootstrap, IInputDevice inputDevice, GameScenesManager sceneManager, PlayerMovementController movement)
	{
		_bootstrap = bootstrap;
		_sceneManager = sceneManager;
		_movement = movement;
		_health = GetTree()?.Root.FindChild(nameof(PlayerHealthController), true, false) as PlayerHealthController;
		if (_health == null && movement != null)
			_health = movement.GetParent()?.FindDeepNodeOfType<PlayerHealthController>();
		if (_movement != null)
			_movement.OnSendSignalToPlayerMovementStateMachine += SetPlayerMovementState;
		if (_sceneManager != null)
		{
			_beginMainMenuHandler = () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdleStanding);
			_sceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += _beginMainMenuHandler;
		}
		SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdleStanding);
		_initialized = true;
	}

	public void SetHealthController(PlayerHealthController healthController) => _health = healthController;

	public override void _Process(double delta)
	{
		if (!_initialized || _bootstrap?.IsBootstrapInitialized != true || _movement == null)
			return;

		_stateTime += (float)delta;
	}

	public void SetPlayerMovementState(PlayerMovementStateTypes newState)
	{
		if (_movement != null && !_movement.IsAbleToChangeMovementType && newState != PlayerMovementStateTypes.PlayerFalling)
			return;
		if (CurrentPlayerMovementStateType == newState)
			return;

		CurrentPlayerMovementStateType = newState;
		_stateTime = 0f;
		_health?.HandleMovementStateChanged(newState);
		OnChangeMovementState?.Invoke(newState);
	}

	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerMovement.PlayerMovementStateType = CurrentPlayerMovementStateType.ToString();
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		if (Enum.TryParse(data.PlayerMovement.PlayerMovementStateType, out PlayerMovementStateTypes state))
			SetPlayerMovementState(state);
		else
			SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdleStanding);
		return Task.CompletedTask;
	}
	public override void _ExitTree()
	{
		if (_movement != null) _movement.OnSendSignalToPlayerMovementStateMachine -= SetPlayerMovementState;
		if (_sceneManager != null && _beginMainMenuHandler != null)
			_sceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= _beginMainMenuHandler;
	}
}