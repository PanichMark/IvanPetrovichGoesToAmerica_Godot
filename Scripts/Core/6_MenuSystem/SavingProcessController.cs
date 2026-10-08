using Godot;
public partial class SavingProcessController : Node
{
	private JsonSaveLoadController _saveLoadController;
	private Node _canvasSavingProcess;
	private Node _gear;

	public void Initialize(
		JsonSaveLoadController saveLoadController,
		Node canvasSavingProcess,
		ViewModelSavingProcess viewModelSavingProcess)
	{
		_saveLoadController = saveLoadController;
		_canvasSavingProcess = canvasSavingProcess;
		_gear = viewModelSavingProcess.Gear;

		_saveLoadController.OnStartGameDataProcessForUI += ShowCanvasSavingProcess;
		_saveLoadController.OnEndGameDataProcessForUI += HideCanvasSavingProcess;
	}

	public override void _Process(double delta)
	{
		RotateGear(300f, (float)delta);
	}

	private void RotateGear(float speed, float delta)
	{
		if (_gear is Node2D gear2D)
			gear2D.RotationDegrees += speed * delta;
		else if (_gear is Node3D gear3D)
			gear3D.RotationDegrees = new Vector3(gear3D.RotationDegrees.X, gear3D.RotationDegrees.Y, gear3D.RotationDegrees.Z + speed * delta);
	}

	private void ShowCanvasSavingProcess()
	{
		GD.Print("ShowCanvasSavingProcess");
		SetVisible(_canvasSavingProcess, true);
	}

	private void HideCanvasSavingProcess()
	{
		GD.Print("HideCanvasSavingProcess");
		SetVisible(_canvasSavingProcess, false);
	}

	private static void SetVisible(Node node, bool visible)
	{
		if (GodotObject.IsInstanceValid(node) && node is CanvasItem canvasItem)
			canvasItem.Visible = visible;
	}
}