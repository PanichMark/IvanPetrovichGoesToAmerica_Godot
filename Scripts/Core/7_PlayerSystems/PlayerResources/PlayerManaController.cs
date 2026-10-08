using System.Threading.Tasks;
using Godot;

public partial class PlayerManaController : Node, IJsonSaveLoad
{
	private const int ManaItemEffect = 34;
	private const int MaxManaReplenishItems = 9;
	private const float ManaBarScale = 0.24f;
	private const float AutoRefillThreshold = 5f;
	private const float AutoRefillInterval = 1f;

	private Range _manaBar;
	private BaseButton _useManaItemButton;
	private Node _manaItemNumberNode;
	private Node _manaBarFillArea;
	private float _autoRefillElapsed;
	private bool _autoRefillActive;

	public float MaxPlayerMana { get; private set; } = 100f;
	public float CurrentPlayerMana { get; private set; }
	public int MaxManaReplenishItemsNumber { get; private set; } = MaxManaReplenishItems;
	public int CurrentManaReplenishItemsNumber { get; private set; }

	public void Initialize(ViewModelHUDHealthAndMana viewModelHUDHealthAndMana, ViewModelMenuWeaponWheel viewModelMenuWeaponWheel)
	{
		_manaBar = viewModelHUDHealthAndMana?.SliderManaBar as Range;
		_manaBarFillArea = viewModelHUDHealthAndMana?.SliderManaBarFillArea;
		_useManaItemButton = viewModelMenuWeaponWheel?.ButtonUseManaReplenishItem as BaseButton;
		_manaItemNumberNode = viewModelMenuWeaponWheel?.TextManaReplenishItemNumber;

		if (_manaBar != null)
			_manaBar.MaxValue = MaxPlayerMana;
		if (_useManaItemButton != null)
			_useManaItemButton.Pressed += UseManaReplenishItem;
		SetAutoRefill(CurrentPlayerMana < AutoRefillThreshold);

		GD.Print("PlayerManaController Initialized");
	}

	public override void _Process(double delta)
	{
		if (!_autoRefillActive || CurrentPlayerMana >= AutoRefillThreshold)
			return;

		_autoRefillElapsed += (float)delta;
		if (_autoRefillElapsed < AutoRefillInterval)
			return;

		_autoRefillElapsed -= AutoRefillInterval;
		CurrentPlayerMana = Mathf.Min(CurrentPlayerMana + 1f, AutoRefillThreshold);
		UpdateManaBar();
		if (CurrentPlayerMana >= AutoRefillThreshold)
			_autoRefillActive = false;
	}

	public void ConfigApplyPlayerMana(int setMana)
	{
		CurrentPlayerMana = Mathf.Clamp(setMana, 0f, MaxPlayerMana);
		UpdateManaBar();
		SetAutoRefill(CurrentPlayerMana < AutoRefillThreshold);
	}

	public void ConfigApplyPlayerManaReplenishItems(int setManaReplenishItems)
	{
		CurrentManaReplenishItemsNumber = Mathf.Clamp(setManaReplenishItems, 0, MaxManaReplenishItemsNumber);
		UpdateManaItemCount();
	}

	public void UseManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber <= 0)
		{
			GD.Print("0 ManaReplenish Items");
			return;
		}

		if (CurrentPlayerMana >= MaxPlayerMana)
		{
			GD.Print("Mana is already Full");
			return;
		}

		CurrentManaReplenishItemsNumber--;
		UpdateManaItemCount();
		ReplenishMana(ManaItemEffect);
		GD.Print("Used ManaReplenish Item");
	}

	public void AddManaReplenishItem()
	{
		if (CurrentManaReplenishItemsNumber >= MaxManaReplenishItemsNumber)
		{
			GD.Print("Max ManaReplenish Items");
			return;
		}

		CurrentManaReplenishItemsNumber++;
		UpdateManaItemCount();
		GD.Print("Added ManaReplenish Item");
	}

	public void ReplenishMana(int mana)
	{
		CurrentPlayerMana = Mathf.Clamp(CurrentPlayerMana + mana, 0f, MaxPlayerMana);
		UpdateManaBar();
		if (CurrentPlayerMana >= AutoRefillThreshold)
			SetAutoRefill(false);

		GD.Print($"replenished: {mana} mana");
	}

	public void UseMana(int manaCost)
	{
		CurrentPlayerMana = Mathf.Clamp(CurrentPlayerMana - manaCost, 0f, MaxPlayerMana);
		UpdateManaBar();
		if (CurrentPlayerMana < AutoRefillThreshold)
			SetAutoRefill(true);

		GD.Print($"used: {manaCost} mana");
	}

	public void ShowButtonUseManaReplenishItem()
	{
		SetVisible(_useManaItemButton, true);
	}

	public void HideButtonUseManaReplenishItem()
	{
		SetVisible(_useManaItemButton, false);
	}

	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerResources.PlayerMana = CurrentPlayerMana;
		data.PlayerResources.PlayerManaReplenishItemsNumber = CurrentManaReplenishItemsNumber;
		return Task.CompletedTask;
	}

	public Task LoadJsonData(JsonGameData data)
	{
		CurrentPlayerMana = Mathf.Clamp(data.PlayerResources.PlayerMana, 0f, MaxPlayerMana);
		CurrentManaReplenishItemsNumber = Mathf.Clamp(data.PlayerResources.PlayerManaReplenishItemsNumber, 0, MaxManaReplenishItemsNumber);
		UpdateManaBar();
		UpdateManaItemCount();
		SetAutoRefill(CurrentPlayerMana < AutoRefillThreshold);
		return Task.CompletedTask;
	}

	private void SetAutoRefill(bool enabled)
	{
		_autoRefillActive = enabled && CurrentPlayerMana < AutoRefillThreshold;
		_autoRefillElapsed = 0f;
	}

	private void UpdateManaBar()
	{
		if (GodotObject.IsInstanceValid(_manaBar))
			_manaBar.Value = CurrentPlayerMana * ManaBarScale;

		if (GodotObject.IsInstanceValid(_manaBarFillArea))
			_manaBarFillArea.Set("visible", CurrentPlayerMana > 0f);
	}

	private void UpdateManaItemCount()
	{
		SetText(_manaItemNumberNode, CurrentManaReplenishItemsNumber.ToString());
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
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
}