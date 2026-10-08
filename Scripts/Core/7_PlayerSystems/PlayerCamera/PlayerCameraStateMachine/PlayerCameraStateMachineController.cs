using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerCameraStateMachineController : Node, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _input;
	private GameScenesManager _scenes;
	private PlayerMovementController _movement;
	private PlayerMovementStateMachineController _movementStates;
	private PlayerCameraController _camera;
	private Action _mainMenuHandler;
	private Action _gameplayHandler;
	public PlayerCameraStateTypes CurrentPlayerCameraStateType { get; private set; } = PlayerCameraStateTypes.ThirdPerson;
	public event Action OnCameraStateChanged;
	public event Action OnFirstPersonCameraState;
	public event Action OnThirdPersonCameraState;

	public void Initialize(Bootstrap bootstrap, IInputDevice input, GameScenesManager scenes, PlayerMovementController movement, PlayerMovementStateMachineController movementStates, PlayerCameraController camera)
	{
		_bootstrap = bootstrap;
		_input = input;
		_scenes = scenes;
		_movement = movement;
		_movementStates = movementStates;
		_camera = camera;
		if (_scenes != null)
		{
			_mainMenuHandler = () => SetPlayerCameraState(PlayerCameraStateTypes.MainMenu);
			_gameplayHandler = () => SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);
			_scenes.OnBeginLoadingMainMenuScene += _mainMenuHandler;
			_scenes.OnBeginLoadingGameplayScene += _gameplayHandler;
		}
		SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);
	}

	public override void _Process(double delta)
	{
		if (_bootstrap?.IsBootstrapInitialized != true)
			return;
		if (_input?.GetKeyChangeCameraView() == true)
			ToggleGameplayCamera();
		_camera?.UpdateCamera((float)delta, _input);
	}

	public void ToggleGameplayCamera()
	{
		SetPlayerCameraState(CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson
			? PlayerCameraStateTypes.ThirdPerson
			: PlayerCameraStateTypes.FirstPerson);
	}

	public void SetPlayerCameraState(PlayerCameraStateTypes type)
	{
		CurrentPlayerCameraStateType = type;
		_movement?.GiveCurrentPlayerCameraType(type);
		if (type == PlayerCameraStateTypes.FirstPerson)
		{
			_camera?.SetCameraToFirstPerson();
			OnFirstPersonCameraState?.Invoke();
		}
		else if (type == PlayerCameraStateTypes.ThirdPerson)
		{
			_camera?.SetCameraToThirdPerson();
			OnThirdPersonCameraState?.Invoke();
		}
		OnCameraStateChanged?.Invoke();
	}

	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerCamera.PlayerCameraStateType = CurrentPlayerCameraStateType.ToString();
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		if (Enum.TryParse(data.PlayerCamera.PlayerCameraStateType, out PlayerCameraStateTypes state))
			SetPlayerCameraState(state);
		return Task.CompletedTask;
	}
	public override void _ExitTree()
	{
		if (_scenes != null)
		{
			if (_mainMenuHandler != null) _scenes.OnBeginLoadingMainMenuScene -= _mainMenuHandler;
			if (_gameplayHandler != null) _scenes.OnBeginLoadingGameplayScene -= _gameplayHandler;
		}
	}
}