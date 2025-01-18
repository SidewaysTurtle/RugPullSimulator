using UnityEngine;
using SerializedJSONSystem;

public class DesktopManager : InventoryPhysical
{
    [SerializeField] private IconInventorySlotManager[] slots;

    void Start()
    {
        LoadInventoryState();

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
        DragManager.OnDropEvent += HandleDrop;
    }

    void OnDestroy()
    {
        DragManager.OnDropEvent -= HandleDrop;
    }

    public void RefreshAllSlots()
    {
        // Clear all slots first and assign initial indices
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
            slots[i].index = i;  // Assign base index
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

    public bool IsSlotEmpty(int index)
    {
        Icon icon;
        return !inventory.GetIcon(index, out icon);
    }

    public bool IsSlotEmpty(IconInventorySlot slot)
    {
        var slotManager = slot.GetComponent<IconInventorySlotManager>();
        // Check if the slot's content is visible
        return slotManager.IsEmpty();
    }

    private void HandleDrop(IconInventorySlotManager droppedSlot)
    {
        if (droppedSlot == null) return;

        // Get the icon from the source slot
        Icon draggedIcon;
        if (!inventory.GetIcon(droppedSlot.index, out draggedIcon))
        {
            RefreshAllSlots();
            return;
        }

        // If dropping on itself, just refresh
        if (droppedSlot == null)
        {
            RefreshAllSlots();
            return;
        }

        // Handle the swap
        int originalIndex = droppedSlot.index;
        inventory.RemoveIcon(originalIndex);
        inventory.InsertIcon(droppedSlot.index, draggedIcon);
        
        SaveInventoryState();
        RefreshAllSlots();
    }

    private void SaveInventoryState()
    {
        SerializedJSON<Inventory>.SaveScriptableObject(inventory, gameObject.scene.name + "_desktop");
    }

    private void LoadInventoryState()
    {
        SerializedJSON<Inventory>.LoadScriptableObject(gameObject.scene.name + "_desktop", out inventory);
        if (inventory == null)
        {
            // If no saved state exists, create a new inventory
            inventory = ScriptableObject.CreateInstance<Inventory>();
        }
    }

    private IconInventorySlot FindClosestSlot(IconInventorySlot droppedSlot)
    {
        float smallestDistance = float.MaxValue;
        IconInventorySlot closest = null;

        foreach (var slot in slots)
        {
            float distance = Vector3.Distance(
                droppedSlot.transform.position, 
                slot.transform.position
            );
            
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closest = slot;
            }
        }

        return closest;
    }

    void OnApplicationQuit()
    {
        SaveInventoryState();
    }
}
