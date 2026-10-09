using Godot;

public abstract partial class InteractionObjectPickableThrowableAbstract : InteractionObjectPickableAbstract, IThrowable
{
	[Export] protected float _damage;
	[Export] protected bool _canDamageBreakable;

	private bool _wasThrown;

	public float ObjectThrowPower { get; set; } = 10f;

	public virtual void ThrowObject()
	{
		if (!IsObjectPickedUp)
			return;

		Camera3D camera = GetViewport().GetCamera3D();
		Vector3 throwDirection = camera != null ? -camera.GlobalBasis.Z : -GlobalBasis.Z;
		if (camera != null)
		{
			float pitch = camera.GlobalRotation.X;
			throwDirection -= camera.GlobalBasis.Y * Mathf.Tan(pitch);
		}
		DropOffObject();
		_wasThrown = true;
		ApplyCentralImpulse(throwDirection.Normalized() * ObjectThrowPower);
	}

	public override void _Ready()
	{
		base._Ready();
		ContactMonitor = true;
		MaxContactsReported = 4;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_wasThrown)
			return;

		for (int index = 0; index < GetContactCount(); index++)
		{
			GodotObject collider = GetContactColliderObject(index);
			if (collider is Node node && node != this && node.HasMethod("TakeDamage"))
				node.Call("TakeDamage", _damage);
			if (_canDamageBreakable && collider is Node breakableNode && breakableNode != this && breakableNode.HasMethod("TakeBreakDamage"))
				breakableNode.Call("TakeBreakDamage", _damage);
			_wasThrown = false;
			break;
		}
	}

}