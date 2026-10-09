using System;
using Godot;

public partial class PlayerWeaponAnimationController : Node
{
	private PlayerWeaponController _weaponController;
	private PlayerLegKickAttackController _legKickController;
	private bool _initialized;

	public event Action OnPlayerStartedReloading;
	public event Action OnPlayerEndedReloading;
	public event Action<WeaponAbstract> OnShowWeapon;
	public event Action<WeaponAbstract> OnHideWeapon;
	public event Action<WeaponHandType> OnShowThirdPersonHand;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		PlayerInteractionController interactionController,
		PlayerWeaponController weaponController,
		PlayerLegKickAttackController legKickController,
		Node transferBonesFirstPerson,
		Node transferBonesThirdPerson,
		Node player,
		Node playerCamera)
	{
		_weaponController = weaponController;
		_legKickController = legKickController;
		if (_weaponController != null)
		{
			_weaponController.OnShowWeapon += HandleShowWeapon;
			_weaponController.OnHideWeapon += HandleHideWeapon;
		}
		if (_legKickController != null)
			_legKickController.OnLegKickStateChanged += HandleLegKickStateChanged;
		_initialized = true;
	}

	public void NotifyReloadStarted() => OnPlayerStartedReloading?.Invoke();
	public void NotifyReloadEnded() => OnPlayerEndedReloading?.Invoke();
	public void NotifyThirdPersonHandShown(WeaponHandType hand) => OnShowThirdPersonHand?.Invoke(hand);

	private void HandleShowWeapon(WeaponAbstract weapon)
	{
		if (_initialized)
			OnShowWeapon?.Invoke(weapon);
	}

	private void HandleHideWeapon(WeaponAbstract weapon)
	{
		if (_initialized)
			OnHideWeapon?.Invoke(weapon);
	}

	private void HandleLegKickStateChanged(bool isKicking)
	{
		// A scene can provide an AnimationPlayer on this controller's parent for its kick animation.
		AnimationPlayer animationPlayer = GetParent()?.FindDeepNodeOfType<AnimationPlayer>();
		if (animationPlayer == null)
			return;

		string animationName = isKicking ? "LegKick" : "RESET";
		if (animationPlayer.HasAnimation(animationName))
			animationPlayer.Play(animationName);
	}

	public override void _ExitTree()
	{
		if (_weaponController != null)
		{
			_weaponController.OnShowWeapon -= HandleShowWeapon;
			_weaponController.OnHideWeapon -= HandleHideWeapon;
		}
		if (_legKickController != null)
			_legKickController.OnLegKickStateChanged -= HandleLegKickStateChanged;
	}
}