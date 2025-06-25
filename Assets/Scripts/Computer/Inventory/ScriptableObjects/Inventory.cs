using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Collections;
using System;

[CreateAssetMenu(menuName = "Inventory/Inventory", fileName = "Inventory.asset")]
[System.Serializable]
public class Inventory : ScriptableObject
{
    [SerializeReference] private Icon[] inventory;

    /// <summary>
    /// Checks if a slot is empty.
    /// </summary>
    /// <param name="index">The index of the slot to check.</param>
    /// <returns>True if the slot is empty, false if it is not.</returns>
    public bool SlotEmpty(int index) {
        if (inventory[index] == null)
            return true;
        return false;
    }

    /// <summary>
    // Get an icon if it exists.
    /// </summary>
    /// <param name="index">The index of the icon.</param>
    /// <param name="icon">The icon to return.</param>
    /// <returns>True if the icon exists, false if it doesn't.</returns>
    public bool GetIcon(int index, out Icon icon) {
        if (SlotEmpty(index)) {
            icon = default(Icon);
            return false;
        }

        icon = inventory[index];
        return true;
    }

    /// <summary>
    /// Remove an icon at an index if one exists at that index.
    /// </summary>
    /// <param name="index">The index of the icon to remove.</param>
    /// <returns>True if the icon was removed, false if it didn't exist.</returns>
    public bool RemoveIcon(int index) {
        if (SlotEmpty(index)) {
            // Nothing existed at the specified slot.
            return false;
        }

        inventory[index] = default(Icon);

        return true;
    }

    /// <summary>
    /// Push an icon, return the index where it was inserted. If the icon already exists, return -1
    /// </summary>
    /// <param name="icon">The icon to insert.</param>
    /// <returns>The index where the icon was inserted. If the icon already exists, return -1.</returns>
    public int PushIcon(Icon icon) {
        for (int i = 0; i < inventory.Length; i++) {
            if (SlotEmpty(i)) {
                inventory[i] = icon;
                return i;
            }
        }

        // Couldn't find a free slot.
        return -1;
    }

    /// <summary>
    /// Push an icon, return the index where it was inserted. If the icon already exists, return -1
    /// </summary>
    /// <param name="icon">The icon to insert.</param>
    /// <returns>The index where the icon was inserted. If the icon already exists, return -1.</returns>
    public int InsertIcon(int index, Icon icon) {
        if(SlotEmpty(index)){
            inventory[index] = icon;
            return index;
        }
        // Was not a free slot.
        return -1;
    }

    public int GetLength()
    {
        return inventory.Length;
    }

    /// <summary>
    /// Returns all non-empty slots in the inventory
    /// </summary>
    /// <returns>Array of indices that contain icons</returns>
    public int[] GetFilledSlots()
    {
        List<int> filledSlots = new();
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!SlotEmpty(i))
            {
                filledSlots.Add(i);
            }
        }
        return filledSlots.ToArray();
    }
}
