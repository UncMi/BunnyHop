using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool isHoldingAnItem { get; private set; }
    public bool HasScrewdriver { get; private set; }

    // List to store inventory items
    private List<string> items = new List<string>();

    private void Start()
    {
        HasScrewdriver = false;
        isHoldingAnItem = false;
    }

    // Function to add an item to the inventory
    public void AddToInventory(string itemName)
    {
        if (!string.IsNullOrEmpty(itemName))
        {
            items.Add(itemName);
            isHoldingAnItem = true;

            // Example: if the item is a screwdriver, set HasScrewdriver to true
            if (itemName.ToLower() == "screwdriver")
            {
                HasScrewdriver = true;
            }

            Debug.Log($"Added {itemName} to the inventory.");
        }
    }

    // Optional: Check if the inventory contains a specific item
    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
