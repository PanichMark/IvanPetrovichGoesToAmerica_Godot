public interface IPickable
{
	bool IsObjectPickedUp { get; }
	InteractionObjectsPickableTypes PickableType { get; }

	void PickUpObject(bool isPickedUpByLoadSafeFile);
	void DropOffObject();
}