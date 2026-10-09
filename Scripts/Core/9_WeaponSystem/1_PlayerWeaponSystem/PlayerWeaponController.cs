using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerWeaponController : Node
{
	private readonly Dictionary<string, PackedScene> _unlockedWeapons = new(StringComparer.OrdinalIgnoreCase);
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerInteractionController _interactionController;
	private PlayerWeaponAmmoController _ammoManager;
	private Node3D _leftSlot;
	private Node3D _rightSlot;
	private bool _isLeftHand;

	public IReadOnlyDictionary<string, PackedScene> UnlockedWeapons => _unlockedWeapons;
	public bool WasRightButtonPressedLastFrame { get; private set; }
	public bool WasLeftButtonPressedLastFrame { get; private set; }
	public bool IsLeftHand => _isLeftHand;
	public bool IsAbleToUseRightWeapon { get; private set; } = true;
	public bool IsAbleToUseLeftWeapon { get; private set; } = true;
	public bool HasAnyWeapon => _unlockedWeapons.Count > 0;
	public Node3D LeftHandWeapon { get; private set; }
	public Node3D RightHandWeapon { get; private set; }
	public WeaponAbstract LeftHandWeaponComponent { get; private set; }
	public WeaponAbstract RightHandWeaponComponent { get; private set; }

	public delegate void WeaponVisibilityHandler(WeaponAbstract weapon);
	public delegate void WeaponShootHandler(WeaponHandType weaponHandType);
	public delegate void WeaponUnlockHandler(PackedScene weaponScene);
	public delegate void WeaponChangeHandler(WeaponHandType activeHand);
	public event WeaponVisibilityHandler OnShowWeapon;
	public event WeaponVisibilityHandler OnHideWeapon;
	public event WeaponShootHandler OnWeaponShoot;
	public event WeaponUnlockHandler OnAnyWeaponUnlocked;
	public event WeaponChangeHandler OnWeaponChanged;

	public void Initialize(Bootstrap bootstrap, GameController gameController, IInputDevice inputDevice, MenuManager menuManager,
		PlayerBehaviourController playerBehaviour, PlayerManaController playerManaController,
		HUDhealthAndManaController hudHealthAndManaController, PlayerWeaponAmmoController ammoManager,
		PlayerInteractionController interactionController)
	{
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_playerBehaviour = playerBehaviour;
		_ammoManager = ammoManager;
		_interactionController = interactionController;
		_playerBehaviour.OnPlayerArmed += OnPlayerArmed;
		_playerBehaviour.OnPlayerDisarmed += OnPlayerDisarmed;
		if (_interactionController != null)
		{
			_interactionController.OnPickUpNonThrowable += OnPickUpNonThrowable;
			_interactionController.OnPickUpThrowable += OnPickUpThrowable;
			_interactionController.OnGetRidOfNonThrowable += OnGetRidOfPickable;
			_interactionController.OnGetRidOfThrowable += OnGetRidOfPickable;
		}
	}

	public void ConfigureWeaponSlots(Node firstPersonRight, Node firstPersonLeft, Node thirdPersonRight = null, Node thirdPersonLeft = null)
	{
		_rightSlot = firstPersonRight as Node3D;
		_leftSlot = firstPersonLeft as Node3D;
	}

	public override void _Process(double delta)
	{
		if (_inputDevice == null || _menuManager?.IsAnyMenuOpened == true) return;
		bool right = _inputDevice.GetKeyRightHandWeaponAttack();
		bool left = _inputDevice.GetKeyLeftHandWeaponAttack();
		if (_inputDevice.GetKeyRightHandWeaponAttackReleased())
		{
			RightHandWeaponComponent?.StopAutoAttackingWeaponPlayer();
			WasRightButtonPressedLastFrame = false;
		}
		if (right && !WasRightButtonPressedLastFrame && IsAbleToUseRightWeapon)
		{
			if (RightHandWeapon?.Visible == true) RightWeaponAttack();
			else if (RightHandWeapon != null) _playerBehaviour?.ArmPlayer();
			WasRightButtonPressedLastFrame = true;
		}
		if (_inputDevice.GetKeyLeftHandWeaponAttackReleased())
		{
			LeftHandWeaponComponent?.StopAutoAttackingWeaponPlayer();
			WasLeftButtonPressedLastFrame = false;
		}
		if (left && !WasLeftButtonPressedLastFrame && IsAbleToUseLeftWeapon)
		{
			if (LeftHandWeapon?.Visible == true) LeftWeaponAttack();
			else if (LeftHandWeapon != null) _playerBehaviour?.ArmPlayer();
			WasLeftButtonPressedLastFrame = true;
		}
		if (_inputDevice.GetKeyReload() && _interactionController?.CurrentIThrowable == null)
		{
			(LeftHandWeaponComponent as WeaponRangedAbstract)?.Reload();
			(RightHandWeaponComponent as WeaponRangedAbstract)?.Reload();
		}
	}

	public override void _ExitTree()
	{
		if (_playerBehaviour != null)
		{
			_playerBehaviour.OnPlayerArmed -= OnPlayerArmed;
			_playerBehaviour.OnPlayerDisarmed -= OnPlayerDisarmed;
		}
		if (_interactionController != null)
		{
			_interactionController.OnPickUpNonThrowable -= OnPickUpNonThrowable;
			_interactionController.OnPickUpThrowable -= OnPickUpThrowable;
			_interactionController.OnGetRidOfNonThrowable -= OnGetRidOfPickable;
			_interactionController.OnGetRidOfThrowable -= OnGetRidOfPickable;
		}
	}

	public void RightWeaponAttack() => Attack(WeaponHandType.Right);
	public void LeftWeaponAttack() => Attack(WeaponHandType.Left);
	public void StopAutoShootingRightWeaponPlayer() => RightHandWeaponComponent?.StopAutoAttackingWeaponPlayer();
	public void StopAutoShootingLeftWeaponPlayer() => LeftHandWeaponComponent?.StopAutoAttackingWeaponPlayer();

	public void Attack(WeaponHandType hand)
	{
		WeaponAbstract weapon = hand == WeaponHandType.Right ? RightHandWeaponComponent : LeftHandWeaponComponent;
		bool allowed = hand == WeaponHandType.Right ? IsAbleToUseRightWeapon : IsAbleToUseLeftWeapon;
		if (!allowed || weapon == null || weapon.Visible == false) return;
		weapon.WeaponPlayerAttack();
		OnWeaponShoot?.Invoke(hand);
	}

	public void ShowWeapon(WeaponHandType hand)
	{
		WeaponAbstract weapon = hand == WeaponHandType.Right ? RightHandWeaponComponent : LeftHandWeaponComponent;
		weapon?.ShowWeapon();
		if (weapon != null) OnShowWeapon?.Invoke(weapon);
	}

	public void HideWeapon(WeaponHandType hand)
	{
		WeaponAbstract weapon = hand == WeaponHandType.Right ? RightHandWeaponComponent : LeftHandWeaponComponent;
		weapon?.HideWeapon();
		if (weapon != null) OnHideWeapon?.Invoke(weapon);
	}

	public void ChangeWeapon(WeaponHandType hand, Node3D weaponNode)
	{
		if (weaponNode is not WeaponAbstract weapon) weapon = weaponNode?.GetNodeOrNull<WeaponAbstract>();
		if (weapon == null) return;
		if (hand == WeaponHandType.Right)
		{
			if (RightHandWeapon != null) RightHandWeapon.QueueFree();
			RightHandWeapon = weaponNode;
			RightHandWeaponComponent = weapon;
			if (_rightSlot != null && weaponNode.GetParent() != _rightSlot) weaponNode.Reparent(_rightSlot, false);
		}
		else
		{
			if (LeftHandWeapon != null) LeftHandWeapon.QueueFree();
			LeftHandWeapon = weaponNode;
			LeftHandWeaponComponent = weapon;
			if (_leftSlot != null && weaponNode.GetParent() != _leftSlot) weaponNode.Reparent(_leftSlot, false);
		}
		weapon.InitializeWeaponPlayer();
		weapon.HideWeapon();
		OnWeaponChanged?.Invoke(hand);
	}

	public void UnlockWeapon(PackedScene weaponScene)
	{
		if (weaponScene == null) return;
		Node instance = weaponScene.Instantiate();
		string key = instance.Name.ToString();
		instance.Free();
		if (!TryExtractWeaponIndex(key, out _))
		{
			GD.PushWarning($"Weapon scene '{key}' should use the 'idx<number>_<name>' naming format.");
			return;
		}
		_unlockedWeapons[key] = weaponScene;
		OnAnyWeaponUnlocked?.Invoke(weaponScene);
	}

	public int ExtractWeaponIndex(string name) => TryExtractWeaponIndex(name, out int index) ? index : -1;
	public List<Node3D> CollectActiveWeapons()
	{
		var result = new List<Node3D>();
		foreach (PackedScene scene in _unlockedWeapons.Values)
		{
			Node node = scene.Instantiate();
			if (node is Node3D node3D) result.Add(node3D);
			else node.Free();
		}
		return result;
	}

	public bool IsLeftHandWeaponSelected() => _isLeftHand;
	public void SetActiveHand(WeaponHandType hand) { _isLeftHand = hand == WeaponHandType.Left; OnWeaponChanged?.Invoke(hand); }

	private static bool TryExtractWeaponIndex(string name, out int index)
	{
		index = -1;
		if (string.IsNullOrEmpty(name)) return false;
		int start = name.IndexOf("idx", StringComparison.OrdinalIgnoreCase);
		if (start < 0) return false;
		start += 3;
		int end = name.IndexOf('_', start);
		return end > start && int.TryParse(name.Substring(start, end - start), out index);
	}

	private void OnPlayerArmed()
	{
		if (RightHandWeaponComponent != null && _interactionController?.CurrentIThrowable == null) ShowWeapon(WeaponHandType.Right);
		if (LeftHandWeaponComponent != null) ShowWeapon(WeaponHandType.Left);
	}
	private void OnPlayerDisarmed()
	{
		HideWeapon(WeaponHandType.Right);
		HideWeapon(WeaponHandType.Left);
	}
	private void OnPickUpNonThrowable(InteractionObjectsPickableTypes type) { IsAbleToUseRightWeapon = false; IsAbleToUseLeftWeapon = false; }
	private void OnPickUpThrowable(InteractionObjectsPickableTypes type) { IsAbleToUseRightWeapon = false; IsAbleToUseLeftWeapon = true; HideWeapon(WeaponHandType.Right); }
	private void OnGetRidOfPickable() { IsAbleToUseRightWeapon = true; IsAbleToUseLeftWeapon = true; if (_playerBehaviour?.IsPlayerArmed == true) ShowWeapon(WeaponHandType.Right); }
}