using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class KeysManager : Node, IJsonSaveLoad
{
	private readonly List<string> _collectedKeys = new();

	public List<string> CollectedKeys => _collectedKeys;

	public bool HasKey(string keyId) => _collectedKeys.Contains(keyId);

	public void Initialize()
	{
		GD.Print("KeysManager Initialized");
	}

	public void AddKey(string keyId)
	{
		if (!string.IsNullOrWhiteSpace(keyId) && !_collectedKeys.Contains(keyId))
		{
			_collectedKeys.Add(keyId);
		}
	}

	public void RemoveKey(string keyId)
	{
		_collectedKeys.Remove(keyId);
	}

	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerKeys = new List<string>(_collectedKeys);
		return Task.CompletedTask;
	}

	public Task LoadJsonData(JsonGameData data)
	{
		_collectedKeys.Clear();
		if (data.PlayerKeys != null)
		{
			_collectedKeys.AddRange(data.PlayerKeys);
		}

		return Task.CompletedTask;
	}
}