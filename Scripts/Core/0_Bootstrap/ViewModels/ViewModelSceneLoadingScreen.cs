using Godot;
public class ViewModelSceneLoadingScreen
{
	public Node ImageScene;

	public Node TextMissionName;
	public Node TextSceneName;
	public Node TextSceneDescription;
	public Node TextLoadingIsReady;

	public Node SliderSceneLoadingStatus;

	public ViewModelSceneLoadingScreen(Bootstrap bootstrap, Node canvas)
	{
		ImageScene = bootstrap.FindDeepNode(canvas, "ImageScene");

		TextMissionName = bootstrap.FindDeepNode(canvas, "TextMissionName");
		TextSceneName = bootstrap.FindDeepNode(canvas, "TextSceneName");
		TextSceneDescription = bootstrap.FindDeepNode(canvas, "TextSceneDescription");
		TextLoadingIsReady = bootstrap.FindDeepNode(canvas, "TextLoadingIsReady");

		SliderSceneLoadingStatus = bootstrap.FindDeepNode(canvas, "SliderSceneLoadingStatus");
	}
}
