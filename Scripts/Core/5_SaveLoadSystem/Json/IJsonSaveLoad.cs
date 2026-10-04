using System.Threading.Tasks;

public interface IJsonSaveLoad
{
	Task SaveJsonData(JsonGameData data);
	Task LoadJsonData(JsonGameData data);
}