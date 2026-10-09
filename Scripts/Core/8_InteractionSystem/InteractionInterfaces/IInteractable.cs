public interface IInteractable
{
	public delegate void InteractableObjectHandler();
	event InteractableObjectHandler OnInteract;

	string InteractionObjectNameSystem { get; }
	string InteractionObjectNameUI { get; }
	string InteractionHintMessageMain { get; }
	string InteractionHintMessageAction { get; }
	string InteractionHintMessageFail { get; }
	bool IsInteractionHintMessageFailActive { get; }

	void Interact();
	void InteractCutscene();
}