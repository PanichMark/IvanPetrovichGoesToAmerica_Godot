using Godot;
using System.Collections;
using System.Threading.Tasks;

public partial class HUDweaponsController : Node, IJsonSaveLoad
{
	private MenuManager _menuManager;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private Node _canvasHUDammo;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private PlayerWeaponController _playerWeaponController;
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerInteractionController _interactionController;

	private Node _HUDammo;

	private Node _rightWeaponAmmoMagazine;
	private Node _rightWeaponAmmoReserve;
	private Node _rightWeaponAmmoBox;
	private Node _leftWeaponAmmoMagazine;
	private Node _leftWeaponAmmoReserve;
	private Node _leftWeaponAmmoBox;

	private Label _rightWeaponAmmoMagazineText;
	private Label _rightWeaponAmmoReserveText;
	private Label _leftWeaponAmmoMagazineText;
	private Label _leftWeaponAmmoReserveText;

	private Node _HUDcrosshairs;

	private Node _crosshairRevolver;

	private Node _crosshairAutoPistol;
	private Node[] _listCrosshairPartsAutoPistol = new Node[4];

	private Node _crosshairShotgun;
	private Node[] _listCrosshairPartsShotgun = new Node[4];

	private Node _crosshairTranquilizer;

	private Node _crosshairCrossbow;
	private Node[] _listCrosshairTypesCrossbow = new Node[4];

	public void Initialize(
		GameController gameController,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PlayerBehaviourController playerBehaviour,
		PlayerWeaponController weaponController,
		PlayerWeaponAmmoController playerResourcesAmmoManager,
		PlayerInteractionController interactionController,
		Node canvasHUDammo,
		ViewModelHUDWeapons viewModelHUDWeapons)
	{
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasHUDammo = canvasHUDammo;
		_playerWeaponController = weaponController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_gameController	= gameController;
		_playerBehaviour = playerBehaviour;
		_interactionController = interactionController;

		_HUDammo = viewModelHUDWeapons.HUDammo;

		_rightWeaponAmmoMagazine = viewModelHUDWeapons.TextRightWeaponAmmoMagazineNumber;
		_rightWeaponAmmoReserve = viewModelHUDWeapons.TextRightWeaponAmmoReserveNumber;
		_rightWeaponAmmoBox = viewModelHUDWeapons.RightWeaponAmmoBox;
		_leftWeaponAmmoMagazine = viewModelHUDWeapons.TextLeftWeaponAmmoMagazineNumber;
		_leftWeaponAmmoReserve = viewModelHUDWeapons.TextLeftWeaponAmmoReserveNumber;
		_leftWeaponAmmoBox = viewModelHUDWeapons.LeftWeaponAmmoBox;

		_rightWeaponAmmoMagazineText = _rightWeaponAmmoMagazine.GetNodeOrNull<Label>();
		_rightWeaponAmmoReserveText = _rightWeaponAmmoReserve.GetNodeOrNull<Label>();
		_leftWeaponAmmoMagazineText = _leftWeaponAmmoMagazine.GetNodeOrNull<Label>();
		_leftWeaponAmmoReserveText = _leftWeaponAmmoReserve.GetNodeOrNull<Label>();

		_HUDcrosshairs = viewModelHUDWeapons.HUDcrosshiars;

		_crosshairRevolver = viewModelHUDWeapons.CrosshairRevolver;

		_crosshairAutoPistol = viewModelHUDWeapons.CrosshairAutoPistol;
		_listCrosshairPartsAutoPistol = viewModelHUDWeapons.ListCrosshairPartsAutoPistol;

		_crosshairShotgun = viewModelHUDWeapons.CrosshairShotgun;
		_listCrosshairPartsShotgun = viewModelHUDWeapons.ListCrosshairPartsShotgun;

		_crosshairTranquilizer = viewModelHUDWeapons.CrosshairTranquilizer;

		_crosshairCrossbow = viewModelHUDWeapons.CrosshairCrossbow;
		_listCrosshairTypesCrossbow = viewModelHUDWeapons.ListCrosshairTypesCrossbow;

		_menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDWeaponDisplay;

		_playerBehaviour.OnPlayerArmed += ShowCanvasHUDammo;
		_playerBehaviour.OnPlayerDisarmed += HideCanvasHUDammo;

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasHUDammo;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDammo;

		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;
	
		_playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		_playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		_playerWeaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;

		_interactionController.OnPickUpThrowable += (InteractionObjectsPickableTypes pickableType) => { HideRightWeaponAmmo(); };
		_interactionController.OnGetRidOfThrowable += () => 
		{
			if (_playerWeaponController.RightHandWeapon != null && _playerWeaponController.RightHandWeaponComponent is WeaponRangedAbstract)
			{
				ShowRightWeaponAmmo();
			}
		};

		_playerWeaponController.OnShowWeapon += ShowWeaponCrosshair;
		_playerWeaponController.OnHideWeapon += HideWeaponCrosshair;

		HideRightWeaponAmmo();
		HideLeftWeaponAmmo();

		GD.Print("HUDammoController Initialized");
	}

	private void ShowCanvasHUDammo()
	{
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOrEndGameTitlesActive && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded && _playerBehaviour.IsPlayerArmed)
		{
			_canvasHUDammo .Set("visible", true);

			GD.Print("Show canvasAmmo");
		}
	}

	public void HideCanvasHUDammo()
	{
		_canvasHUDammo .Set("visible", false);

		GD.Print("Hide canvasAmmo");
	}

	private void ShowHUDWeaponDisplay()
	{
		_HUDammo .Set("visible", true);
		_HUDcrosshairs .Set("visible", true);
	}

	private void HideHUDWeaponDisplay()
	{
		_HUDammo .Set("visible", false);
		_HUDcrosshairs .Set("visible", false);
	}

	private void UpdateAmmoDisplayForActiveWeapon(WeaponHandType activeHand)
	{
		if (activeHand == WeaponHandType.Left)
		{
			var ranged = _playerWeaponController.LeftHandWeapon?.GetNodeOrNull<WeaponRangedAbstract>();
			if (_playerWeaponController.LeftHandWeapon != null && ranged != null)
			{
				ShowLeftWeaponAmmo();

				_leftWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_leftWeaponAmmoReserveText.text = ammoData.AmmoReserve.ToString();
				}
			}
			else
			{
				HideLeftWeaponAmmo();
			}
		}
		if (activeHand == WeaponHandType.Right)
		{
			var ranged = _playerWeaponController.RightHandWeapon?.GetNodeOrNull<WeaponRangedAbstract>();
			if (_playerWeaponController.RightHandWeapon != null && ranged != null)
			{
				ShowRightWeaponAmmo();

				_rightWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_rightWeaponAmmoReserveText.text = ammoData.AmmoReserve.ToString();
				}
			}
			else
			{
				HideRightWeaponAmmo();
			}
		}

		if(_playerWeaponController.RightHandWeapon == null)
		{
			HideRightWeaponAmmo();
		}
		if (_playerWeaponController.LeftHandWeapon == null)
		{
			HideLeftWeaponAmmo();
		}
	}

	private void UpdateReserveDisplay(AmmoTypes type, int newTotalAmount)
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			var rightRanged = _playerWeaponController.RightHandWeapon.GetNodeOrNull<WeaponRangedAbstract>();
			if (rightRanged != null && rightRanged.PlayerWeaponAmmoType == type)
			{
				_rightWeaponAmmoReserveText.text = newTotalAmount.ToString();
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftRanged = _playerWeaponController.LeftHandWeapon.GetNodeOrNull<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.PlayerWeaponAmmoType == type)
			{
				_leftWeaponAmmoReserveText.text = newTotalAmount.ToString();
			}
		}
	}

	private void UpdateMagazineDisplay(PlayerWeaponNames weaponType, AmmoTypes ammoType, int newAmount)
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			var rightComp = _playerWeaponController.RightHandWeapon.GetNodeOrNull<WeaponAbstract>();
			if (rightComp != null && rightComp.WeaponName == weaponType)
			{
				_rightWeaponAmmoMagazineText.text = newAmount.ToString();
				return;
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftComp = _playerWeaponController.LeftHandWeapon.GetNodeOrNull<WeaponAbstract>();
			if (leftComp != null && leftComp.WeaponName == weaponType)
			{
				_leftWeaponAmmoMagazineText.text = newAmount.ToString();
				return;
			}
		}
	}

	public void ShowRightWeaponAmmo()
	{
		_rightWeaponAmmoBox .Set("visible", true);
	}

	public void HideRightWeaponAmmo()
	{
		_rightWeaponAmmoBox .Set("visible", false);
	}

	public void ShowLeftWeaponAmmo()
	{

		_leftWeaponAmmoBox .Set("visible", true);
	}

	public void HideLeftWeaponAmmo()
	{
		_leftWeaponAmmoBox .Set("visible", false);
	}

	public void ShowWeaponCrosshair(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.Revolver)
		{
			_crosshairRevolver .Set("visible", true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			_crosshairAutoPistol .Set("visible", true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			_crosshairShotgun .Set("visible", true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Tranquilizer)
		{
			_crosshairTranquilizer .Set("visible", true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Crossbow)
		{
			_crosshairCrossbow .Set("visible", true);
		}
	}

	public void HideWeaponCrosshair(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.Revolver)
		{
			_crosshairRevolver .Set("visible", false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			_crosshairAutoPistol .Set("visible", false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			_crosshairShotgun .Set("visible", false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Tranquilizer)
		{
			_crosshairTranquilizer .Set("visible", false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Crossbow)
		{
			_crosshairCrossbow .Set("visible", false);
		}
	}

	public void AnimateWeaponCrosshairOnShoot(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			StartCoroutine(AnimateAutoPistolCrosshair());
		}
		else if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			StartCoroutine(AnimateShotgunCrosshair());
		}
	}

	private IEnumerator AnimateAutoPistolCrosshair()
	{
		var offsets = new Vector2[4]
		{
			new Vector2(25f, -25f),
			new Vector2(-25f, -25f),
			new Vector2(-25f, 25f),
			new Vector2(25f, 25f)
		};

		for (int i = 0; i < _listCrosshairPartsAutoPistol.Length; i++)
		{
			var part = _listCrosshairPartsAutoPistol[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x + offsets[i].x, startPos.y + offsets[i].y, startPos.z);
		}

		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < _listCrosshairPartsAutoPistol.Length; i++)
		{
			var part = _listCrosshairPartsAutoPistol[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x - offsets[i].x, startPos.y - offsets[i].y, startPos.z);
		}
	}

	private IEnumerator AnimateShotgunCrosshair()
	{
		var offsets = new Vector2[4]
		{
			new Vector2(100f, 0f),
			new Vector2(0f, -100f),
			new Vector2(-100f, 0f),
			new Vector2(0f, 100f)
		};

		for (int i = 0; i < _listCrosshairPartsShotgun.Length; i++)
		{
			var part = _listCrosshairPartsShotgun[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x + offsets[i].x, startPos.y + offsets[i].y, startPos.z);
		}

		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < _listCrosshairPartsShotgun.Length; i++)
		{
			var part = _listCrosshairPartsShotgun[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x - offsets[i].x, startPos.y - offsets[i].y, startPos.z);
		}
	}

	public void HandleCrossbowCrosshair(int crossbowCrosshairType)
	{
		if (crossbowCrosshairType == 0)
		{
			_listCrosshairTypesCrossbow[0] .Set("visible", true);
			_listCrosshairTypesCrossbow[1] .Set("visible", false);
			_listCrosshairTypesCrossbow[2] .Set("visible", false);
			_listCrosshairTypesCrossbow[3] .Set("visible", false);
		}
		if (crossbowCrosshairType == 1)
		{
			_listCrosshairTypesCrossbow[0] .Set("visible", false);
			_listCrosshairTypesCrossbow[1] .Set("visible", true);
			_listCrosshairTypesCrossbow[2] .Set("visible", false);
			_listCrosshairTypesCrossbow[3] .Set("visible", false);
		}
		if (crossbowCrosshairType == 2)
		{
			_listCrosshairTypesCrossbow[0] .Set("visible", false);
			_listCrosshairTypesCrossbow[1] .Set("visible", false);
			_listCrosshairTypesCrossbow[2] .Set("visible", true);
			_listCrosshairTypesCrossbow[3] .Set("visible", false);
		}
		if (crossbowCrosshairType == 3)
		{
			_listCrosshairTypesCrossbow[0] .Set("visible", false);
			_listCrosshairTypesCrossbow[1] .Set("visible", false);
			_listCrosshairTypesCrossbow[2] .Set("visible", false);
			_listCrosshairTypesCrossbow[3] .Set("visible", true);
		}
	}

	public Task SaveJsonData(JsonGameData data)
	{
		return Task.CompletedTask;
	}

	public Task LoadJsonData(JsonGameData data)
	{
		if (_playerWeaponController != null && _playerWeaponController.RightHandWeapon != null)
		{
			UpdateAmmoDisplayForActiveWeapon(WeaponHandType.Right);
		}

		if (_playerWeaponController != null && _playerWeaponController.LeftHandWeapon != null)
		{
			UpdateAmmoDisplayForActiveWeapon(WeaponHandType.Left);
		}

		return Task.CompletedTask;
	}
}