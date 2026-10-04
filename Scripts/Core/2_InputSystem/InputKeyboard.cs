using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;

public class InputKeyboard : IInputDevice
{
	private readonly Dictionary<InputControlsEnum, Key> _initialKeyboardKeyBindings;
	private GameController _gameController;
	private float _lastTimeSinceKeyHideWeaponWasHeld;
	private float _lastTimeSinceKeySkipCutsceneWasHeld;
	private float _timeToHoldKeyHideWeapon = 0.5f;
	private float _timeToHoldKeySkipCutscene = 1;
	private bool _isKeyInteractBeingHeld;
	private bool _isKeySkipCutsceneBeingHeld;
	private bool _isRightHandWeaponWheelOpened;
	private bool _isLeftHandWeaponWheelOpened;
	private Key _keyPauseMenu;

	private Dictionary<InputControlsEnum, Key> _keyboardKeyBindings = new()
	{
		{ InputControlsEnum.MoveForward, Key.W },
		{ InputControlsEnum.MoveBackward, Key.S },
		{ InputControlsEnum.MoveRight, Key.D },
		{ InputControlsEnum.MoveLeft, Key.A },
		{ InputControlsEnum.Run, Key.Shift },
		{ InputControlsEnum.Jump, Key.Space },
		{ InputControlsEnum.Crouch, Key.Ctrl },
		{ InputControlsEnum.Interact, Key.F },
		{ InputControlsEnum.ChangeCameraView, Key.V },
		{ InputControlsEnum.ChangeCameraShoulder, Key.C },
		{ InputControlsEnum.WeaponWheelRightHand, Key.E },
		{ InputControlsEnum.WeaponWheelLeftHand, Key.Q },
		{ InputControlsEnum.WeaponAttackRightHand, Key.None },
		{ InputControlsEnum.WeaponAttackLeftHand, Key.None },
		{ InputControlsEnum.WeaponReload, Key.R },
		{ InputControlsEnum.LegKick, Key.None }
	};

	public InputKeyboard(GameController gameController, Key keyPauseMenu)
	{
		_gameController = gameController;
		_keyPauseMenu = keyPauseMenu;
		_initialKeyboardKeyBindings = new Dictionary<InputControlsEnum, Key>(_keyboardKeyBindings);

		GD.Print("InputKeyboard Initialized");
	}

	public IReadOnlyDictionary<InputControlsEnum, Key> CurrentKeyboardKeyBindings
	{
		get
		{
			return new ReadOnlyDictionary<InputControlsEnum, Key>(_keyboardKeyBindings);
		}
	}

	public IReadOnlyDictionary<InputControlsEnum, Key> GetDefaultKeyBindings()
	{
		var copyOfInitialBindings = new Dictionary<InputControlsEnum, Key>(_initialKeyboardKeyBindings);
		return new ReadOnlyDictionary<InputControlsEnum, Key>(copyOfInitialBindings);
	}

	public IEnumerable<(InputControlsEnum action, Key key)> GetCurrentKeyBindings()
	{
		return _keyboardKeyBindings.Select(kvp => (kvp.Key, kvp.Value));
	}

	public void RebindKey(InputControlsEnum actionName, Key newKey)
	{
		if (!_keyboardKeyBindings.ContainsKey(actionName))
			GD.PushError($"Нет такого действия '{actionName}'.");
		else
			_keyboardKeyBindings[actionName] = newKey;
	}

	public bool GetKeyPauseMenu()
	{
		if (Input.IsKeyPressed(_keyPauseMenu) && Input.IsKeyJustPressed(_keyPauseMenu) && _gameController.IsPauseMenuAvailable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyUp()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveForward]) &&
			Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveForward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyDown()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveForward]) &&
			Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveBackward]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRight()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveRight]) &&
			Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveRight]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeft()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveRight]) &&
			Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return false;
		}
		else if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.MoveLeft]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraView()
	{
		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.ChangeCameraView]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyChangeCameraShoulder()
	{
		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.ChangeCameraShoulder]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyHideWeapons()
	{
		if (!_isKeyInteractBeingHeld)
		{
			if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable)
			{
				_lastTimeSinceKeyHideWeaponWasHeld = Time.GetTicksMsec() / 1000f;
				_isKeyInteractBeingHeld = true;
			}
		}
		else if (Input.IsKeyJustReleased(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable)
		{
			_isKeyInteractBeingHeld = false;
		}
		else if (_isKeyInteractBeingHeld && Time.GetTicksMsec() / 1000f >= _lastTimeSinceKeyHideWeaponWasHeld + _timeToHoldKeyHideWeapon)
		{
			_isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public bool GetKeyReload()
	{
		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.WeaponReload]) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRun()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.Run]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJump()
	{
		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.Jump]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyJumpBeingHeld()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.Jump]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyCrouch()
	{
		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.Crouch]) && _gameController.IsPlayerControllable && _gameController.IsPlayerAbleToMove)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLegKick()
	{
		if (Input.IsMouseButtonJustPressed(MouseButton.Middle) && _gameController.IsPlayerControllable && !_gameController.IsPlayerMovementRestrictedByCarryingNonThrowable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyInteract()
	{
		if (_isKeyInteractBeingHeld && Time.GetTicksMsec() / 1000f > _lastTimeSinceKeyHideWeaponWasHeld + 0.01f)
		{
			return false;
		}

		if (Input.IsKeyJustPressed(_keyboardKeyBindings[InputControlsEnum.Interact]) && _gameController.IsPlayerControllable)
		{
			_isKeyInteractBeingHeld = false;
			return true;
		}
		return false;
	}

	public bool GetKeyRightHandWeaponWheel()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.WeaponWheelRightHand]) && !_isLeftHandWeaponWheelOpened && _gameController.IsPlayerControllable)
		{
			_isRightHandWeaponWheelOpened = true;
			return true;
		}
		else
		{
			_isRightHandWeaponWheelOpened = false;
			return false;
		}
	}

	public bool GetKeyLeftHandWeaponWheel()
	{
		if (Input.IsKeyPressed(_keyboardKeyBindings[InputControlsEnum.WeaponWheelLeftHand]) && !_isRightHandWeaponWheelOpened && _gameController.IsPlayerControllable)
		{
			_isLeftHandWeaponWheelOpened = true;
			return true;
		}
		else
		{
			_isLeftHandWeaponWheelOpened = false;
			return false;
		}
	}

	public bool GetKeyRightHandWeaponAttack()
	{
		if (Input.IsMouseButtonJustPressed(MouseButton.Right) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttack()
	{
		if (Input.IsMouseButtonJustPressed(MouseButton.Left) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyRightHandWeaponAttackReleased()
	{
		if (Input.IsMouseButtonJustReleased(MouseButton.Right) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeyLeftHandWeaponAttackReleased()
	{
		if (Input.IsMouseButtonJustReleased(MouseButton.Left) && _gameController.IsPlayerControllable)
		{
			return true;
		}
		else return false;
	}

	public bool GetKeySkipCutscene()
	{
		if (!_isKeySkipCutsceneBeingHeld)
		{
			if (Input.IsKeyJustPressed(Key.Space))
			{
				_lastTimeSinceKeySkipCutsceneWasHeld = Time.GetTicksMsec() / 1000f;
				_isKeySkipCutsceneBeingHeld = true;
			}
		}
		else if (Input.IsKeyJustReleased(Key.Space))
		{
			_isKeySkipCutsceneBeingHeld = false;
		}
		else if (_isKeySkipCutsceneBeingHeld && Time.GetTicksMsec() / 1000f >= _lastTimeSinceKeySkipCutsceneWasHeld + _timeToHoldKeySkipCutscene)
		{
			_isKeySkipCutsceneBeingHeld = false;
			return true;
		}
		return false;
	}

	public string GetNameOfKey(InputControlsEnum actionName)
	{
		if (_keyboardKeyBindings.TryGetValue(actionName, out Key key))
		{
			return key.ToString();
		}

		GD.PushWarning($"[InputKeyboard] Не найдено действие '{actionName}' для получения имени клавиши.");
		return "?";
	}

	public float CameraAxisX()
	{
		if (_gameController.IsPlayerControllable)
		{
			return Input.GetLastMouseVelocity().X * (float)Engine.GetProcessDeltaTime();
		}
		else
		{
			return 0;
		}
	}

	public float CameraAxisY()
	{
		if (_gameController.IsPlayerControllable)
		{
			return Input.GetLastMouseVelocity().Y * (float)Engine.GetProcessDeltaTime();
		}
		else
		{
			return 0;
		}
	}

	public float CameraScroll()
	{
		if (_gameController.IsPlayerControllable)
		{
			if (Input.IsMouseButtonJustPressed(MouseButton.WheelUp))
			{
				return 1;
			}
			else if (Input.IsMouseButtonJustPressed(MouseButton.WheelDown))
			{
				return -1;
			}
			return 0;
		}
		else
		{
			return 0;
		}
	}
}