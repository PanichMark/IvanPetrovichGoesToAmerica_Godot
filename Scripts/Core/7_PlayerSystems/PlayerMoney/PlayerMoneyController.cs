using System.Threading.Tasks;
using Godot;

public partial class PlayerMoneyController : Node, IJsonSaveLoad
{
	private Node _moneyDisplay;
	public int PlayerMoney { get; private set; }

	public void Initialize(Node moneyDisplay)
	{
		_moneyDisplay = moneyDisplay;
		UpdateMoneyDisplay();
	}

	public void ConfigApplyPlayerMoney(int amount)
	{
		PlayerMoney = amount;
		UpdateMoneyDisplay();
	}
	public void AddMoney(int amount)
	{
		if (amount < 0) { GD.PushWarning("Can't add negative money."); return; }
		PlayerMoney = PlayerMoney > int.MaxValue - amount ? int.MaxValue : PlayerMoney + amount;
		UpdateMoneyDisplay();
	}
	public void DeductMoney(int amount)
	{
		if (amount > 0) { GD.PushWarning("Can't deduct positive money; pass a negative amount."); return; }
		if (amount < -PlayerMoney) { GD.PushWarning("Not enough money."); return; }
		PlayerMoney += amount;
		UpdateMoneyDisplay();
	}
	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerResources.PlayerMoney = PlayerMoney;
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		PlayerMoney = data.PlayerResources.PlayerMoney;
		UpdateMoneyDisplay();
		return Task.CompletedTask;
	}
	private void UpdateMoneyDisplay()
	{
		if (!GodotObject.IsInstanceValid(_moneyDisplay)) return;
		if (_moneyDisplay is Label label) label.Text = PlayerMoney.ToString();
		else if (_moneyDisplay is RichTextLabel richTextLabel) richTextLabel.Text = PlayerMoney.ToString();
	}
}