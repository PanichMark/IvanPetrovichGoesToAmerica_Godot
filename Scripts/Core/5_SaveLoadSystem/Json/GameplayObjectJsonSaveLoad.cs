using System.Threading.Tasks;
using Godot;

/// <summary>Base class for scene-owned nodes whose state is stored by the save system.</summary>
public partial class GameplayObjectJsonSaveLoad : Node, IJsonSaveLoad
{
	public int GameplayObjectIndex { get; protected set; }

	public virtual void AssignGameplayObjectIndex(int index) => GameplayObjectIndex = index;
	public virtual Task LoadJsonData(JsonGameData data) => Task.CompletedTask;
	public virtual Task SaveJsonData(JsonGameData data) => Task.CompletedTask;
	public virtual void ChildClassSave(JsonGameData data) { }
	public virtual void ChildClassLoad(JsonGameData data) { }
}