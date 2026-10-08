using Godot;
public partial class WeaponWheelMenuButton2D : Node
{
	private PlayerWeaponController _weaponController;
	private WeaponWheelMenuController2D _weaponWheelController;
	private Node _WeaponPrefab;
	private string _WeaponName;
	private Texture2D _WeaponIcon;
	private Button _button;

	private Color _originalNormalColor;
	LocalizationManager _localizationManager;
	private Node _currentWeapon;
	private WeaponAbstract _weaponComponent;
	private WeaponRangedAbstract _weaponRangedAbstract;
	private Node _previousWeapon;
	private Color _highlightedColor;

	public void Initialize(LocalizationManager localizationManager, PlayerWeaponController weaponController, WeaponWheelMenuController2D weaponWheelController, Node weaponPrefab, WeaponAbstract weaponComponent)
	{
		_localizationManager = localizationManager;
		_weaponController = weaponController;
		_weaponWheelController = weaponWheelController;
		_WeaponPrefab = weaponPrefab;
		_weaponComponent = weaponComponent;
		_WeaponName = _localizationManager.GetLocalizedString(_weaponComponent.WeaponNameSystem);
		_WeaponIcon = _weaponComponent.WeaponIconBig;

		var button = GetNodeOrNull<Button>();
		button.onClick.AddListener(() => SelectWeapon());
		button.onClick.AddListener(() => _weaponWheelController.ShowWeaponIcon());

		_button = button; 

		_originalNormalColor = _button.colors.normalColor;
		_highlightedColor = _button.colors.highlightedColor;

		_weaponWheelController.OnOpenWeaponWheelMenu += OnOpenWeaponWheel;
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponController.OnWeaponChanged += OnWeaponChange;

		//GD.Print("NEW BUTTON!!!!!");
	}

	private void OnOpenWeaponWheel(WeaponHandType activeHand)
	{
		if (activeHand == WeaponHandType.Left)
		{
			_previousWeapon = _weaponController.LeftHandWeapon;
		}
		else
		{
			_previousWeapon = _weaponController.RightHandWeapon;
		}
		HandleOnWeaponChanged(activeHand);
	}

	private void OnWeaponChange(WeaponHandType activeHand)
	{
		HandleOnWeaponChanged(activeHand);
		_previousWeapon = _currentWeapon;
	}

	private void HandleOnWeaponChanged(WeaponHandType activeHand)
	{
		if (activeHand == WeaponHandType.Left)
		{
			_currentWeapon = _weaponController.LeftHandWeapon;
		}
		else
		{
			_currentWeapon = _weaponController.RightHandWeapon;
		}
		if (_currentWeapon != _previousWeapon)
		{
			UpdateButtonColor(_currentWeapon);
		}
		else
		{
			UpdateButtonColor(_previousWeapon);
		}
	}

	private void UpdateButtonColor(Node activeWeapon)
	{
		if (activeWeapon == null)
		{
			ChangeButtonColor(_originalNormalColor);
			return;
		}

		WeaponAbstract activeWeaponComponent = activeWeapon.GetNodeOrNull<WeaponAbstract>();

		WeaponAbstract buttonWeaponComponent = _WeaponPrefab.GetNodeOrNull<WeaponAbstract>();

		if (activeWeaponComponent != null && buttonWeaponComponent != null)
		{
			if (activeWeaponComponent.WeaponNameSystem == buttonWeaponComponent.WeaponNameSystem)
			{
				ChangeButtonColor(_highlightedColor);
			}
			else
			{
				ChangeButtonColor(_originalNormalColor);
			}
		}
	}

	public void HoverEnter()
	{
		_weaponWheelController.WeaponIcon.Node .Set("visible", true);
		_weaponWheelController.WeaponText.text = _WeaponName;
		_weaponWheelController.WeaponIcon.Texture2D = _WeaponIcon;

		if (_weaponComponent is WeaponRangedAbstract)
		{
			_weaponRangedAbstract = _WeaponPrefab.GetNodeOrNull<WeaponRangedAbstract>();
			_weaponWheelController.ShowWeaponAmmo(_weaponRangedAbstract);
		}
		else
		{
			_weaponWheelController.HideWeaponAmmo();
		}
	}

	public void HoverExit()
	{
		_weaponWheelController.ShowWeaponName();
		_weaponWheelController.ShowWeaponIcon();

		_weaponWheelController.ShowWeaponAmmo();
	}

	private void SelectWeapon()
	{
		if (_weaponController.IsAbleToUseRightWeapon || (_weaponController.IsLeftHand && _weaponController.IsAbleToUseLeftWeapon))
		{
			_weaponController.SelectWeapon(_WeaponPrefab);
		}
	}

	private void OnDestroy()
	{
		_weaponController.OnWeaponChanged -= OnWeaponChange;
		_weaponWheelController.OnOpenWeaponWheelMenu -= OnOpenWeaponWheel;
		_localizationManager.OnLanguageChanged -= ChangeLanguage;

		//GD.Print("DESTROY BUTTON!!!!!");
	}

	private void ChangeButtonColor(Color newColor)
	{
		ColorBlock colors = _button.colors;
		colors.normalColor = newColor;
		_button.colors = colors;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_WeaponName = _localizationManager.GetLocalizedString(_weaponComponent.WeaponNameSystem);
	}
}