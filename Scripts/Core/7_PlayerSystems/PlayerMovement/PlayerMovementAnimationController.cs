using Godot;

public partial class PlayerMovementAnimationController : Node
{
	private GameController _gameController;
	private PlayerBehaviourController _behaviour;
	private PlayerMovementController _movement;
	private PlayerMovementStateMachineController _states;
	private PlayerCameraStateMachineController _cameraStates;
	private AnimationPlayer _animationPlayer;
	private string _currentAnimation = string.Empty;
	private string _deathAnimation = "StationaryAction_Dying";

	public void Initialize(GameController gameController, IInputDevice input, PlayerBehaviourController behaviour,
		PlayerMovementController movement, PlayerMovementStateMachineController states,
		PlayerCameraStateMachineController cameraStates, Node player)
	{
		_gameController = gameController;
		_behaviour = behaviour;
		_movement = movement;
		_states = states;
		_cameraStates = cameraStates;
		_animationPlayer = player?.FindDeepNodeOfType<AnimationPlayer>();
		_states.OnChangeMovementState += HandleMovementStateChanged;
		_movement.OnChangePlayerMovementSpeedChangedByPickable += SetAnimationSpeed;
		_movement.OnMovementSpeedChangedByStateMachine += ResetAnimationSpeed;
		HandleMovementStateChanged(_states.CurrentPlayerMovementStateType);
	}

	private void HandleMovementStateChanged(PlayerMovementStateTypes state)
	{
		if (_gameController?.IsPlayerDead == true) return;
		string animation = state switch
		{
			PlayerMovementStateTypes.PlayerIdleStanding => "StationaryAction_IdleStanding",
			PlayerMovementStateTypes.PlayerIdleCrouhcing => "StationaryAction_IdleCrouching",
			PlayerMovementStateTypes.PlayerWalkingStanding => "Movement_WalkingStandingForward",
			PlayerMovementStateTypes.PlayerWalkingCrouching => "Movement_WalkingCrouchingForward",
			PlayerMovementStateTypes.PlayerRunning => "Movement_RunningForward",
			PlayerMovementStateTypes.PlayerJumping => "Movement_Jumping",
			PlayerMovementStateTypes.PlayerFalling => "Movement_Falling",
			PlayerMovementStateTypes.PlayerSliding => "Movement_Sliding",
			PlayerMovementStateTypes.PlayerLedgeClimbingStanding => "Movement_LedgeClimbingStanding",
			_ => "StationaryAction_IdleStanding"
		};
		Play(animation);
	}

	private void Play(string animation)
	{
		if (_animationPlayer == null || _currentAnimation == animation || !_animationPlayer.HasAnimation(animation)) return;
		_currentAnimation = animation;
		_animationPlayer.Play(animation, 0.2);
	}
	private void SetAnimationSpeed(float speed) { if (_animationPlayer != null) _animationPlayer.SpeedScale = speed; }
	private void ResetAnimationSpeed() => SetAnimationSpeed(1f);
	public void PlayerDeathAnimation() => Play(_deathAnimation);
	public override void _ExitTree()
	{
		if (_states != null) _states.OnChangeMovementState -= HandleMovementStateChanged;
		if (_movement != null)
		{
			_movement.OnChangePlayerMovementSpeedChangedByPickable -= SetAnimationSpeed;
			_movement.OnMovementSpeedChangedByStateMachine -= ResetAnimationSpeed;
		}
	}
}