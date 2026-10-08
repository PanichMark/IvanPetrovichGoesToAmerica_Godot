using Godot;

public partial class PlayerColliderController : Node
{
	private Bootstrap _bootstrap;
	private PlayerMovementStateMachineController _states;
	private CollisionShape3D _shape;
	private Node3D _colliderNode;
	private Shape3D _standingShape;
	private Shape3D _crouchingShape;

	public void Initialize(Bootstrap bootstrap, PlayerMovementStateMachineController movementController)
	{
		_bootstrap = bootstrap;
		_states = movementController;
		_colliderNode = GetParent() as Node3D;
		_shape = FindChild("*", true, false) as CollisionShape3D;
		if (_shape != null)
			_standingShape = _shape.Shape?.Duplicate() as Shape3D;
	}

	public override void _Process(double delta)
	{
		if (_bootstrap?.IsBootstrapInitialized != true || _states == null || _shape == null)
			return;

		bool crouching = _states.CurrentPlayerMovementStateType is PlayerMovementStateTypes.PlayerIdleCrouhcing or PlayerMovementStateTypes.PlayerWalkingCrouching or PlayerMovementStateTypes.PlayerSliding or PlayerMovementStateTypes.PlayerJumping;
		if (crouching && _standingShape is CapsuleShape3D capsule)
		{
			if (_crouchingShape == null)
				_crouchingShape = new CapsuleShape3D { Radius = capsule.Radius, Height = Mathf.Max(capsule.Radius * 2f, capsule.Height * 0.55f) };
			_shape.Shape = _crouchingShape;
			_shape.Position = new Vector3(_shape.Position.X, 0.55f, _shape.Position.Z);
		}
		else if (_standingShape != null)
		{
			_shape.Shape = _standingShape;
			_shape.Position = new Vector3(_shape.Position.X, 1f, _shape.Position.Z);
		}
		_shape.Disabled = _states.CurrentPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbingStanding;
	}
}