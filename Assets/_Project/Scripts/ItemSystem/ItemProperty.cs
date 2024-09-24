using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpecification", menuName = "Items/ItemSpecification")]
public class ItemProperty : ScriptableObject
{
    public string itemName;
    public bool isInteractable;
    public bool isDraggable;
    public bool isPickupable;
    public bool isFlammable;

    public ItemStrength strength;


}