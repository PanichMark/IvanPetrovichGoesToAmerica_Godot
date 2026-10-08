using System.Threading.Tasks;
using Godot;

public partial class PlayerHealthController : Node, IJsonSaveLoad
{
	private const float MinFallDurationForDamage = 1f;
	private const float DamagePerSecondOverThreshold = 0.5f;
	private const int HealingItemEffect = 34;
	private const int MaxHealingItems = 9;
	private const float HealthBarScale = 0.23f;

	private GameController _gameController;
	private Node _movementAnimationController;
	private PlayerMovementStateMachineController _movementStateMachine;
	private Range _healthBar;
	private BaseButton _useHealingItemButton;
	private Node _healingItemNumberNode;
	private bool _isFalling;
	private double _fallStartTime;
	private bool _deathStarted;

	public float MaxPlayerHealth { get; private set; } = 100f;
	public float CurrentPlayerHealth { get; private set; }
	public int MaxHealingItemsNumber { get; private set; } = MaxHealingItems;
	public int CurrentHealingItemsNumber { get; private set; }
	public bool IsObjectDestroyed => false;
	public float CurrentHealth => CurrentPlayerHealth;
	public bool CanObjectBeDamaged => true;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		Node playerMovementAnimationController,
		Node playerMovementStateMachineController,
		ViewModelHUDHealthAndMana viewModelHUDHealthAndMana,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel)
	{
		_gameController = gameController;
		_movementAnimationController = playerMovementAnimationController;
		_movementStateMachine = playerMovementStateMachineController as PlayerMovementStateMachineController;
		_healthBar = viewModelHUDHealthAndMana?.SliderHealthBar as Range;
		_useHealingItemButton = viewModelMenuWeaponWheel?.ButtonUseHealingItem as BaseButton;
		_healingItemNumberNode = viewModelMenuWeaponWheel?.TextHealingItemNumber;

		if (_healthBar != null)
			_healthBar.MaxValue = MaxPlayerHealth;
		if (_useHealingItemButton != null)
			_useHealingItemButton.Pressed += UseHealingItem;
		if (_movementStateMachine != null)
			_movementStateMachine.OnChangeMovementState += HandleMovementStateChanged;

		GD.Print("PlayerHealthController Initialized");
	}

	public void ConfigApplyPlayerHealth(int setHealth)
	{
		CurrentPlayerHealth = Mathf.Clamp(setHealth, 0f, MaxPlayerHealth);
		UpdateHealthBar();
	}

	public void ConfigApplyPlayerHealingItems(int setHealingItems)
	{
		CurrentHealingItemsNumber = Mathf.Clamp(setHealingItems, 0, MaxHealingItemsNumber);
		UpdateHealingItemCount();
	}

	public void UseHealingItem()
	{
		if (CurrentHealingItemsNumber <= 0)
		{
			GD.Print("0 Healing Items");
			return;
		}

		if (CurrentPlayerHealth >= MaxPlayerHealth)
		{
			GD.Print("Health is already Full");
			return;
		}

		CurrentHealingItemsNumber--;
		CurrentPlayerHealth = Mathf.Min(CurrentPlayerHealth + HealingItemEffect, MaxPlayerHealth);
		UpdateHealthBar();
		UpdateHealingItemCount();
		GD.Print("Used Healing Item");
	}

	public void AddHealingItem()
	{
		if (CurrentHealingItemsNumber >= MaxHealingItemsNumber)
		{
			GD.Print("Max Healing Items");
			return;
		}

		CurrentHealingItemsNumber++;
		UpdateHealingItemCount();
		GD.Print("Added Healing Item");
	}

	public void ReceiveHealth(float health)
	{
		CurrentPlayerHealth = Mathf.Clamp(CurrentPlayerHealth + health, 0f, MaxPlayerHealth);
		UpdateHealthBar();
	}

	public void TakeDamage(float amount)
	{
		if (amount <= 0f || _deathStarted)
			return;

		GD.Print($"Player is damaged by: {amount}");
		CurrentPlayerHealth = Mathf.Max(CurrentPlayerHealth - amount, 0f);
		UpdateHealthBar();

		if (CurrentPlayerHealth <= 0f)
			ObjectIsFullyDamaged();
	}

	public void HandleMovementStateChanged(PlayerMovementStateTypes movementState)
	{
		if (movementState == PlayerMovementStateTypes.PlayerFalling)
		{
			if (!_isFalling)
			{
				_isFalling = true;
				_fallStartTime = Time.GetTicksMsec() / 1000.0;
			}
			return;
		}

		if (_isFalling)
		{
			_isFalling = false;
			ApplyFallDamage();
		}
	}

	public void ObjectIsFullyDamaged()
	{
		if (_deathStarted)
			return;

		_deathStarted = true;
		CurrentPlayerHealth = 0f;
		UpdateHealthBar();

		if (GodotObject.IsInstanceValid(_movementAnimationController) &&
			_movementAnimationController.HasMethod("PlayerDeathAnimation"))
		{
			_movementAnimationController.Call("PlayerDeathAnimation");
		}

		if (_gameController != null)
			_ = _gameController.PlayerHasDied();
	}

	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerResources.PlayerHealth = CurrentPlayerHealth;
		data.PlayerResources.PlayerHealingItemsNumber = CurrentHealingItemsNumber;
		return Task.CompletedTask;
	}

	public Task LoadJsonData(JsonGameData data)
	{
		CurrentPlayerHealth = Mathf.Clamp(data.PlayerResources.PlayerHealth, 0f, MaxPlayerHealth);
		CurrentHealingItemsNumber = Mathf.Clamp(data.PlayerResources.PlayerHealingItemsNumber, 0, MaxHealingItemsNumber);
		_deathStarted = CurrentPlayerHealth <= 0f;
		UpdateHealthBar();
		UpdateHealingItemCount();
		return Task.CompletedTask;
	}

	private void ApplyFallDamage()
	{
		var elapsed = (Time.GetTicksMsec() / 1000.0 - _fallStartTime) * Engine.TimeScale;
		if (elapsed <= MinFallDurationForDamage)
			return;

		var fullSecondsOverThreshold = Mathf.FloorToInt((float)(elapsed - MinFallDurationForDamage));
		var baseDamage = MaxPlayerHealth * DamagePerSecondOverThreshold;
		TakeDamage(baseDamage * Mathf.Pow(2f, fullSecondsOverThreshold));
	}

	private void UpdateHealthBar()
	{
		if (GodotObject.IsInstanceValid(_healthBar))
			_healthBar.Value = CurrentPlayerHealth * HealthBarScale;
	}

	private void UpdateHealingItemCount()
	{
		SetText(_healingItemNumberNode, CurrentHealingItemsNumber.ToString());
	}

	private static void SetText(Node node, string text)
	{
		if (!GodotObject.IsInstanceValid(node))
			return;

		if (node is Label label)
			label.Text = text;
		else if (node is RichTextLabel richTextLabel)
			richTextLabel.Text = text;
	}

	public override void _ExitTree()
	{
		if (_movementStateMachine != null)
			_movementStateMachine.OnChangeMovementState -= HandleMovementStateChanged;
		if (_useHealingItemButton != null)
			_useHealingItemButton.Pressed -= UseHealingItem;
	}
}