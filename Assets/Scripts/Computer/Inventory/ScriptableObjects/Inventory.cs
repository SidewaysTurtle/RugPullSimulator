using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Inventory", fileName = "Inventory.asset")]
[System.Serializable]
public class Inventory : ScriptableObject
{
    [SerializeField] private TextIcon[] inventory;
    // using an indexer, isnt needed in this context
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/indexers/
    // public Icon this[int index]
    // {
    //     get
    //     {
    //         return inventory[index];
    //     }
    //     set
    //     {
    //         inventory[index] = value;
    //     }
    // }

    /// <summary>
    /// Loads the inventory to a json file.
    /// </summary>
    /// <param name="path">The path to the json file.</param>
    
    /* Inventory START */

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
    public bool GetIcon(int index, out TextIcon icon) {
        // inventory[index] doesn't return null, so check icon instead.
        if (SlotEmpty(index)) {
            icon = default(TextIcon);
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

        inventory[index] = default(TextIcon);

        return true;
    }

    /// <summary>
    /// Push an icon, return the index where it was inserted. If the icon already exists, return -1
    /// </summary>
    /// <param name="icon">The icon to insert.</param>
    /// <returns>The index where the icon was inserted. If the icon already exists, return -1.</returns>
    public int PushIcon(TextIcon icon) {
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
    public int InsertIcon(int index, TextIcon icon) {
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
}
