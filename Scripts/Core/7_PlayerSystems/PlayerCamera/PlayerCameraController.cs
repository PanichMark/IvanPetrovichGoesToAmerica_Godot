using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerCameraController : Node, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _input;
	private MenuManager _menuManager;
	private PlayerMovementController _movement;
	private Node3D _player;
	private Node3D _cameraPivot;
	private Camera3D _camera;
	private Vector2 _rotation;
	private bool _firstPerson;
	private bool _cameraPivotIsCamera;
	private bool _shoulderRight = true;
	private float _distanceY = -1.75f;
	private float _distanceZ = 3.25f;
	private float _sensitivityX = 0.1f;
	private float _sensitivityY = 0.1f;
	private PauseSubMenuSettingsSectionGeneralController _generalSettings;
	private PauseSubMenuSettingsSectionControlsController _controlsSettings;

	public bool IsAbleToZoomCameraOut { get; private set; } = true;
	public float PlayerCameraDistanceX { get; private set; } = -0.85f;
	public float PlayerCameraDistanceY { get => _distanceY; private set => _distanceY = value; }
	public float PlayerCameraDistanceZ { get => _distanceZ; private set => _distanceZ = value; }
	public float CameraRotationLimit { get; private set; } = 70f;
	public PlayerCameraStateTypes PreviousPlayerCameraGameplayType { get; private set; } = PlayerCameraStateTypes.ThirdPerson;
	public float TransitionDelay { get; private set; } = 0.5f;

	public void Initialize(Bootstrap bootstrap, GameController gameController, IInputDevice input, MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController generalSettings, PauseSubMenuSettingsSectionControlsController controlsSettings,
		PlayerMovementController movement, PlayerColliderController playerCollider, Node player, Node cameraRoot)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_input = input;
		_menuManager = menuManager;
		_generalSettings = generalSettings;
		_controlsSettings = controlsSettings;
		_movement = movement;
		_player = player as Node3D;
		_cameraPivot = cameraRoot as Node3D;
		_camera = cameraRoot as Camera3D ?? cameraRoot.FindDeepNodeOfType<Camera3D>();
		_cameraPivotIsCamera = _cameraPivot == _camera;
		_movement?.SetCamera(_camera);
		controlsSettings.OnMouseSensitivityXchanged += SetMouseSensitivityX;
		controlsSettings.OnMouseSensitivityYchanged += SetMouseSensitivityY;
		generalSettings.OnCameraFOVchanged += SetCameraFov;
		_gameController.OnDeactivateMainMenuEndGameTitlesActive += SendCameraFOV;
		_gameController.OnActivateMainMenuEndGameTitlesActive += SendCameraFOV;
		_movement.OnSetPlayerCameraRotationY += SetCameraRotationY;
	}

	public override void _Ready()
	{
		if (_camera == null)
		{
			_camera = this as Camera3D ?? FindDeepNodeOfType<Camera3D>();
			_cameraPivot ??= GetParent() as Node3D;
			_cameraPivotIsCamera = _cameraPivot == _camera;
		}
	}

	public void UpdateCamera(float delta, IInputDevice input)
	{
		if (_cameraPivot == null || _player == null || _menuManager?.IsAnyMenuOpened == true)
			return;
		if (input != null)
		{
			_rotation.X = Mathf.PosMod(_rotation.X + input.CameraAxisX() * _sensitivityX, 360f);
			_rotation.Y = Mathf.Clamp(_rotation.Y + input.CameraAxisY() * _sensitivityY, -CameraRotationLimit, CameraRotationLimit);
			float scroll = input.CameraScroll();
			if (!_firstPerson && IsAbleToZoomCameraOut && scroll != 0f)
			{
				_distanceZ = Mathf.Clamp(_distanceZ - scroll * 0.35f, 1.5f, 5f);
				_distanceY = Mathf.Clamp(_distanceY + scroll * 0.05f, -2f, -1.5f);
			}
			if (input.GetKeyChangeCameraShoulder() && !_firstPerson)
				_shoulderRight = !_shoulderRight;
		}
		PlayerCameraDistanceX = Mathf.MoveToward(PlayerCameraDistanceX, _shoulderRight ? -0.85f : 0.85f, delta * 4f);
		_cameraPivot.GlobalRotation = new Vector3(Mathf.DegToRad(-_rotation.Y), Mathf.DegToRad(_rotation.X), 0f);
		_cameraPivot.GlobalPosition = _firstPerson
			? _player.GlobalPosition + _player.GlobalBasis.Y * (_movement?.PlayerCurrentHeight - 0.13f ?? 1.62f) + _player.GlobalBasis.Z * 0.1f
			: _player.GlobalPosition - _cameraPivot.GlobalBasis * new Vector3(PlayerCameraDistanceX, _distanceY, _distanceZ);
		if (_firstPerson)
			_cameraPivot.GlobalPosition = _player.GlobalPosition + Vector3.Up * ((_movement?.PlayerCurrentHeight ?? 1.75f) - 0.13f);
		if (!_cameraPivotIsCamera && _camera != null)
			_camera.Position = new Vector3(0f, 0f, _cameraPivot == null || _firstPerson ? 0f : _distanceZ);
	}

	public void SetCameraToFirstPerson() { _firstPerson = true; PreviousPlayerCameraGameplayType = PlayerCameraStateTypes.FirstPerson; }
	public void SetCameraToThirdPerson() { _firstPerson = false; PreviousPlayerCameraGameplayType = PlayerCameraStateTypes.ThirdPerson; }
	public void FirstPersonCameraTransform() => UpdateCamera(0f, null);
	public void ThirdPersonCameraTransform() => UpdateCamera(0f, null);
	public void CameraStanding() { }
	public void CameraCrouching() { }
	public void SetCameraMainMenuPosition(Vector3 position) { if (_cameraPivot != null) _cameraPivot.GlobalPosition = position; }
	public void SetCameraMainMenuRotation(Quaternion rotation) { if (_cameraPivot != null) _cameraPivot.GlobalBasis = new Basis(rotation); }
	public void SetCameraRotationY(float yaw) => _rotation = new Vector2(yaw, 0f);
	public void ApplyWeaponRecoilSingle(int upForce, float upDuration, float downDuration) => _rotation.Y = Mathf.Clamp(_rotation.Y - upForce, -CameraRotationLimit, CameraRotationLimit);
	public void ApplyWeaponRecoilAuto() => _rotation.Y = Mathf.Clamp(_rotation.Y - 1f, -CameraRotationLimit, CameraRotationLimit);
	public void SetPostDialogueCameraTransform() { }
	public void RotateCameraTowardsNPC(Node3D npc) { if (npc != null && _cameraPivot != null) _cameraPivot.LookAt(npc.GlobalPosition + Vector3.Up * 1.65f, Vector3.Up); }
	private void SetMouseSensitivityX(float value) => _sensitivityX = value;
	private void SetMouseSensitivityY(float value) => _sensitivityY = value;
	private void SetCameraFov(float value, float minimum, float maximum) { if (_camera != null) _camera.Fov = Mathf.Clamp(value, minimum, maximum); }
	private void SendCameraFOV() { }
	public Task SaveJsonData(JsonGameData data)
	{
		data.PlayerCamera.PlayerCameraDistanceY = _distanceY;
		data.PlayerCamera.PlayerCameraDistanceZ = _distanceZ;
		data.PlayerCamera.PlayerCameraRotation = new Quaternion(Vector3.Up, Mathf.DegToRad(_rotation.X)) * new Quaternion(Vector3.Right, Mathf.DegToRad(-_rotation.Y));
		data.PlayerCamera.IsPlayerCameraShoulderRight = _shoulderRight;
		return Task.CompletedTask;
	}
	public Task LoadJsonData(JsonGameData data)
	{
		_distanceY = data.PlayerCamera.PLayerCameraDistanceY;
		_distanceZ = data.PlayerCamera.PlayerCameraDistanceZ;
		_shoulderRight = data.PlayerCamera.IsPlayerCameraShoulderRight;
		if (data.PlayerCamera.PlayerCameraRotation != Quaternion.Identity)
		{
			Vector3 euler = new Basis(data.PlayerCamera.PlayerCameraRotation).GetEuler();
			_rotation = new Vector2(Mathf.RadToDeg(euler.Y), -Mathf.RadToDeg(euler.X));
		}
		return Task.CompletedTask;
	}
	public override void _ExitTree()
	{
		if (_controlsSettings != null)
		{
			_controlsSettings.OnMouseSensitivityXchanged -= SetMouseSensitivityX;
			_controlsSettings.OnMouseSensitivityYchanged -= SetMouseSensitivityY;
		}
		if (_generalSettings != null) _generalSettings.OnCameraFOVchanged -= SetCameraFov;
		if (_gameController != null)
		{
			_gameController.OnDeactivateMainMenuEndGameTitlesActive -= SendCameraFOV;
			_gameController.OnActivateMainMenuEndGameTitlesActive -= SendCameraFOV;
		}
		if (_movement != null) _movement.OnSetPlayerCameraRotationY -= SetCameraRotationY;
	}
}