using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerLegKickAttackController : Node
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private PlayerMovementController _movement;
	private PlayerMovementStateMachineController _movementStates;
	private Node3D _player;
	private PlayerWeaponController _weaponController;
	private bool _wasKickPressed;

	public bool IsPlayerLegKicking { get; private set; }
	public float WeaponDamage { get; private set; } = 50f;
	public event Action<bool> OnLegKickStateChanged;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		PlayerMovementController playerMovementController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		Node player,
		PlayerWeaponController playerWeaponController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_movement = playerMovementController;
		_movementStates = playerMovementStateMachineController;
		_player = player as Node3D;
		_weaponController = playerWeaponController;
	}

	public override void _Process(double delta)
	{
		bool kickPressed = _inputDevice?.GetKeyLegKick() == true;
		if (kickPressed && !_wasKickPressed && _bootstrap?.IsBootstrapInitialized == true &&
			!IsPlayerLegKicking && _weaponController?.HasAnyWeapon == true && CanKickFromCurrentState())
		{
			LegKick();
		}
		_wasKickPressed = kickPressed;
	}

	public void LegKick()
	{
		if (IsPlayerLegKicking || _player == null || !CanKickFromCurrentState())
			return;

		PlayerMovementStateTypes currentState = _movementStates.CurrentPlayerMovementStateType;
		bool crouching = currentState == PlayerMovementStateTypes.PlayerIdleCrouhcing ||
			currentState == PlayerMovementStateTypes.PlayerWalkingCrouching;
		_movementStates.SetPlayerMovementState(crouching
			? PlayerMovementStateTypes.PlayerIdleCrouhcing
			: PlayerMovementStateTypes.PlayerIdleStanding);
		_movement?.DisablePlayerMovementDuringLegKickAttack();
		IsPlayerLegKicking = true;
		OnLegKickStateChanged?.Invoke(true);
		ApplyKickDamage();
		FinishKickAfterDelay();
	}

	private bool CanKickFromCurrentState()
	{
		if (_movementStates == null)
			return false;

		return _movementStates.CurrentPlayerMovementStateType is
			PlayerMovementStateTypes.PlayerIdleStanding or
			PlayerMovementStateTypes.PlayerWalkingStanding or
			PlayerMovementStateTypes.PlayerRunning or
			PlayerMovementStateTypes.PlayerIdleCrouhcing or
			PlayerMovementStateTypes.PlayerWalkingCrouching;
	}

	private async void ApplyKickDamage()
	{
		World3D world = GetViewport()?.World3D;
		if (_player == null || world == null)
			return;

		Vector3 forward = -_player.GlobalBasis.Z;
		Vector3 capsuleCenter = _player.GlobalPosition + forward * 0.5f + Vector3.Up * 0.9f;
		CapsuleShape3D shape = new() { Radius = 0.3f, Height = 1.8f };
		Godot.Collections.Array<Rid> exclusions = new();
		if (_player is CollisionObject3D collisionObject)
			exclusions.Add(collisionObject.GetRid());

		PhysicsShapeQueryParameters3D query = new()
		{
			Shape = shape,
			Transform = new Transform3D(Basis.Identity, capsuleCenter),
			Exclude = exclusions,
			CollisionMask = uint.MaxValue
		};
		var hits = world.DirectSpaceState.IntersectShape(query, 32);
		await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);

		foreach (Godot.Collections.Dictionary hit in hits)
		{
			Node collider = hit["collider"].AsGodotObject() as Node;
			if (collider == null || collider == _player || !GodotObject.IsInstanceValid(collider))
				continue;

			if (collider.HasMethod("TakeDamage"))
				collider.Call("TakeDamage", WeaponDamage);
			if (collider.HasMethod("TakeBreakDamage"))
				collider.Call("TakeBreakDamage", WeaponDamage);
		}
	}

	private async void FinishKickAfterDelay()
	{
		await ToSignal(GetTree().CreateTimer(0.95), SceneTreeTimer.SignalName.Timeout);
		if (!GodotObject.IsInstanceValid(this))
			return;

		IsPlayerLegKicking = false;
		_movement?.ResumePlayerMovement();
		OnLegKickStateChanged?.Invoke(false);
	}
}