using System;
using System.Threading.Tasks;
using Godot;

/// <summary>Godot CharacterBody3D movement adapter for the migrated player systems.</summary>
public partial class PlayerMovementController : Node, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private GameScenesManager _sceneManager;
	private PlayerBehaviourController _behaviour;
	private CharacterBody3D _body;
	private Node3D _player;
	private Camera3D _camera;
	private IInputDevice _input;
	private Vector2 _moveInput;
	private Vector3 _previousPosition;
	private bool _jumpRequested;
	private bool _wasJumpPressed;
	private bool _wasCrouchPressed;
	private bool _crouchRequested;
	private bool _runRequested;
	private bool _movementLocked;
	private PlayerCameraStateTypes _cameraType = PlayerCameraStateTypes.ThirdPerson;
	private float _speedMultiplier = 1f;
	private float _slideRemaining;

	public event Action<PlayerMovementStateTypes> OnSendSignalToPlayerMovementStateMachine;
	public event Action OnMovementSpeedChangedByStateMachine;
	public event Action<float> OnChangePlayerMovementSpeedChangedByPickable;
	public event Action<float> OnSetPlayerCameraRotationY;
	public Node3D PlayerTransform => _player;
	public CharacterBody3D PlayerRigidBody => _body;
	public float PlayerCurrentMovementSpeed { get; private set; } = 3f;
	public float PlayerDefaultMovementSpeed { get; private set; } = 3f;
	public float PlayerRotationSpeed { get; private set; } = 300f;
	public float PlayerSlidingSpeed { get; private set; } = 7.5f;
	public float PlayerCurrentHeight { get; private set; } = 1.75f;
	public bool IsAbleToChangeMovementType { get; set; } = true;
	public bool IsPlayerJumping { get; private set; }
	public bool IsPlayerSliding { get; private set; }
	public bool IsPlayerGrounded => _body?.IsOnFloor() ?? false;
	public bool IsPlayerCrouching { get; private set; }
	public bool IsPlayerAbleToStandUp { get; private set; } = true;
	public bool IsPlayerFalling { get; private set; }
	public bool IsPlayerAbleToClimbLedge { get; private set; }
	public bool IsPlayerOnSlope { get; private set; }
	public bool IsPlayerLedgeClimbing { get; private set; }
	public Vector3 PlayerPreviousFramePositionChange { get; private set; }
	public float PlayerUpRayYPosition { get; private set; } = 1.9f;
	public float PlayerDownRayYPosition { get; private set; } = 0.1f;

	public override void _Ready()
	{
		_player = GetParent() as Node3D;
		_body = _player as CharacterBody3D;
		if (_body == null && _player != null)
			_body = _player.FindDeepNodeOfType<CharacterBody3D>();
		_previousPosition = _player?.GlobalPosition ?? Vector3.Zero;
	}

	public void Initialize(Bootstrap bootstrap, GameScenesManager sceneManager, PlayerBehaviourController behaviour)
	{
		_bootstrap = bootstrap;
		_sceneManager = sceneManager;
		_behaviour = behaviour;
		_player ??= GetParent() as Node3D;
		_body ??= _player as CharacterBody3D;
		_body ??= _player?.FindDeepNodeOfType<CharacterBody3D>();
		_camera = GetTree()?.Root.FindChild("Camera3D", true, false) as Camera3D;
		if (_sceneManager != null)
			_sceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += () => SetPlayerPosition(new Vector3(0f, 0f, -5f));
		_previousPosition = _player?.GlobalPosition ?? Vector3.Zero;
	}

	public override void _Process(double delta)
	{
		if (_behaviour == null || _input == null)
			return;

		_moveInput = new Vector2(
			(_input?.GetKeyRight() == true ? 1f : 0f) - (_input?.GetKeyLeft() == true ? 1f : 0f),
			(_input?.GetKeyDown() == true ? 1f : 0f) - (_input?.GetKeyUp() == true ? 1f : 0f));
		_runRequested = _input?.GetKeyRun() == true;
		bool crouchPressed = _input?.GetKeyCrouch() == true;
		if (crouchPressed && !_wasCrouchPressed)
		{
			bool requestCrouch = !_crouchRequested;
			if (!requestCrouch || IsPlayerAbleToStandUp)
				_crouchRequested = requestCrouch;
		}
		_wasCrouchPressed = crouchPressed;
		bool jumpPressed = _input?.GetKeyJump() == true;
		if (jumpPressed && !_wasJumpPressed)
			_jumpRequested = true;
		_wasJumpPressed = jumpPressed;
	}

	public void SetInputDevice(IInputDevice inputDevice) => _input = inputDevice;

	public override void _PhysicsProcess(double delta)
	{
		if (_body == null || _bootstrap?.IsBootstrapInitialized != true)
			return;

		float dt = (float)delta;
		PlayerPreviousFramePositionChange = _player.GlobalPosition - _previousPosition;
		_previousPosition = _player.GlobalPosition;
		IsPlayerFalling = !_body.IsOnFloor() && _body.Velocity.Y < -0.01f;
		IsPlayerJumping = !_body.IsOnFloor() && !IsPlayerFalling;
		IsPlayerCrouching = _crouchRequested;
		IsPlayerAbleToStandUp = CanStandUp();

		Vector3 forward = -_player.GlobalBasis.Z;
		Vector3 right = _player.GlobalBasis.X;
		if (_camera != null)
		{
			forward = -_camera.GlobalBasis.Z;
			right = _camera.GlobalBasis.X;
		}
		forward.Y = 0f;
		right.Y = 0f;
		Vector3 direction = (right.Normalized() * _moveInput.X + forward.Normalized() * -_moveInput.Y).Normalized();

		if (_body.IsOnFloor())
		{
			if (_body.Velocity.Y < 0f)
				_body.Velocity = new Vector3(_body.Velocity.X, 0f, _body.Velocity.Z);
			if (_jumpRequested && !_crouchRequested && _bootstrap?.IsBootstrapInitialized == true)
				_body.Velocity = new Vector3(_body.Velocity.X, 5.5f, _body.Velocity.Z);
		}
		else
		{
			_body.Velocity += _body.GetGravity() * dt;
		}

		float speed = PlayerCurrentMovementSpeed * (_runRequested && !_crouchRequested ? 1.7f : 1f);
		if (_movementLocked)
			direction = Vector3.Zero;
		if (_slideRemaining > 0f)
		{
			_slideRemaining -= dt;
			IsPlayerSliding = true;
			speed = PlayerSlidingSpeed;
		}
		else
		{
			IsPlayerSliding = false;
		}

		_body.Velocity = new Vector3(direction.X * speed, _body.Velocity.Y, direction.Z * speed);
		if (_bootstrap?.IsBootstrapInitialized == true)
			_body.MoveAndSlide();
		else
			_body.Velocity = Vector3.Zero;
		_jumpRequested = false;
		IsPlayerOnSlope = _body.IsOnFloor() && _body.GetFloorAngle() > 0.08f;

		if (direction.LengthSquared() > 0.001f && _cameraType == PlayerCameraStateTypes.ThirdPerson && !_behaviour.IsPlayerArmed)
		{
			float targetYaw = Mathf.Atan2(-direction.X, -direction.Z);
			_player.Rotation = new Vector3(_player.Rotation.X, Mathf.RotateToward(_player.Rotation.Y, targetYaw, Mathf.DegToRad(PlayerRotationSpeed) * dt), _player.Rotation.Z);
		}

		OnSendSignalToPlayerMovementStateMachine?.Invoke(ResolveMovementState(direction));
	}

	private PlayerMovementStateTypes ResolveMovementState(Vector3 direction)
	{
		if (IsPlayerLedgeClimbing) return PlayerMovementStateTypes.PlayerLedgeClimbingStanding;
		if (IsPlayerSliding) return PlayerMovementStateTypes.PlayerSliding;
		if (!_body.IsOnFloor()) return IsPlayerFalling ? PlayerMovementStateTypes.PlayerFalling : PlayerMovementStateTypes.PlayerJumping;
		if (_crouchRequested) return direction.LengthSquared() > 0.001f ? PlayerMovementStateTypes.PlayerWalkingCrouching : PlayerMovementStateTypes.PlayerIdleCrouhcing;
		if (direction.LengthSquared() < 0.001f) return PlayerMovementStateTypes.PlayerIdleStanding;
		return _runRequested ? PlayerMovementStateTypes.PlayerRunning : PlayerMovementStateTypes.PlayerWalkingStanding;
	}

	private bool CanStandUp()
	{
		if (_body == null || _body.GetWorld3D() == null) return true;
		var query = new PhysicsShapeQueryParameters3D { Shape = new CapsuleShape3D { Radius = 0.35f, Height = 1.8f }, Transform = new Transform3D(Basis.Identity, _body.GlobalPosition + Vector3.Up * 0.9f), Exclude = new Godot.Collections.Array<Rid> { _body.GetRid() }, CollisionMask = _body.CollisionMask };
		return _body.GetWorld3D().DirectSpaceState.IntersectShape(query, 1).Count == 0;
	}

	public void SetPlayerWorldMovement(Vector3 movement) => _moveInput = new Vector2(movement.X, movement.Z);
	public void SetPlayerWorldMovement(Vector2 movement) => _moveInput = movement;
	public void RequestJump() => _jumpRequested = true;
	public void SetCrouching(bool crouching) => _crouchRequested = crouching && (IsPlayerCrouching || IsPlayerAbleToStandUp);
	public void SetRunning(bool running) => _runRequested = running;
	public void ChangePlayerRayPosition(float height) => PlayerUpRayYPosition = height;
	public void GiveCurrentPlayerCameraType(PlayerCameraStateTypes type) => _cameraType = type;
	public void SetCamera(Camera3D camera) => _camera = camera;
	public float ChangePlayerMovementSpeed(float multiplier, bool isChangedByStateMachine)
	{
		_speedMultiplier = Mathf.Max(0f, multiplier);
		PlayerCurrentMovementSpeed = PlayerDefaultMovementSpeed * _speedMultiplier;
		if (isChangedByStateMachine) OnMovementSpeedChangedByStateMachine?.Invoke();
		else OnChangePlayerMovementSpeedChangedByPickable?.Invoke(_speedMultiplier);
		return PlayerCurrentMovementSpeed;
	}
	public void ChangePlayerRotationSpeed(float speed) => PlayerRotationSpeed = Mathf.Max(0f, speed);
	public void StopPlayerRigidBodyVelocity() { if (_body != null) _body.Velocity = Vector3.Zero; }
	public void StopPlayerMovement() { _movementLocked = true; StopPlayerRigidBodyVelocity(); }
	public void ResumePlayerMovement() => _movementLocked = false;
	public void StartPlayerSliding() { _slideRemaining = 1f; IsPlayerSliding = true; }
	public void StartPlayerLedgeClimbing() { IsPlayerLedgeClimbing = true; _movementLocked = true; }
	public void StopPlayerLedgeClimbing() { IsPlayerLedgeClimbing = false; _movementLocked = false; }
	public void StartPlayerVaulting() => StartPlayerLedgeClimbing();
	public void SetPlayerRotationY(float rotationY) { if (_player != null) _player.Rotation = new Vector3(0f, Mathf.DegToRad(rotationY), 0f); OnSetPlayerCameraRotationY?.Invoke(rotationY); }
	public void SetPlayerPosition(Vector3 position) { if (_player != null) { _player.GlobalPosition = position; StopPlayerRigidBodyVelocity(); } }
	public void RotatePlayerTowardsNPC(Node3D npc) { if (_player == null || npc == null) return; Vector3 d = npc.GlobalPosition - _player.GlobalPosition; if (d.LengthSquared() > 0.001f) _player.LookAt(new Vector3(npc.GlobalPosition.X, _player.GlobalPosition.Y, npc.GlobalPosition.Z), Vector3.Up); }
	public void SetPlayerFloorDetectionRayCastLengthToDefault() { }
	public void SetPlayerFloorDetectionRayCastLengthToZero() { }
	public bool JumpingStateWait() => IsPlayerOnSlope;
	public void StopJumpingStateWait() { }
	public void DisablePlayerMovementDuringLegKickAttack() { _movementLocked = true; }

	public Task SaveJsonData(JsonGameData data)
	{
		if (_player != null) { data.PlayerMovement.PlayerPosition = _player.GlobalPosition; data.PlayerMovement.PlayerRotation = _player.GlobalTransform.Basis.GetRotationQuaternion(); }
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		SetPlayerPosition(data.PlayerMovement.PlayerPosition);
		if (_player != null) _player.GlobalTransform = new Transform3D(new Basis(data.PlayerMovement.PlayerRotation), data.PlayerMovement.PlayerPosition);
		return Task.CompletedTask;
	}
}