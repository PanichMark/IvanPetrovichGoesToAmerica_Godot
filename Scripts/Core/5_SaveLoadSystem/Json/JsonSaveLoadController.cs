using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>Coordinates JSON save slots and the scene-transition save/load handshake.</summary>
public partial class JsonSaveLoadController : Node
{
	private const string TempFileName = "SafeFile_TEMP.json";
	private const string SaveSlotPrefix = "SafeFileSlot_";
	private const string SaveSlotSuffix = ".json";

	private Bootstrap _bootstrap;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private string[] _saveFilePaths = Array.Empty<string>();
	private JsonFileDataHandler _fileDataHandler;
	private JsonGameData _gameData;
	private List<IJsonSaveLoad> _coreSaveLoadObjects = new();
	private List<IJsonSaveLoad> _gameplaySaveLoadObjects = new();
	private bool _coreObjectsFound;
	private bool _wasSavedToTempBeforeLoadingNewScene;
	private bool _isProcessingTransition;

	public event Action OnStartGameDataProcessForUI;
	public event Action OnEndGameDataProcessForUI;
	public event Action OnSafeFileDelete;
	public event Action OnSafeFileLoad;
	public event Action OnSafeFileSaved;

	public string SceneNameToLoad { get; private set; }
	public bool IsSavingFinished { get; private set; } = true;
	public bool IsLoadingFromSaveFile { get; private set; }
	public JsonGameData CurrentGameData => _gameData;

	public override void _EnterTree() => ProcessMode = ProcessModeEnum.Always;

	public void Initialize(Bootstrap bootstrap, GameScenesManager gameSceneManager, GameController gameController)
	{
		_bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
		_gameSceneManager = gameSceneManager ?? throw new ArgumentNullException(nameof(gameSceneManager));
		_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
		_saveFilePaths = Enumerable.Range(1, Math.Max(0, _bootstrap.GameData.NumberOfSafeFileSlots))
			.Select(slot => $"{SaveSlotPrefix}{slot}{SaveSlotSuffix}")
			.ToArray();

		_gameSceneManager.OnRequestSaveOldGameplayData -= HandleRequestSaveOldGameplayData;
		_gameSceneManager.OnRequestSaveOldGameplayData += HandleRequestSaveOldGameplayData;
		_gameSceneManager.OnEndLoadingGameplayScene -= HandleEndLoadingGameplayScene;
		_gameSceneManager.OnEndLoadingGameplayScene += HandleEndLoadingGameplayScene;
		_gameSceneManager.OnBeginLoadingMainMenuScene -= HandleBeginLoadingMainMenuScene;
		_gameSceneManager.OnBeginLoadingMainMenuScene += HandleBeginLoadingMainMenuScene;
		GD.Print("JsonSaveLoadController initialized.");
	}

	public async Task NewGame()
	{
		EnsureInitialized();
		DiscoverCoreSaveLoadObjects();
		_gameData = new JsonGameData();
		_fileDataHandler = CreateHandler(TempFileName);
		_fileDataHandler.Save(_gameData);

		foreach (IJsonSaveLoad saveLoadObject in _coreSaveLoadObjects)
			await saveLoadObject.LoadJsonData(_gameData);

		GD.Print("New game save data initialized.");
	}

	public async Task SaveGame(int saveSlotNumber)
	{
		EnsureInitialized();
		if (saveSlotNumber != -1 && !IsValidSlot(saveSlotNumber))
		{
			GD.PushError($"Invalid save slot: {saveSlotNumber}.");
			return;
		}
		if (_gameData == null)
		{
			GD.PushWarning("Cannot save: game data has not been initialized.");
			return;
		}

		IsSavingFinished = false;
		_fileDataHandler = CreateHandler(saveSlotNumber == -1 ? TempFileName : _saveFilePaths[saveSlotNumber - 1]);
		OnStartGameDataProcessForUI?.Invoke();
		try
		{
			RefreshGameplaySaveLoadObjects();
			foreach (IJsonSaveLoad saveLoadObject in _coreSaveLoadObjects)
				await saveLoadObject.SaveJsonData(_gameData);
			foreach (IJsonSaveLoad saveLoadObject in _gameplaySaveLoadObjects)
				await saveLoadObject.SaveJsonData(_gameData);

			_gameData.SafeFileDateAndTime = DateTime.Now.ToString("O");
			if (_gameSceneManager.CurrentScene.HasValue && Enum.TryParse(_gameSceneManager.CurrentScene.Value.ToString(), out GameScenesGameplayEnum scene))
				_gameData.Scene = scene;
			_fileDataHandler.Save(_gameData);

			if (saveSlotNumber != -1)
				OnSafeFileSaved?.Invoke();
			if (_gameSceneManager.WasInitialSceneLoaded)
				_gameSceneManager.SavedOldGameplayData();
		}
		finally
		{
			IsSavingFinished = true;
			OnEndGameDataProcessForUI?.Invoke();
		}
	}

	public async Task LoadGame(int loadSlotNumber)
	{
		EnsureInitialized();
		if (loadSlotNumber != -1 && !IsValidSlot(loadSlotNumber))
		{
			GD.PushError($"Invalid save slot: {loadSlotNumber}.");
			return;
		}

		DiscoverCoreSaveLoadObjects();
		_fileDataHandler = CreateHandler(loadSlotNumber == -1 ? TempFileName : _saveFilePaths[loadSlotNumber - 1]);
		JsonGameData loadedData = _fileDataHandler.Load();
		if (loadedData == null)
		{
			GD.PushWarning($"No valid save data found for slot {loadSlotNumber}.");
			return;
		}

		_gameData = loadedData;
		SceneNameToLoad = _gameData.Scene.ToString();
		OnSafeFileLoad?.Invoke();
		if (_gameController.IsMainMenuOrEndGameTitlesActive)
			_gameController.DeactivateMainMenuOrEndGameTitlesActive();

		IsLoadingFromSaveFile = loadSlotNumber != -1;
		OnStartGameDataProcessForUI?.Invoke();
		try
		{
			if (loadSlotNumber != -1)
			{
				if (!Enum.TryParse(SceneNameToLoad, out GameScenesGameplayEnum scene))
				{
					GD.PushError($"Save references an unknown gameplay scene '{SceneNameToLoad}'.");
					return;
				}
				_ = _gameSceneManager.LoadGameplayScene(scene);
				while (!_gameSceneManager.HasLoadedGameplayScene)
					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			}

			foreach (IJsonSaveLoad saveLoadObject in _coreSaveLoadObjects)
				await saveLoadObject.LoadJsonData(_gameData);

			RefreshGameplaySaveLoadObjects();
			foreach (IJsonSaveLoad saveLoadObject in _gameplaySaveLoadObjects.AsEnumerable().Reverse())
				await saveLoadObject.LoadJsonData(_gameData);

			_gameSceneManager.ApplyGameplayDataLoadingFinished();
			if (loadSlotNumber != -1)
			{
				CopySlotToTemp(loadSlotNumber);
				_wasSavedToTempBeforeLoadingNewScene = true;
			}
		}
		finally
		{
			IsLoadingFromSaveFile = false;
			OnEndGameDataProcessForUI?.Invoke();
		}
	}

	public void DeleteGame(int deleteSlotNumber)
	{
		if (!IsValidSlot(deleteSlotNumber))
		{
			GD.PushError($"Invalid save slot for deletion: {deleteSlotNumber}.");
			return;
		}

		string path = GetSavePath(_saveFilePaths[deleteSlotNumber - 1]);
		try
		{
			if (!File.Exists(path))
			{
				GD.PushWarning($"No save file exists at slot {deleteSlotNumber}.");
				return;
			}
			File.Delete(path);
			OnSafeFileDelete?.Invoke();
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not delete save slot {deleteSlotNumber}: {exception}");
		}
	}

	public (string SavefileDateAndTime, GameScenesGameplayEnum SafefileSceneNameSystem)[] GetExtendedSaveInfo()
	{
		var details = new (string SavefileDateAndTime, GameScenesGameplayEnum SafefileSceneNameSystem)[_saveFilePaths.Length];
		for (int index = 0; index < _saveFilePaths.Length; index++)
		{
			JsonGameData save = CreateHandler(_saveFilePaths[index]).Load();
			details[index] = save == null ? (null, default) : (save.SafeFileDateAndTime, save.Scene);
		}
		return details;
	}

	public Task UpdateGameplaySaveLoadObjectsIndexes()
	{
		RefreshGameplaySaveLoadObjects();
		return Task.CompletedTask;
	}

	private async void HandleRequestSaveOldGameplayData()
	{
		if (_isProcessingTransition)
			return;
		_isProcessingTransition = true;
		try
		{
			if (!_wasSavedToTempBeforeLoadingNewScene && _gameSceneManager.CurrentScene.HasValue && IsGameplayScene(_gameSceneManager.CurrentScene.Value))
				await SaveGame(-1);
			else
				_gameSceneManager.SavedOldGameplayData();
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not persist gameplay data before scene transition: {exception}");
			_gameSceneManager.SavedOldGameplayData();
		}
		finally
		{
			_wasSavedToTempBeforeLoadingNewScene = false;
			_isProcessingTransition = false;
		}
	}

	private async void HandleEndLoadingGameplayScene()
	{
		if (IsLoadingFromSaveFile)
			return;
		try
		{
			await LoadGame(-1);
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not restore gameplay data after scene transition: {exception}");
		}
		finally
		{
			_gameSceneManager.ApplyGameplayDataLoadingFinished();
		}
	}

	private async void HandleBeginLoadingMainMenuScene()
	{
		if (!_gameSceneManager.WasInitialSceneLoaded)
			return;
		try
		{
			await NewGame();
		}
		catch (Exception exception)
		{
			GD.PushError($"Could not initialize new-game save data: {exception}");
		}
	}

	private void DiscoverCoreSaveLoadObjects()
	{
		if (_coreObjectsFound)
			return;
		_coreSaveLoadObjects = FindSaveLoadObjects(_bootstrap.GetTree().Root)
			.Where(item => item is not GameplayObjectJsonSaveLoad && item is not JsonSaveLoadController)
			.ToList();
		_coreObjectsFound = true;
	}

	private void RefreshGameplaySaveLoadObjects()
	{
		_gameplaySaveLoadObjects = _gameSceneManager.CurrentScene.HasValue
			? FindSaveLoadObjects(_gameSceneManager.CurrentGameplaySceneNode)
			.OfType<GameplayObjectJsonSaveLoad>().Cast<IJsonSaveLoad>().ToList()
			: new List<IJsonSaveLoad>();
		GameplayObjectJsonSaveLoad[] indexedObjects = _gameplaySaveLoadObjects
			.OfType<GameplayObjectJsonSaveLoad>()
			.OrderBy(item => item.Name.ToString(), StringComparer.Ordinal)
			.ToArray();
		for (int index = 0; index < indexedObjects.Length; index++)
			indexedObjects[index].AssignGameplayObjectIndex(index);
	}

	private static IEnumerable<IJsonSaveLoad> FindSaveLoadObjects(Node root)
	{
		if (root == null || !GodotObject.IsInstanceValid(root))
			yield break;
		if (root is IJsonSaveLoad saveLoadObject)
			yield return saveLoadObject;
		foreach (Node child in root.GetChildren())
			foreach (IJsonSaveLoad nested in FindSaveLoadObjects(child))
				yield return nested;
	}

	private void CopySlotToTemp(int slotNumber)
	{
		string source = GetSavePath(_saveFilePaths[slotNumber - 1]);
		string destination = GetSavePath(TempFileName);
		if (File.Exists(source))
			File.Copy(source, destination, true);
	}

	private JsonFileDataHandler CreateHandler(string fileName) => new(ProjectSettings.GlobalizePath("user://"), fileName);
	private static string GetSavePath(string fileName) => Path.Combine(ProjectSettings.GlobalizePath("user://"), fileName);
	private bool IsValidSlot(int slotNumber) => slotNumber > 0 && slotNumber <= _saveFilePaths.Length;
	private void EnsureInitialized()
	{
		if (_bootstrap == null || _gameSceneManager == null || _gameController == null)
			throw new InvalidOperationException("JsonSaveLoadController.Initialize must be called before use.");
	}

	private static bool IsGameplayScene(GameScenesSystemEnum scene) => Enum.TryParse(scene.ToString(), out GameScenesGameplayEnum _);
}