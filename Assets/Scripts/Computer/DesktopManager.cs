using UnityEngine;
using SerializedJSONSystem;

public class DesktopManager : InventoryPhysical
{
    [SerializeField] private IconInventorySlotManager[] slots;
    [SerializeField] private int gridWidth = 8;  // Number of slots horizontally
    [SerializeField] private int gridHeight = 6; // Number of slots vertically
    [SerializeField] private float slotSpacing = 100f; // Space between slots

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

        PositionSlotsInGrid(); // Position the slots in grid layout
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
        if (!inventory.GetIcon(droppedSlot.index, out Icon draggedIcon))
        {
            RefreshAllSlots();
            return;
        }

        // Get the mouse position and convert it to local position
        Vector2 mousePos = Input.mousePosition;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, null, out localPoint))
        {
            // Calculate grid position
            int gridX = Mathf.RoundToInt((localPoint.x + (rectTransform.rect.width / 2)) / slotSpacing);
            int gridY = Mathf.RoundToInt((localPoint.y + (rectTransform.rect.height / 2)) / slotSpacing);

            // Clamp to grid boundaries
            gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
            gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);

            // Convert grid position to slot index
            int targetIndex = gridY * gridWidth + gridX;

            // Ensure the target index is valid
            if (targetIndex >= 0 && targetIndex < slots.Length)
            {
                inventory.InsertIcon(targetIndex, draggedIcon);
                SaveInventoryState();
                RefreshAllSlots();
            }
        }
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

    // Helper method to position slots in grid layout
    private void PositionSlotsInGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                if (index < slots.Length)
                {
                    RectTransform slotRect = slots[index].GetComponent<RectTransform>();
                    float posX = (x - (gridWidth - 1) / 2f) * slotSpacing;
                    float posY = (y - (gridHeight - 1) / 2f) * slotSpacing;
                    slotRect.anchoredPosition = new Vector2(posX, posY);
                }
            }
        }
    }
}
