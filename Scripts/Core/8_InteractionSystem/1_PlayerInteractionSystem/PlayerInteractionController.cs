using System;
using Godot;

public partial class PlayerInteractionController : Node
{
	private const float FirstPersonInteractionRange = 2.5f;
	private const float ThirdPersonInteractionRange = 2f;
	private const uint InteractionCollisionMask = uint.MaxValue;

	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private GameScenesManager _gameSceneManager;
	private MenuManager _menuManager;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;
	private Camera3D _camera;
	private Node _canvasHudInteraction;
	private ViewModelHUDInteraction _viewModelHudInteraction;
	private Label _mainInteractionText;
	private Label _failInteractionText;
	private Label _phraseLine;
	private CanvasItem _hudInteraction;
	private CanvasItem _hudPhraseLine;
	private CanvasItem _interactionDot;
	private IInteractable _lookedAtInteractable;
	private IPickable _currentPickable;
	private IThrowable _currentThrowable;
	private Node3D _currentPickableNode;
	private bool _initialized;
	private bool _isInteractionHudVisible = true;
	private bool _isPhraseLineVisible = true;
	private float _interactionRange;
	private string _interactionPressText;
	private string _dropText;
	private string _throwText;
	private PauseSubMenuSettingsSectionGeneralController _pauseSettings;

	public delegate void PickableObjectsPickUpHandler(InteractionObjectsPickableTypes pickableType);
	public delegate void PickableObjectsGetRidOfHandler();
	public delegate void ThrowableObjectThrowHandler(InteractionObjectsPickableTypes throwableType);

	public event PickableObjectsPickUpHandler OnPickUpThrowable;
	public event PickableObjectsPickUpHandler OnPickUpNonThrowable;
	public event PickableObjectsGetRidOfHandler OnGetRidOfNonThrowable;
	public event PickableObjectsGetRidOfHandler OnGetRidOfThrowable;
	public event ThrowableObjectThrowHandler OnThrowTrowable;

	public IThrowable CurrentIThrowable => _currentThrowable;
	public Node3D CurrentPickableObject => _currentPickableNode;
	public bool IsInteractionObjectLookedAt => _lookedAtInteractable != null;

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSettings,
		PlayerBehaviourController playerBehaviour,
		PlayerCameraController playerCameraController,
		PlayerCameraStateMachineController playerCameraStateMachineController,
		Node canvasHudInteraction,
		ViewModelHUDInteraction viewModelHudInteraction)
	{
		_bootstrap = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
		_gameController = gameController ?? throw new ArgumentNullException(nameof(gameController));
		_inputDevice = inputDevice ?? throw new ArgumentNullException(nameof(inputDevice));
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_playerBehaviour = playerBehaviour;
		_pauseSettings = pauseSettings;
		_playerCameraController = playerCameraController;
		_playerCameraStateMachineController = playerCameraStateMachineController;
		_camera = playerCameraController?.GetViewport()?.GetCamera3D();
		_canvasHudInteraction = canvasHudInteraction;
		_viewModelHudInteraction = viewModelHudInteraction;

		_mainInteractionText = viewModelHudInteraction?.TextInteractionMessageMain as Label;
		_failInteractionText = viewModelHudInteraction?.TextInteractionMessageFail as Label;
		_phraseLine = viewModelHudInteraction?.TextPhraseLine as Label;
		_hudInteraction = viewModelHudInteraction?.HUDinteraction as CanvasItem;
		_hudPhraseLine = viewModelHudInteraction?.HUDphraseLine as CanvasItem;
		_interactionDot = viewModelHudInteraction?.DotInteraction as CanvasItem;

		_gameController.OnPlayerEarlyDeath += OnPlayerUnavailable;
		_gameController.OnPlayerRevive += RefreshInteractionRange;
		_menuManager.OnOpenAnyMenu += RefreshInteractionRange;
		_menuManager.OnCloseAnyMenu += RefreshInteractionRange;
		_menuManager.OnOpenCutsceneMenu += RefreshInteractionRange;
		_menuManager.OnCloseCutsceneMenu += RefreshInteractionRange;
		_menuManager.OnOpenInteractionHUD += ShowCanvasHudInteraction;
		_menuManager.OnCloseInteractionHUD += HideCanvasHudInteraction;
		_playerCameraStateMachineController.OnCameraStateChanged += RefreshInteractionRange;
		_playerBehaviour.OnPlayerArmed += HideInteractionDot;
		_playerBehaviour.OnPlayerDisarmed += ShowInteractionDot;
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasHudInteraction;
			_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHudInteraction;
		}
		if (_pauseSettings != null)
		{
			_pauseSettings.OnHUDfull += ShowInteractionHud;
			_pauseSettings.OnHUDdialoguesOnly += HideInteractionHud;
			_pauseSettings.OnHUDdialoguesHide += ShowInteractionHud;
			_pauseSettings.OnHUDturnOff += HideInteractionHud;
			_pauseSettings.OnHUDfull += ShowPhraseLine;
			_pauseSettings.OnHUDdialoguesOnly += ShowPhraseLine;
			_pauseSettings.OnHUDdialoguesHide += HidePhraseLine;
			_pauseSettings.OnHUDturnOff += HidePhraseLine;
		}

		_initialized = true;
		if (_localizationManager != null)
			_localizationManager.OnLanguageChanged += ChangeLanguage;
		RefreshInteractionRange();
		ChangeLanguage(localizationManager);
		HideCanvasHudInteraction();
		GD.Print("PlayerInteractionController initialized.");
	}

	public override void _Process(double delta)
	{
		if (!_initialized || !_bootstrap.IsBootstrapInitialized)
			return;

		UpdateLookTarget();
		if (_currentPickable != null)
		{
			if (_inputDevice.GetKeyInteract() || _gameController.IsPlayerDead)
				DropPickable();
			else if (_currentThrowable != null && _inputDevice.GetKeyRightHandWeaponAttack())
				OnThrowTrowable?.Invoke(_currentPickable.PickableType);
			return;
		}

		if (_lookedAtInteractable == null)
		{
			SetHudText(_mainInteractionText, string.Empty);
			SetHudText(_failInteractionText, string.Empty);
			return;
		}

		SetHudText(_mainInteractionText,
			$"{_lookedAtInteractable.InteractionHintMessageMain}\n{_interactionPressText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}");
		SetHudText(_failInteractionText, _lookedAtInteractable.IsInteractionHintMessageFailActive
			? _lookedAtInteractable.InteractionHintMessageFail
			: string.Empty);

		if (_inputDevice.GetKeyInteract())
		{
			IInteractable target = _lookedAtInteractable;
			target.Interact();
			if (target is IPickable pickable && pickable.IsObjectPickedUp)
				SetCurrentPickable(pickable);
		}
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_interactionPressText = GetLocalized("UI_HUD_Interaction_HintMessage_MainPress", "Interact");
		_dropText = GetLocalized("UI_HUD_Interaction_HintMessage_Action_Drop", "Drop");
		_throwText = GetLocalized("UI_HUD_Interaction_HintMessage_Action_Throw", "Throw");
	}

	public void EarlyThrowThrowable()
	{
		if (_currentThrowable == null)
			return;

		_currentThrowable.ThrowObject();
		_currentPickable = null;
		_currentThrowable = null;
		_currentPickableNode = null;
		RefreshInteractionRange();
	}

	public void LateThrowThrowable() => OnGetRidOfThrowable?.Invoke();

	private void UpdateLookTarget()
	{
		_lookedAtInteractable = null;
		_camera ??= _playerCameraController?.GetViewport()?.GetCamera3D();
		if (!CanInteract() || _camera == null || _interactionRange <= 0f)
			return;

		Vector3 origin = _camera.GlobalPosition;
		Vector3 direction = -_camera.GlobalBasis.Z;
		var query = PhysicsRayQueryParameters3D.Create(origin, origin + direction * _interactionRange, InteractionCollisionMask);
		query.CollideWithAreas = true;
		query.CollideWithBodies = true;
		Godot.Collections.Dictionary hit = _camera.GetWorld3D().DirectSpaceState.IntersectRay(query);
		if (hit.Count == 0 || hit["collider"].AsGodotObject() is not Node hitNode)
			return;

		Node candidate = hitNode;
		while (candidate != null && candidate is not IInteractable)
			candidate = candidate.GetParent();
		_lookedAtInteractable = candidate as IInteractable;
	}

	private bool CanInteract() => _currentPickable == null && !_gameController.IsPlayerDead &&
		!_gameController.IsMainMenuOrEndGameTitlesActive && !_menuManager.IsAnyMenuOpened &&
		!(_menuManager?.IsCutsceneMenuOpened ?? false);

	private void SetCurrentPickable(IPickable pickable)
	{
		_currentPickable = pickable;
		_currentThrowable = pickable as IThrowable;
		_currentPickableNode = pickable as Node3D;
		if (_currentThrowable != null)
		{
			OnPickUpThrowable?.Invoke(pickable.PickableType);
			SetHudText(_mainInteractionText,
				$"{_dropText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}\n{_throwText} {_inputDevice.GetNameOfKey(InputControlsEnum.WeaponAttackRightHand)}");
		}
		else
		{
			OnPickUpNonThrowable?.Invoke(pickable.PickableType);
			SetHudText(_mainInteractionText, $"{_dropText} {_inputDevice.GetNameOfKey(InputControlsEnum.Interact)}");
		}
		RefreshInteractionRange();
	}

	private void DropPickable()
	{
		if (_currentPickable == null)
			return;

		bool wasThrowable = _currentThrowable != null;
		_currentPickable.DropOffObject();
		_currentPickable = null;
		_currentThrowable = null;
		_currentPickableNode = null;
		if (wasThrowable)
			OnGetRidOfThrowable?.Invoke();
		else
			OnGetRidOfNonThrowable?.Invoke();
		RefreshInteractionRange();
	}

	private void RefreshInteractionRange()
	{
		if (_currentPickable != null || _gameController?.IsPlayerDead == true ||
			_gameController?.IsMainMenuOrEndGameTitlesActive == true || _menuManager?.IsAnyMenuOpened == true ||
			_menuManager?.IsCutsceneMenuOpened == true)
		{
			_interactionRange = 0f;
			_lookedAtInteractable = null;
			return;
		}

		_interactionRange = _playerCameraStateMachineController?.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson
			? FirstPersonInteractionRange
			: ThirdPersonInteractionRange + (_playerCameraController?.PlayerCameraDistanceZ ?? 0f);
	}
	private void OnPlayerUnavailable() => DropPickable();
	private string GetLocalized(string key, string fallback) => _localizationManager == null ? fallback : _localizationManager.GetLocalizedString(key);
	private static void SetHudText(Label label, string value) { if (GodotObject.IsInstanceValid(label)) label.Text = value; }
	private static void SetCanvasVisible(CanvasItem item, bool visible) { if (GodotObject.IsInstanceValid(item)) item.Visible = visible; }
	private void ShowCanvasHudInteraction() { SetCanvasVisible(_canvasHudInteraction as CanvasItem, true); SetCanvasVisible(_hudInteraction, _isInteractionHudVisible); SetCanvasVisible(_hudPhraseLine, _isPhraseLineVisible); }
	private void HideCanvasHudInteraction() => SetCanvasVisible(_canvasHudInteraction as CanvasItem, false);
	private void ShowInteractionHud() { _isInteractionHudVisible = true; SetCanvasVisible(_hudInteraction, true); }
	private void HideInteractionHud() { _isInteractionHudVisible = false; SetCanvasVisible(_hudInteraction, false); }
	private void ShowPhraseLine() { _isPhraseLineVisible = true; SetCanvasVisible(_hudPhraseLine, true); }
	private void HidePhraseLine() { _isPhraseLineVisible = false; SetCanvasVisible(_hudPhraseLine, false); }
	private void HideInteractionDot() => SetCanvasVisible(_interactionDot, false);
	private void ShowInteractionDot() => SetCanvasVisible(_interactionDot, true);

	public override void _ExitTree()
	{
		if (!_initialized)
			return;
		_gameController.OnPlayerEarlyDeath -= OnPlayerUnavailable;
		_gameController.OnPlayerRevive -= RefreshInteractionRange;
		_menuManager.OnOpenAnyMenu -= RefreshInteractionRange;
		_menuManager.OnCloseAnyMenu -= RefreshInteractionRange;
		_menuManager.OnOpenCutsceneMenu -= RefreshInteractionRange;
		_menuManager.OnCloseCutsceneMenu -= RefreshInteractionRange;
		_menuManager.OnOpenInteractionHUD -= ShowCanvasHudInteraction;
		_menuManager.OnCloseInteractionHUD -= HideCanvasHudInteraction;
		_playerCameraStateMachineController.OnCameraStateChanged -= RefreshInteractionRange;
		_playerBehaviour.OnPlayerArmed -= HideInteractionDot;
		_playerBehaviour.OnPlayerDisarmed -= ShowInteractionDot;
		if (_localizationManager != null)
			_localizationManager.OnLanguageChanged -= ChangeLanguage;
		if (_pauseSettings != null)
		{
			_pauseSettings.OnHUDfull -= ShowInteractionHud;
			_pauseSettings.OnHUDdialoguesOnly -= HideInteractionHud;
			_pauseSettings.OnHUDdialoguesHide -= ShowInteractionHud;
			_pauseSettings.OnHUDturnOff -= HideInteractionHud;
			_pauseSettings.OnHUDfull -= ShowPhraseLine;
			_pauseSettings.OnHUDdialoguesOnly -= ShowPhraseLine;
			_pauseSettings.OnHUDdialoguesHide -= HidePhraseLine;
			_pauseSettings.OnHUDturnOff -= HidePhraseLine;
		}
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= HideCanvasHudInteraction;
			_gameSceneManager.OnBeginLoadingGameplayScene -= ShowCanvasHudInteraction;
		}
	}
}