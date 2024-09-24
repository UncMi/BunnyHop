using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemStrength
{
    Weak,
    Medium,
    Strong,
    Indestructable
}
public interface IItemProperty 
{

        string Name { get; }
        bool IsInteractable { get; }
        bool IsDraggable { get; }
        bool IsPickupable { get; }
        bool IsFlammable { get; }
        ItemStrength Strength { get; }

}
