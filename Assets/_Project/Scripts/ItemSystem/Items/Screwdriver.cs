using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screwdriver : BaseItem, IInteractable
{
    public string InteractionPrompt => throw new System.NotImplementedException();

    public bool Interact(Interactor interactor)
    {
        var playerInventory = interactor.GetComponent<Inventory>();
        Debug.Log("Screwdriver- Interacted");
        if (playerInventory == null) return false;

        if (!playerInventory.isHoldingAnItem){
            Debug.Log("Screwdriver - picking up screwdriver");
            return true;
        }

        else
        {
            Debug.Log("Screwdriver - holding another item");
            return false;
        }
    }

    public bool Interact2(Interactor interactor)
    {
        throw new System.NotImplementedException();
    }
}
