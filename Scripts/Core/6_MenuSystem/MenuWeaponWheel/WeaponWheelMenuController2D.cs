using Godot;
using System.Collections.Generic;
public partial class WeaponWheelMenuController2D : Node, IWeaponWheelMenuController
{
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private Node _weaponWheelSegment;                    
	private Node _weaponWheelMenuCanvas;
	private Bootstrap _bootstrap;
	private Node _textWeaponAmmoMagazineNumber;
	private Label _textComponentWeaponAmmoMagazineNumber;

	private Node _textWeaponAmmoReserveNumber;
	private Label _textComponentWeaponAmmoReserveNumber;
	private Node _textWeaponAmmoSeparator;
	private Label _textComponentWeaponAmmoSeparator;
	private Node _weaponWheelRadius;
	private Node _weaponIconImage;
	public Label WeaponText { get; private set; }            
	public Label WeaponWheelName { get; private set; }
	private PlayerInteractionController _playerInteractionController;

	private Node _textWeaponWheelUnavailable;
	private Label _textComponentWeaponWheelUnavailable;

	private Node _weaponWheelData;


	private List<Node> _wheelSegments = new List<Node>();
	private bool _isWeaponLeftHand = false;
	private WeaponRangedAbstract _weaponRangedAbstractRight;
	private WeaponRangedAbstract _weaponRangedAbstractLeft;
	public delegate void WeaponWheelMenuHandler(WeaponHandType activeHand);
	public event WeaponWheelMenuHandler OnOpenWeaponWheelMenu;

	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private PlayerWeaponController _weaponController;
	private PlayerBehaviourController _playerBehaviour;
	private MenuManager _menuManager;

	private string _weaponWheelHandRight;
	private string _weaponWheelHandLeft;

	private bool _isHoveringOverButton;
	private bool _previousRightHandPressed = false;
	private bool _previousLeftHandPressed = false;
	private float _radius = 130;

	public event System.Action<int> OnSegmentSelected;

	public TextureRect WeaponIcon {  get; private set; }

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerInteractionController playerInteractionController,
		PlayerWeaponAmmoController playerResourcesAmmoManager,
		PlayerWeaponController weaponController,
		Node weaponWheelMenuCanvas,
		ViewModelMenuWeaponWheel viewModelMenuWeaponWheel,
		Node PlayerCamera)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_playerBehaviour = playerBehaviour;
		_playerInteractionController = playerInteractionController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_weaponController = weaponController;
		_menuManager = menuManager;
		_weaponWheelSegment = viewModelMenuWeaponWheel.GameObjectWeaponWheelSegment;
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = viewModelMenuWeaponWheel.TextWeaponWheelWeaponName.GetNodeOrNull<Label>();
		WeaponWheelName = viewModelMenuWeaponWheel.TextWeaponWheelHandType.GetNodeOrNull<Label>();
		WeaponIcon = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon.GetNodeOrNull<TextureRect>();
		_weaponIconImage = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon;
		_weaponWheelRadius = viewModelMenuWeaponWheel.WeaponWheelRadius;
		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetNodeOrNull<Label>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetNodeOrNull<Label>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;
		_textComponentWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator.GetNodeOrNull<Label>();

		_textWeaponWheelUnavailable = viewModelMenuWeaponWheel.TextWeaponWheelUnavailable;
		_textComponentWeaponWheelUnavailable = viewModelMenuWeaponWheel.TextWeaponWheelUnavailable.GetNodeOrNull<Label>();

		_weaponWheelData = viewModelMenuWeaponWheel.WeaponWheelData;

		_weaponWheelRadius .Set("visible", true);
		_weaponIconImage .Set("visible", true);
		_weaponWheelMenuCanvas.Node .Set("visible", false);
		RecreateWheel();
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";
		_textComponentWeaponWheelUnavailable.text = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_CannotChangeWeapon")}";
		_weaponController.OnAnyWeaponUnlocked += OnWeaponUnlocked;

		GD.Print("WeaponWheelMenuController2D Initialized");
	}

	public void RestrictWeaponWheelWhilePickable()
	{
		foreach (Node segment in _wheelSegments)
		{
			segment.GetNodeOrNull<Button>().interactable = false;
		}

		_weaponWheelData .Set("visible", false);
		_textWeaponWheelUnavailable .Set("visible", true);
	}

	public void UnrestrictWeaponWheelWhilePickable()
	{
		foreach (Node segment in _wheelSegments)
		{
			segment.GetNodeOrNull<Button>().interactable = true;
		}

		_weaponWheelData .Set("visible", true);
		_textWeaponWheelUnavailable .Set("visible", false);
	}

	public void HideWeaponAmmo()
	{
		//GD.Print("SHOW WEAPON AMMO");


		_textWeaponAmmoMagazineNumber .Set("visible", false);
		_textWeaponAmmoReserveNumber .Set("visible", false);
		_textWeaponAmmoSeparator .Set("visible", false);
	}

	public void ShowWeaponAmmo(WeaponRangedAbstract weaponComponent)
	{
		_textWeaponAmmoMagazineNumber .Set("visible", true);
		_textWeaponAmmoReserveNumber .Set("visible", true);
		_textWeaponAmmoSeparator .Set("visible", true);

		PlayerWeaponNames newKey = (PlayerWeaponNames)System.Enum.Parse(typeof(PlayerWeaponNames), weaponComponent.WeaponName.ToString());

		if (_playerResourcesAmmoManager.WeaponsRangedDictionary.TryGetValue(newKey, out var newData))
		{
			_textComponentWeaponAmmoMagazineNumber.text = newData.MagazineAmmoCurrent.ToString();
		}

		if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(weaponComponent.PlayerWeaponAmmoType, out var ammoData))
		{
			_textComponentWeaponAmmoReserveNumber.text = ammoData.AmmoReserve.ToString();
		}
	}

	public void ShowWeaponAmmo()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				if (_weaponController.LeftHandWeaponComponent is WeaponRangedAbstract)
				{
					_textWeaponAmmoMagazineNumber .Set("visible", true);
					_textWeaponAmmoReserveNumber .Set("visible", true);
					_textWeaponAmmoSeparator .Set("visible", true);

					_weaponRangedAbstractLeft = _weaponController.LeftHandWeapon.GetNodeOrNull<WeaponRangedAbstract>();
					_textComponentWeaponAmmoMagazineNumber.text = _weaponRangedAbstractLeft.PlayerMagazineAmmoCurrent.ToString();
					_textComponentWeaponAmmoReserveNumber.text = _weaponRangedAbstractLeft.PlayerAmmoReserve.ToString();
				}
				else
				{
					_textWeaponAmmoMagazineNumber .Set("visible", false);
					_textWeaponAmmoReserveNumber .Set("visible", false);
					_textWeaponAmmoSeparator .Set("visible", false);
				}
			}
			else
			{
				_textWeaponAmmoMagazineNumber .Set("visible", false);
				_textWeaponAmmoReserveNumber .Set("visible", false);
				_textWeaponAmmoSeparator .Set("visible", false);
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				if (_weaponController.RightHandWeaponComponent is WeaponRangedAbstract)
				{
					_textWeaponAmmoMagazineNumber .Set("visible", true);
					_textWeaponAmmoReserveNumber .Set("visible", true);
					_textWeaponAmmoSeparator .Set("visible", true);

					_weaponRangedAbstractRight = _weaponController.RightHandWeapon.GetNodeOrNull<WeaponRangedAbstract>();
					_textComponentWeaponAmmoMagazineNumber.text = _weaponRangedAbstractRight.PlayerMagazineAmmoCurrent.ToString();
					_textComponentWeaponAmmoReserveNumber.text = _weaponRangedAbstractRight.PlayerAmmoReserve.ToString();
				}
				else
				{
					_textWeaponAmmoMagazineNumber .Set("visible", false);
					_textWeaponAmmoReserveNumber .Set("visible", false);
					_textWeaponAmmoSeparator .Set("visible", false);
				}
			}
			else
			{
				_textWeaponAmmoMagazineNumber .Set("visible", false);
				_textWeaponAmmoReserveNumber .Set("visible", false);
				_textWeaponAmmoSeparator .Set("visible", false);
			}
		}
	}

	public void OnWeaponUnlocked(Node weaponPrefab)
	{
		RecreateWheel();
	}
	
	void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;
		bool currentRightHandPressed = _inputDevice.GetKeyRightHandWeaponWheel();
		bool currentLeftHandPressed = _inputDevice.GetKeyLeftHandWeaponWheel();

		if ((currentRightHandPressed != _previousRightHandPressed || currentLeftHandPressed != _previousLeftHandPressed) && _weaponController.HasAnyWeapon)
		{
			HandleWeaponWheel(currentRightHandPressed, currentLeftHandPressed);
		}

		_previousRightHandPressed = currentRightHandPressed;
		_previousLeftHandPressed = currentLeftHandPressed;
	}

	public void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandType.Right);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = false;
			ShowWeaponName();
			ShowWeaponIcon();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandRight;

			if (_playerInteractionController.CurrentPickableObject != null)
			{
				RestrictWeaponWheelWhilePickable();
			}
		}
		else if (leftHandPressed)
		{
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandType.Left);
			ShowWeaponWheelMenuCanvas();
			_isWeaponLeftHand = true;
			ShowWeaponName();
			ShowWeaponIcon();
			ShowWeaponAmmo();
			WeaponWheelName.text = _weaponWheelHandLeft;

			if (_playerInteractionController.CurrentPickableObject != null && _playerInteractionController.CurrentIThrowable == null)
			{
				RestrictWeaponWheelWhilePickable();
			}
		}
		else
		{
			HideWeaponWheelMenuCanvas();
			UnrestrictWeaponWheelWhilePickable();
		}
	}

	public void CreateWheel()
	{
		List<Node> activeWeapons = _weaponController.CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return;

		activeWeapons.Sort((a, b) =>
		{
			int indexA = _weaponController.ExtractWeaponIndex(a.name);
			int indexB = _weaponController.ExtractWeaponIndex(b.name);
			return indexA.CompareTo(indexB);
		});

		float angleStep = 360f / activeWeapons.Count;

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			Node segmentInstance = Instantiate(_weaponWheelSegment);
			segmentInstance.name = $"Segment {i + 1}";

			segmentInstance.transform.SetParent(_weaponWheelMenuCanvas.transform, false);

			Button button = segmentInstance.GetNodeOrNull<Button>();
			button.onClick.AddListener(() => OnSegmentSelected?.Invoke(i));

			Control buttonRectTransform = button.GetNodeOrNull<Control>();
			buttonRectTransform.sizeDelta = new Vector2(50, 50);

			Node iconObject = new Node("Icon");
			iconObject.transform.SetParent(button.transform, false);
			iconObject.transform.localPosition = Vector3.zero;
			iconObject.transform.localScale = Vector3.one;

			TextureRect iconImage = iconObject.AddComponent<TextureRect>();
			WeaponAbstract weaponComponent = activeWeapons[i].GetNodeOrNull<WeaponAbstract>();
			if (weaponComponent != null)
			{
				iconImage.Texture2D = weaponComponent.WeaponIconSmall;
			}
			else
			{
				GD.PushError($"Отсутствует компонент WeaponClass у объекта {activeWeapons[i]}");
			}

			iconImage.type = TextureRect.Type.Simple;
			iconImage.fillMethod = TextureRect.FillMethod.Horizontal;
			iconImage.fillAmount = 1f;

			Control iconRectTransform = iconObject.GetNodeOrNull<Control>();
			iconRectTransform.sizeDelta = new Vector2(50, 50);
			iconRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			iconRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			iconRectTransform.pivot = new Vector2(0.5f, 0.5f);

			float adjustedAngle = i * angleStep + 90f;
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, _radius);
			segmentInstance.transform.localPosition = positionOnCircle;

			WeaponWheelMenuButton2D buttonScript = segmentInstance.GetNodeOrNull<WeaponWheelMenuButton2D>();
			if (buttonScript != null)
			{
				buttonScript.Initialize(_localizationManager, _weaponController, this, activeWeapons[i], weaponComponent);
			}

			_wheelSegments.Add(segmentInstance);
		}
	}

	private Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}

	public void RecreateWheel()
	{
		foreach (var seg in _wheelSegments)
			Destroy(seg.Node);

		_wheelSegments.Clear();
		CreateWheel();
	}

	private void ShowWeaponWheelMenuCanvas()
	{
		_weaponWheelMenuCanvas.Node .Set("visible", true);
		_menuManager.OpenWeaponWheelMenu();
	}

	private void HideWeaponWheelMenuCanvas()
	{
		_weaponWheelMenuCanvas.Node .Set("visible", false);
		if (!_menuManager.IsPauseMenuOpened)
		{
			_menuManager.CloseWeaponWheelMenu();
		}
	}

	public void ShowWeaponIcon()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				WeaponIcon.Node .Set("visible", true);
				WeaponIcon.Texture2D = _weaponController.LeftHandWeaponComponent.WeaponIconBig;
			}
			else
			{
				if (!_isHoveringOverButton)
				{
					WeaponIcon.Node .Set("visible", false);
				}
				WeaponIcon.Texture2D = null;
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				WeaponIcon.Node .Set("visible", true);
				WeaponIcon.Texture2D = _weaponController.RightHandWeaponComponent.WeaponIconBig;
			}
			else
			{
				if (!_isHoveringOverButton)
				{
					WeaponIcon.Node .Set("visible", false);
				}
				WeaponIcon.Texture2D = null;
			}
		}
	}

	public void ShowWeaponName()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.LeftHandWeaponComponent.WeaponNameSystem);
			}
			else
			{
				WeaponText.text = "";
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.RightHandWeaponComponent.WeaponNameSystem);
			}
			else
			{
				WeaponText.text = "";
			}
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";

		ShowWeaponName();
	}

	private void OnDestroy()
	{
		foreach (Node segment in _wheelSegments)
		{
			Destroy(segment);
		}

		_wheelSegments.Clear();
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_weaponController.OnAnyWeaponUnlocked -= OnWeaponUnlocked;
	}
}