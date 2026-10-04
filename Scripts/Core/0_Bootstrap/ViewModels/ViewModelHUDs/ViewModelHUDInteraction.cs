using Godot;
public class ViewModelHUDInteraction
{
	public Node TextInteractionMessageMain;
	public Node TextInteractionMessageFail;
	public Node[] TextsGainedItems;
	public Node[] ImagesGainedItems;
	public Node TextPhraseLine;
	public Node HUDinteraction;
	public Node HUDphraseLine;

	public Node DotInteraction;

	public Node TextStrangleHintNPC;

	public ViewModelHUDInteraction(Bootstrap bootstrap, Node canvas)
	{
		DotInteraction = bootstrap.FindDeepNode(canvas, "DotInteraction");

		TextInteractionMessageMain = bootstrap.FindDeepNode(canvas, "TextMainInteraction");
		TextInteractionMessageFail = bootstrap.FindDeepNode(canvas, "TextFailInteraction");

		TextsGainedItems = new Node[]
		{
			bootstrap.FindDeepNode(canvas,"TextGainedItem1"),
			bootstrap.FindDeepNode(canvas,"TextGainedItem2"),
			bootstrap.FindDeepNode(canvas,"TextGainedItem3")
		};

		ImagesGainedItems = new Node[]
		{
			bootstrap.FindDeepNode(canvas,"ImageGainedItem1"),
			bootstrap.FindDeepNode(canvas,"ImageGainedItem2"),
			bootstrap.FindDeepNode(canvas,"ImageGainedItem3")
		};
		TextPhraseLine = bootstrap.FindDeepNode(canvas, "TextPhrase");

		HUDinteraction= bootstrap.FindDeepNode(canvas, "HUDinteraction");
		HUDphraseLine = bootstrap.FindDeepNode(canvas, "HUDphraseLine");

		TextStrangleHintNPC = bootstrap.FindDeepNode(canvas, "TextChokeNPC");
	}
}
