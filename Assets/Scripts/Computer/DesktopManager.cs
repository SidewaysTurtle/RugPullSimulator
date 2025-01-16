using UnityEngine;
using System.Collections.Generic;
using SerializedJSONSystem; // Add the correct namespace

public class DesktopManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private IconInventorySlotManager[] slots;

    void Start()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory not assigned to DesktopManager!");
            return;
        }

        // Get all slots if not assigned in inspector
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<IconInventorySlotManager>();
        }

        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        // Clear all slots first
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }

        // Get filled slots from inventory and update UI
        int[] filledSlots = inventory.GetFilledSlots();
        for (int i = 0; i < filledSlots.Length && i < slots.Length; i++)
        {
            Icon icon;
            if (inventory.GetIcon(filledSlots[i], out icon))
            {
                slots[i].SetIcon(icon, filledSlots[i]);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (inventory != null)
        {
            SerializedJSON<Inventory>.SaveScriptableObject(inventory, name);
        }
    }
}
