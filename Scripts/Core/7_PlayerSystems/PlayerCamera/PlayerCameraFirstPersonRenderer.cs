using Godot;

public partial class PlayerCameraFirstPersonRenderer : Node
{
	private PlayerCameraStateMachineController _cameraStates;
	private Node _head;
	private Node _hat;

	public void Initialize(PlayerCameraStateMachineController cameraStates, Node playerHead, Node playerHatSlot)
	{
		_cameraStates = cameraStates;
		_head = playerHead;
		_hat = playerHatSlot;
		_cameraStates.OnFirstPersonCameraState += HidePlayerHead;
		_cameraStates.OnThirdPersonCameraState += ShowPlayerHead;
		if (_cameraStates.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson) HidePlayerHead();
	}

	private void ShowPlayerHead() { SetShadowsOnly(_head, false); SetShadowsOnly(_hat, false); }
	private void HidePlayerHead() { SetShadowsOnly(_head, true); SetShadowsOnly(_hat, true); }
	public void ShowBodyPart(Node root) => SetShadowsOnly(root, false);
	public void HideBodyPart(Node root) => SetShadowsOnly(root, true);
	private static void SetShadowsOnly(Node root, bool shadowsOnly)
	{
		if (root == null) return;
		if (root is GeometryInstance3D geometry)
			geometry.CastShadow = shadowsOnly ? GeometryInstance3D.ShadowCastingSetting.ShadowsOnly : GeometryInstance3D.ShadowCastingSetting.On;
		foreach (Node child in root.GetChildren()) SetShadowsOnly(child, shadowsOnly);
	}
	public override void _ExitTree()
	{
		if (_cameraStates == null) return;
		_cameraStates.OnFirstPersonCameraState -= HidePlayerHead;
		_cameraStates.OnThirdPersonCameraState -= ShowPlayerHead;
	}
}