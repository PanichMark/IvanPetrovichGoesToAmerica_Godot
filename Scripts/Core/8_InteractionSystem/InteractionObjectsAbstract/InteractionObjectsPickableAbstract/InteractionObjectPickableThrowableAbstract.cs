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

		Node3D player = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player) as Node3D;
		Camera3D camera = GetViewport().GetCamera3D();
		Vector3 throwDirection = camera?.GlobalBasis.Z * -1f ?? (player?.GlobalBasis.Z * -1f ?? -GlobalBasis.Z);
		DropOffObject();
		_wasThrown = true;
		ApplyCentralImpulse(throwDirection.Normalized() * ObjectThrowPower);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_wasThrown)
			return;

		for (int index = 0; index < GetContactCount(); index++)
		{
			GodotObject collider = GetContactColliderObject(index);
			if (collider is Node node && node.HasMethod("TakeDamage"))
				node.Call("TakeDamage", _damage);
			_wasThrown = false;
			break;
		}
	}

}