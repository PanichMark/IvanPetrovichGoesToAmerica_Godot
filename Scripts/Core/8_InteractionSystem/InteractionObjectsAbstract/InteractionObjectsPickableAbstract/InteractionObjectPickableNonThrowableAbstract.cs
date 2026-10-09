public abstract partial class InteractionObjectPickableNonThrowableAbstract : InteractionObjectPickableAbstract
{
	[Godot.Export] protected bool _isMovementRestricted;

	private GameController _gameController;

	public override void _Ready()
	{
		base._Ready();
		_gameController = ServiceLocator.Resolve<GameController>();
	}

	public override void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		bool wasPickedUp = IsObjectPickedUp;
		base.PickUpObject(isPickedUpByLoadSafeFile);
		if (!wasPickedUp && IsObjectPickedUp && _isMovementRestricted)
			_gameController?.RestrictPlayerMovementWhileCarryingNonThrowable();
	}

	public override void DropOffObject()
	{
		bool wasPickedUp = IsObjectPickedUp;
		base.DropOffObject();
		if (wasPickedUp && !IsObjectPickedUp && _isMovementRestricted)
			_gameController?.UnrestrictPlayerMovementWhileCarryingNonThrowable();
	}
}