using UnityEngine;

public class BaseItem : MonoBehaviour, IItemProperty
{
    [SerializeField] protected ItemProperty itemProperty;
    [SerializeField] protected GameObject InteractTransformItem;

    public string Name => itemProperty.itemName;
    public bool IsInteractable => itemProperty.isInteractable;
    public bool IsDraggable => itemProperty.isDraggable;
    public bool IsPickupable => itemProperty.isPickupable;
    public bool IsFlammable => itemProperty.isFlammable;
    public ItemStrength Strength => itemProperty.strength;



    void Start()
    {
        // Example: Automatically display properties when the game starts
        DisplayProperties();
    }

    // Display item properties
    public virtual void DisplayProperties()
    {
        Debug.Log($"Item: {Name}, Draggable: {IsDraggable}, Strength: {Strength}, Flammable: {IsFlammable}");
    }

    // Additional methods if you want to access these properties manually in the script
    public string GetItemName()
    {
        return Name;
    }

    public bool IsItemDraggable()
    {
        return IsDraggable;
    }

    public ItemStrength GetItemStrength()
    {
        return Strength;
    }

    public bool IsItemFlammable()
    {
        return IsFlammable;
    }

    public void TransformOnInteract()
    {
        Instantiate(InteractTransformItem);
        Destroy( transform.gameObject );   
    }
}