using Godot;

public abstract partial class InteractionObjectPickableAbstract : RigidBody3D, IInteractable, IPickable
{
	[Export] protected string _interactionObjectNameSystem = string.Empty;
	[Export] protected InteractionObjectPickableData _interactionObjectPickableType;

	private CollisionShape3D[] _collisionShapes;
	private uint _originalCollisionLayer;
	private Tween _pickupTween;
	private LocalizationManager _localizationManager;
	private Node3D _pickupAnchor;
	private bool _isDestroyed;
	private uint _originalCollisionMask;

	public event IInteractable.InteractableObjectHandler OnInteract;
	public InteractionObjectsPickableTypes PickableType => _interactionObjectPickableType?.PickableType ?? InteractionObjectsPickableTypes.Crate;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; } = string.Empty;
	public string InteractionHintMessageAction { get; protected set; } = string.Empty;
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;
	public bool IsObjectPickedUp { get; protected set; }
	public bool IsObjectDestroyed => _isDestroyed;

	public override void _Ready()
	{
		_originalCollisionLayer = CollisionLayer;
		_originalCollisionMask = CollisionMask;
		if (CollisionLayer == 0)
			CollisionLayer = 1;
		_collisionShapes = FindCollisionShapes(this);
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>();
		UpdateLocalizedText();
		if (_localizationManager != null)
			_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void Interact()
	{
		PickUpObject(false);
		OnInteract?.Invoke();
	}

	public virtual void InteractCutscene()
	{
		PickUpObject(true);
		OnInteract?.Invoke();
	}

	public virtual void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		if (IsObjectPickedUp || _isDestroyed || !IsInsideTree())
			return;

		_pickupAnchor = ResolvePickupAnchor();
		if (_pickupAnchor == null)
			return;

		SetObjectPickedUp(true);
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;

		if (isPickedUpByLoadSafeFile)
		{
			AttachToPickupAnchor();
			return;
		}

		Vector3 target = _pickupAnchor.ToGlobal(GetPickupOffset());
		Reparent(GetTree().CurrentScene ?? GetParent(), true);
		_pickupTween?.Kill();
		_pickupTween = CreateTween();
		_pickupTween.TweenProperty(this, "global_position", target, 0.2f)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		_pickupTween.TweenCallback(Callable.From(AttachToPickupAnchor));
	}

	public virtual void DropOffObject()
	{
		if (!IsObjectPickedUp)
			return;

		_pickupTween?.Kill();
		_pickupTween = null;
		Node dropParent = GetTree().CurrentScene ?? GetParent();
		if (dropParent != null && GetParent() != dropParent)
			Reparent(dropParent, true);
		SetObjectPickedUp(false);
		GlobalPosition += -GlobalBasis.Z * 0.3f;
		_pickupAnchor = null;
	}

	protected void SetObjectPickedUp(bool pickedUp)
	{
		if (IsObjectPickedUp == pickedUp)
			return;
		IsObjectPickedUp = pickedUp;
		Freeze = pickedUp;
		CollisionLayer = pickedUp ? 0u : _originalCollisionLayer;
		CollisionMask = pickedUp ? 0u : _originalCollisionMask;
		SetCollisionShapesDisabled(pickedUp);
	}

	protected virtual Node3D ResolvePickupAnchor()
	{
		Node player = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
		return player?.FindDeepNode("Spine") as Node3D ?? player as Node3D;
	}

	protected Vector3 GetPickupOffset() => _interactionObjectPickableType?.Position ?? Vector3.Zero;

	protected virtual void AttachToPickupAnchor()
	{
		if (_pickupAnchor == null || !GodotObject.IsInstanceValid(_pickupAnchor))
			return;

		Reparent(_pickupAnchor, true);
		Position = GetPickupOffset();
		RotationDegrees = _interactionObjectPickableType?.RotationDegrees ?? Vector3.Zero;
	}

	protected void MarkDestroyed()
	{
		_isDestroyed = true;
		QueueFree();
	}

	private void SetCollisionShapesDisabled(bool disabled)
	{
		foreach (CollisionShape3D shape in _collisionShapes)
		{
			if (GodotObject.IsInstanceValid(shape))
				shape.SetDeferred("disabled", disabled);
		}
	}

	private void UpdateLocalizedText()
	{
		if (_localizationManager == null)
		{
			InteractionObjectNameUI = string.IsNullOrEmpty(_interactionObjectNameSystem) ? Name : _interactionObjectNameSystem;
			InteractionHintMessageAction = "Pick up";
			return;
		}

		InteractionObjectNameUI = string.IsNullOrEmpty(_interactionObjectNameSystem)
			? Name
			: _localizationManager.GetLocalizedString(_interactionObjectNameSystem, Name);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_PickUp", Name);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		UpdateLocalizedText();
	}

	private static CollisionShape3D[] FindCollisionShapes(Node root)
	{
		var shapes = new System.Collections.Generic.List<CollisionShape3D>();
		CollectCollisionShapes(root, shapes);
		return shapes.ToArray();
	}

	private static void CollectCollisionShapes(Node node, System.Collections.Generic.List<CollisionShape3D> shapes)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is CollisionShape3D shape)
				shapes.Add(shape);
			CollectCollisionShapes(child, shapes);
		}
	}

	public override void _ExitTree()
	{
		if (_localizationManager != null)
			_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_pickupTween?.Kill();
	}
}