using UnityEngine;

public class Wall_Basic : BaseItem, IInteractable
{
    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if (inventory == null) return false;

        if (inventory.HasScrewdriver)
        {
            Debug.Log("Opening wall");
            TransformOnInteract();
            return true;
        }

        Debug.Log("No Screw!");

        DisplayProperties();
        return false;
    }


    public void DisplayProperties()
    {
        Debug.Log($"Item: {itemProperty.itemName}, Draggable: {itemProperty.isDraggable}, Strength: {itemProperty.strength}, Flammable: {itemProperty.isFlammable}");
    }

    public bool Interact2(Interactor interactor)
    {
        throw new System.NotImplementedException();
    }
}
