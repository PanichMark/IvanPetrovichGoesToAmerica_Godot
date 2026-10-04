using Godot;
public class GameplayCanvases
{
	public Node CanvasLockMechanical { get; private set; }
	public Node CanvasLockElectronic { get; private set; }
	public Node CanvasNote { get; private set; }
	public Node CanvasDialogue { get; private set; }
	public Node CanvasHUDmonocular {  get; private set; }
	public Node CanvasMainMenuChooseMission { get; private set; }
	public Node CanvasMainMenuReadNews { get; private set; }
	public Node CanvasCutscene { get; private set; } // Новое поле

	public GameplayCanvases(Node canvasLockMechanical,
							Node canvasLockElectronic,
							Node canvasNote,
							Node canvasDialogue,
							Node canvasMainMenuChooseMission,
							Node canvasMainMenuReadNews,
							Node canvasCutscene,
							Node canvasHUDmonocular) // Новый аргумент
	{
		CanvasLockMechanical = canvasLockMechanical;
		CanvasLockElectronic = canvasLockElectronic;
		CanvasNote = canvasNote;
		CanvasDialogue = canvasDialogue;

		CanvasMainMenuChooseMission = canvasMainMenuChooseMission;
		CanvasMainMenuReadNews = canvasMainMenuReadNews;
		CanvasCutscene = canvasCutscene;
		CanvasHUDmonocular = canvasHUDmonocular;
	}
}