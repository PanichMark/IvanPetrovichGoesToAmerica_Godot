using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerBehaviourController : Node, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _input;
	private GameScenesManager _sceneManager;
	private bool _initialized;
	private int _peopleKilled;
	private int _timesSpotted;
	private GameCityState _cityState;

	public event Action OnPlayerArmed;
	public event Action OnPlayerDisarmed;
	public bool WasPlayerArmed { get; private set; }
	public bool IsPlayerArmed { get; private set; }
	public GameCityState CityState => _cityState;
	public int TimesPlayerSpottedGameTotal => _timesSpotted;
	public int PeopleKilledGameTotal => _peopleKilled;

	public void Initialize(Bootstrap bootstrap, IInputDevice inputDevice, GameScenesManager scenesManager)
	{
		_bootstrap = bootstrap;
		_input = inputDevice;
		_sceneManager = scenesManager;
		if (_sceneManager != null)
			_sceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += DisarmPlayer;
		_initialized = true;
	}

	public override void _Process(double delta)
	{
		if (_initialized && _bootstrap?.IsBootstrapInitialized == true && _input?.GetKeyHideWeapons() == true)
			DisarmPlayer();
	}

	public void OnPlayerSpotted() => _timesSpotted++;
	public void OnHumanNPCkilled()
	{
		_peopleKilled++;
		_cityState = _peopleKilled >= 30 ? GameCityState.Curfew : _peopleKilled >= 10 ? GameCityState.Agitatated : GameCityState.Normal;
	}
	public void ArmPlayer()
	{
		if (IsPlayerArmed) return;
		IsPlayerArmed = true;
		WasPlayerArmed = false;
		OnPlayerArmed?.Invoke();
	}
	public void DisarmPlayer()
	{
		if (!IsPlayerArmed) { WasPlayerArmed = false; return; }
		IsPlayerArmed = false;
		WasPlayerArmed = true;
		OnPlayerDisarmed?.Invoke();
	}
	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerBehaviour.IsPlayerArmed = IsPlayerArmed;
		data.PlayerBehaviour.WasPlayerArmed = WasPlayerArmed;
		data.PlayerBehaviour.TimesPlayerSpottedGameTotal = _timesSpotted;
		data.PlayerBehaviour.PeopleKilledGameTotal = _peopleKilled;
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		bool wasArmed = IsPlayerArmed;
		IsPlayerArmed = data.PlayerBehaviour.IsPlayerArmed;
		WasPlayerArmed = data.PlayerBehaviour.WasPlayerArmed;
		_timesSpotted = data.PlayerBehaviour.TimesPlayerSpottedGameTotal;
		_peopleKilled = data.PlayerBehaviour.PeopleKilledGameTotal;
		_cityState = _peopleKilled >= 30 ? GameCityState.Curfew : _peopleKilled >= 10 ? GameCityState.Agitatated : GameCityState.Normal;
		if (wasArmed != IsPlayerArmed)
			(IsPlayerArmed ? OnPlayerArmed : OnPlayerDisarmed)?.Invoke();
		return Task.CompletedTask;
	}
	public override void _ExitTree()
	{
		if (_sceneManager != null) _sceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= DisarmPlayer;
	}
}