using Godot;
using System.Collections;
using System.Collections.Generic;
public partial class WeaponWheelMenuController3D : Node, IWeaponWheelMenuController
{
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private Node _weaponWheelMenuCanvas;
	private Bootstrap _bootstrap;
	private Node _playerCamera;
	private int _CurrentShowWeaponIndex;
	private Node _weaponToSelect;
	private WeaponAbstract _weaponToSelectComponent;
	private Node _textWeaponAmmoMagazineNumber;
	private Label _textComponentWeaponAmmoMagazineNumber;
	private Node _textWeaponAmmoReserveNumber;
	private Label _textComponentWeaponAmmoReserveNumber;
	private bool _isRotating = false;
	private Node _textWeaponAmmoSeparator;
	private Quaternion _targetRotation; 
	private float _rotationSpeed = 40f;  
	private Node _weaponModelsContainer;
	private List<Node> _weaponModels3D = new List<Node>();
	private bool _isWeaponWheelRestricted;
	public Label WeaponText { get; private set; }
	public Label WeaponWheelName { get; private set; }
	private Node _weaponWheelRadius;

	private List<Node> _wheelSegments = new List<Node>();
	private bool _isWeaponLeftHand = false;
	private WeaponRangedAbstract _weaponRangedAbstractRight;
	private WeaponRangedAbstract _weaponRangedAbstractLeft;
	public delegate void WeaponWheelMenuHandler(WeaponHandType activeHand);
	public event WeaponWheelMenuHandler OnOpenWeaponWheelMenu;
	private Node _weaponIconImage;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private PlayerWeaponController _weaponController;
	private PlayerBehaviourController _playerBehaviour;
	private MenuManager _menuManager;

	private Node _textWeaponWheelUnavailable;
	private Label _textComponentWeaponWheelUnavailable;

	private Node _weaponWheelData;

	private string _weaponWheelHandRight;
	private string _weaponWheelHandLeft;
	private PlayerInteractionController _playerInteractionController;
	private bool _previousRightHandPressed = false;
	private bool _previousLeftHandPressed = false;
	private float _radius = 1f;

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
		_playerCamera = PlayerCamera;
		_weaponWheelMenuCanvas = weaponWheelMenuCanvas;
		WeaponText = viewModelMenuWeaponWheel.TextWeaponWheelWeaponName.GetNodeOrNull<Label>();
		_weaponWheelRadius = viewModelMenuWeaponWheel.WeaponWheelRadius;
		WeaponWheelName = viewModelMenuWeaponWheel.TextWeaponWheelHandType.GetNodeOrNull<Label>();
		_weaponIconImage = viewModelMenuWeaponWheel.ImageWeaponWheelWeaponIcon;

		_textWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber;
		_textComponentWeaponAmmoMagazineNumber = viewModelMenuWeaponWheel.TextWeaponAmmoMagazineNumber.GetNodeOrNull<Label>();
		_textWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber;
		_textComponentWeaponAmmoReserveNumber = viewModelMenuWeaponWheel.TextWeaponAmmoReserveNumber.GetNodeOrNull<Label>();
		_textWeaponAmmoSeparator = viewModelMenuWeaponWheel.TextWeaponAmmoSeparator;

		_textWeaponWheelUnavailable = viewModelMenuWeaponWheel.TextWeaponWheelUnavailable;
		_textComponentWeaponWheelUnavailable = viewModelMenuWeaponWheel.TextWeaponWheelUnavailable.GetNodeOrNull<Label>();

		_weaponWheelData = viewModelMenuWeaponWheel.WeaponWheelData;

		_weaponWheelRadius .Set("visible", false);
		_weaponIconImage .Set("visible", false);
		RecreateWheel();
		_weaponWheelMenuCanvas.Node .Set("visible", false);
		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";
		_textComponentWeaponWheelUnavailable.text = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_CannotChangeWeapon")}";
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_weaponController.OnAnyWeaponUnlocked += OnWeaponUnlocked;

		GD.Print("WeaponWheelMenuController3D Initialized");
	}

	public void RestrictWeaponWheelWhilePickable()
	{
		_isWeaponWheelRestricted = true;

		_weaponWheelData .Set("visible", false);
		_textWeaponWheelUnavailable .Set("visible", true);
	}

	public void UnrestrictWeaponWheelWhilePickable()
	{
		_isWeaponWheelRestricted = false;

		_weaponWheelData .Set("visible", true);
		_textWeaponWheelUnavailable .Set("visible", false);
	}

	public void HideWeaponAmmo()
	{
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

		if (_weaponModelsContainer != null && _weaponModelsContainer.activeSelf && !_isRotating)
		{
			float scrollInput = Input.GetAxis("Mouse ScrollWheel");

			if (scrollInput != 0 && _weaponModels3D.Count > 0)
			{
				_isRotating = true;

				Quaternion[] worldRotations = new Quaternion[_weaponModelsContainer.transform.childCount];
				int i = 0;
				foreach (Transform weaponModel in _weaponModelsContainer.transform)
				{
					worldRotations[i] = weaponModel.rotation;
					i++;
				}

				float angleForOneStep = 360f / _weaponModels3D.Count;
				float direction = Mathf.Sign(scrollInput);

				Quaternion startRotation = _weaponModelsContainer.transform.rotation;
				_targetRotation = startRotation * Quaternion.Euler(0, angleForOneStep * direction, 0);

				if (!_isWeaponWheelRestricted)
				{
					StartCoroutine(RotateWeaponModels(worldRotations));
				}
			}
		}
	}

	private IEnumerator RotateWeaponModels(Quaternion[] savedWorldRotations)
	{
		Quaternion startRotation = _weaponModelsContainer.transform.rotation;
		float elapsedTime = 0f;

		while (elapsedTime < 1f)
		{
			elapsedTime += GetProcessDeltaTime() * _rotationSpeed;
			_weaponModelsContainer.transform.rotation = Quaternion.Slerp(startRotation, _targetRotation, elapsedTime);

			int index = 0;
			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = savedWorldRotations[index];
				index++;
			}
			yield return null; 
		}

		_weaponModelsContainer.transform.rotation = _targetRotation;

		List<Node> weaponsList = _weaponController.CollectActiveWeapons();

		if (weaponsList.Count > 0)
		{
			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			float anglePerSegment = 360f / weaponsList.Count;
			float totalRotatedAngleY = -_weaponModelsContainer.transform.localRotation.eulerAngles.y + 180;
			if (totalRotatedAngleY < 0) totalRotatedAngleY += 360;

			_CurrentShowWeaponIndex = Mathf.RoundToInt(totalRotatedAngleY / anglePerSegment);
			_CurrentShowWeaponIndex %= weaponsList.Count;

			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();
			if (_weaponToSelectComponent is WeaponRangedAbstract)
			{
				ShowWeaponAmmo(_weaponToSelectComponent as WeaponRangedAbstract);
			}
			else
			{
				HideWeaponAmmo();
			}
		}

		ShowWeaponName();

		_isRotating = false;
	}

	public void HandleWeaponWheel(bool rightHandPressed, bool leftHandPressed)
	{
		if (rightHandPressed)
		{
			_isWeaponLeftHand = false;
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandType.Right);
			ShowWeaponWheelMenuCanvas();
		
			ShowWeaponName();
			ShowWeaponPrefabs();
			ShowWeaponAmmo();
			SetEachWeaponTransformRotation();
			WeaponWheelName.text = _weaponWheelHandRight;

			if (_playerInteractionController.CurrentPickableObject != null)
			{
				RestrictWeaponWheelWhilePickable();
			}
		}
		else if (leftHandPressed)
		{
			_isWeaponLeftHand = true;
			OnOpenWeaponWheelMenu?.Invoke(WeaponHandType.Left);
			ShowWeaponWheelMenuCanvas();
		
			ShowWeaponName();
			ShowWeaponPrefabs();
			ShowWeaponAmmo();
			SetEachWeaponTransformRotation();
			WeaponWheelName.text = _weaponWheelHandLeft;

			if (_playerInteractionController.CurrentPickableObject != null && _playerInteractionController.CurrentIThrowable == null)
			{
				RestrictWeaponWheelWhilePickable();
			}
		}
		else
		{
			HideWeaponWheelMenuCanvas();
			HideWeaponPrefabs();

			UnrestrictWeaponWheelWhilePickable();
		}
	}

	private void SetEachWeaponTransformRotation()
	{
		List<Node> activeWeapons = _weaponController.CollectActiveWeapons();

		foreach (Transform weaponModel in _weaponModelsContainer.transform)
		{
			weaponModel.localRotation = Quaternion.Euler(-60, -60, 0);

			Vector3 currentAngles = weaponModel.localEulerAngles;

			if (_CurrentShowWeaponIndex != 0)
			{
				weaponModel.localEulerAngles = new Vector3(currentAngles.x, currentAngles.x + ((360 / activeWeapons.Count) * _CurrentShowWeaponIndex), currentAngles.z);
			}
		}
	}


	private void SetWheelLayerToIgnorePostProcessing()
	{
		if (_weaponModelsContainer == null) return; 

		int ignorePostLayer = LayerMask.NameToLayer("IgnorePostProcessing");
		_weaponModelsContainer.layer = ignorePostLayer;

		foreach (Transform child in _weaponModelsContainer.GetComponentsInChildren<Transform>(true))
		{
			child.Node.layer = ignorePostLayer;
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

		float containerSpawnDistance = 2.0f;

		if (_weaponModelsContainer != null)
		{
			foreach (Transform child in _weaponModelsContainer.transform)
			{
				Destroy(child.Node);
			}
		}
		else
		{
			_weaponModelsContainer = new Node("WeaponModels_Container");
			_weaponModelsContainer.transform.SetParent(_playerCamera.transform, false);
			_weaponModelsContainer.transform.position = _playerCamera.transform.TransformPoint(new Vector3(0, 0.25f, containerSpawnDistance));
		}

		_weaponModels3D.Clear();

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			Node modelInstance = Instantiate(activeWeapons[i], _weaponModelsContainer.transform);

			var eugenicComponent = modelInstance.GetNodeOrNull<WeaponEugenicAbstract>();
			if (eugenicComponent != null)
			{
				ShowOnlyEugenicWeaponBottle(modelInstance);
			}

			SetWheelLayerToIgnorePostProcessing();

			float angleStep = 360f / activeWeapons.Count;
			float angle = i * angleStep;

			Vector3 localPosition = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * angle) * _radius,
				0,
				Mathf.Cos(Mathf.Deg2Rad * angle) * _radius
			);

			modelInstance.transform.SetParent(_weaponModelsContainer.transform, true);
			modelInstance.transform.localPosition = localPosition;
			modelInstance.transform.localRotation = Quaternion.identity;

			_weaponModels3D.Add(modelInstance);

			WeaponAbstract component = modelInstance.GetNodeOrNull<WeaponAbstract>();
			Destroy(component);
		}

		_weaponModelsContainer.transform.localRotation = _playerCamera.transform.localRotation * Quaternion.Euler(0, 180, 0);

		HideWeaponPrefabs();
	}

	private void HideWeaponPrefabs()
	{
		foreach (Node weaponModel in _weaponModels3D)
		{
			if (weaponModel != null)
			{
				weaponModel .Set("visible", false);
			}
		}
	}

	private void ShowWeaponPrefabs()
	{
		foreach (Node weaponModel in _weaponModels3D)
		{
			if (weaponModel != null)
			{
				weaponModel .Set("visible", true);
			}
		}
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
		List<Node> weaponsList = _weaponController.CollectActiveWeapons();

		if (weaponsList.Count > 0)
		{
			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			float anglePerSegment = 360f / weaponsList.Count;
			float totalRotatedAngleY = -_weaponModelsContainer.transform.localRotation.eulerAngles.y + 180;
			if (totalRotatedAngleY < 0) totalRotatedAngleY += 360;

			_CurrentShowWeaponIndex = Mathf.RoundToInt(totalRotatedAngleY / anglePerSegment);
			_CurrentShowWeaponIndex %= weaponsList.Count;
			RotateToEquippedWeapon();

			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();
			if (_weaponToSelectComponent is WeaponRangedAbstract)
			{
				ShowWeaponAmmo(_weaponToSelectComponent as WeaponRangedAbstract);
			}
			else
			{
				HideWeaponAmmo();
			}
		}
	}

	private void RotateToEquippedWeapon()
	{

		if (_isWeaponLeftHand && _weaponController.LeftHandWeapon != null)
		{
			List<Node> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.LeftHandWeaponComponent;

			int targetIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++)
			{
				if (weaponsList[i].GetNodeOrNull<WeaponAbstract>().WeaponNameSystem == activeWeapon.WeaponNameSystem)
				{
					targetIndex = i;
					break;
				}
			}

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo(); 
			}

		}

		if (_isWeaponLeftHand && _weaponController.LeftHandWeapon == null)
		{
			List<Node> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});
			WeaponAbstract activeWeapon = _weaponController.LeftHandWeaponComponent;

			int targetIndex = 0;

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo();
			}
		}
		if (!_isWeaponLeftHand && _weaponController.RightHandWeapon != null)
		{
			List<Node> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.RightHandWeaponComponent;

			int targetIndex = 0;
			for (int i = 0; i < weaponsList.Count; i++)
			{
				if (weaponsList[i].GetNodeOrNull<WeaponAbstract>().WeaponNameSystem == activeWeapon.WeaponNameSystem)
				{
					targetIndex = i;
					break;
				}
			}

			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon);
			}
			else
			{
				HideWeaponAmmo(); 
			}
		}

		if (!_isWeaponLeftHand && _weaponController.RightHandWeapon == null)
		{
			List<Node> weaponsList = _weaponController.CollectActiveWeapons();

			weaponsList.Sort((a, b) =>
			{
				int indexA = _weaponController.ExtractWeaponIndex(a.name);
				int indexB = _weaponController.ExtractWeaponIndex(b.name);
				return indexA.CompareTo(indexB);
			});

			WeaponAbstract activeWeapon = _weaponController.RightHandWeaponComponent;

			int targetIndex = 0;
	
			Quaternion baseRotation = Quaternion.Euler(-30f, 0f, 0f);

			float totalSegments = weaponsList.Count;
			if (totalSegments == 0) return; 

			float anglePerSegment = 360f / totalSegments;

			float wheelYRotation = -(targetIndex * anglePerSegment) + 180;

			Quaternion wheelRotation = Quaternion.Euler(0f, wheelYRotation, 0f);

			Quaternion finalRotation = baseRotation * wheelRotation;

			_weaponModelsContainer.transform.localRotation = finalRotation;

			foreach (Transform weaponModel in _weaponModelsContainer.transform)
			{
				weaponModel.rotation = Quaternion.Euler(-60, -60, 0);
			}

			_CurrentShowWeaponIndex = targetIndex;
			_weaponToSelect = weaponsList[_CurrentShowWeaponIndex];
			_weaponToSelectComponent = _weaponToSelect.GetNodeOrNull<WeaponAbstract>();

			ShowWeaponName(); 
			if (_weaponToSelectComponent is WeaponRangedAbstract rangedWeapon)
			{
				ShowWeaponAmmo(rangedWeapon); 
			}
			else
			{
				HideWeaponAmmo(); 
			}
		}
	}

	private void HideWeaponWheelMenuCanvas()
	{
		if (_weaponController.IsAbleToUseRightWeapon || (_weaponController.IsLeftHand && _weaponController.IsAbleToUseLeftWeapon))
		{
			_weaponController.SelectWeapon(_weaponToSelect);
		}

		_weaponWheelMenuCanvas.Node .Set("visible", false);

		if (!_menuManager.IsPauseMenuOpened)
		{
			_menuManager.CloseWeaponWheelMenu();
		}
	}


	public void ShowWeaponName()
	{
		if (_isWeaponLeftHand)
		{
			if (_weaponController.LeftHandWeapon != null && _weaponController.LeftHandWeapon == _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.LeftHandWeaponComponent.WeaponNameSystem);
			}
			else if (_weaponController.LeftHandWeapon != _weaponToSelect) 
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponToSelectComponent.WeaponNameSystem);
			}
		}
		else if (_isWeaponLeftHand == false)
		{
			if (_weaponController.RightHandWeapon != null && _weaponController.RightHandWeapon == _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponController.RightHandWeaponComponent.WeaponNameSystem);
			}
			else if(_weaponController.RightHandWeapon != _weaponToSelect)
			{
				WeaponText.text = _localizationManager.GetLocalizedString(_weaponToSelectComponent.WeaponNameSystem);
			}
		}
	}

	private void ShowOnlyEugenicWeaponBottle(Node target)
	{
		Transform rightChild = target.transform.Find("Eugenic.R");
		Transform leftChild = target.transform.Find("Eugenic.L");

		Transform eugenicBottle = target.transform.Find("Armature.R/Root/Spine/Arm.R/Forearm.R/Palm.R/WeaponSlot_Hand.R/EugenicBottle");

		rightChild.Node .Set("visible", false);
		leftChild.Node .Set("visible", false);

		eugenicBottle.Node .Set("visible", true);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_weaponWheelHandRight = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandRight")}";
		_weaponWheelHandLeft = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_HandLeft")}";

		_textComponentWeaponWheelUnavailable.text = $"{_localizationManager.GetLocalizedString("UI_Menu_WeaponWheelMenu_CannotChangeWeapon")}";

		ShowWeaponName();
	}

	private void OnDestroy()
	{
		Destroy(_weaponModelsContainer);
		_localizationManager.OnLanguageChanged -= ChangeLanguage;
		_weaponController.OnAnyWeaponUnlocked -= OnWeaponUnlocked;
	}
}