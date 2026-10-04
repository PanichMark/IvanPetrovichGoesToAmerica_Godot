using System.Collections.Generic;
using Godot;

public interface IInputDevice
{
	float CameraAxisX();
	float CameraAxisY();
	float CameraScroll();
	bool GetKeyPauseMenu();
	bool GetKeyUp();
	bool GetKeyDown();
	bool GetKeyRight();
	bool GetKeyLeft();
	bool GetKeyChangeCameraView();
	bool GetKeyChangeCameraShoulder();
	bool GetKeyHideWeapons();
	bool GetKeyReload();
	bool GetKeyRun();
	bool GetKeyJump();
	bool GetKeyJumpBeingHeld();
	bool GetKeyCrouch();
	bool GetKeyLegKick();
	bool GetKeyInteract();
	bool GetKeySkipCutscene();
	bool GetKeyRightHandWeaponWheel();
	bool GetKeyLeftHandWeaponWheel();
	bool GetKeyRightHandWeaponAttack();
	bool GetKeyLeftHandWeaponAttack();
	bool GetKeyRightHandWeaponAttackReleased();
	bool GetKeyLeftHandWeaponAttackReleased();
	string GetNameOfKey(InputControlsEnum actionName);
	IEnumerable<(InputControlsEnum action, Key key)> GetCurrentKeyBindings();
	IReadOnlyDictionary<InputControlsEnum, Key> CurrentKeyboardKeyBindings { get; }
	IReadOnlyDictionary<InputControlsEnum, Key> GetDefaultKeyBindings();
	void RebindKey(InputControlsEnum actionName, Key newKey);
}