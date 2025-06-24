using UnityEngine;
using UnityEngine.UI;
using SerializedJSONSystem;

public class DesktopManager : InventoryPhysical
{
    [SerializeField] private IconInventorySlotManager[] slots;
    [SerializeField] private int gridWidth = 12;
    [SerializeField] private int gridHeight = 4;
    [SerializeField] GridLayoutGroup gridLayout;

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

        SaveInventoryState(); // Ensure initial state is saved
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

        if (!inventory.GetIcon(droppedSlot.index, out Icon draggedIcon))
        {
            RefreshAllSlots();
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, null, out Vector2 localPoint))
        {
            Vector2 padding = new Vector2(gridLayout.padding.left, gridLayout.padding.top);
            Vector2 cellSize = gridLayout.cellSize;
            Vector2 spacing = gridLayout.spacing;
            
            // Adjust for different start corners
            switch (gridLayout.startCorner)
            {
                case GridLayoutGroup.Corner.UpperLeft:
                    localPoint += rectTransform.rect.size * 0.5f;
                    break;
                case GridLayoutGroup.Corner.UpperRight:
                    localPoint += new Vector2(-rectTransform.rect.size.x * 0.5f, rectTransform.rect.size.y * 0.5f);
                    break;
                case GridLayoutGroup.Corner.LowerLeft:
                    localPoint += new Vector2(rectTransform.rect.size.x * 0.5f, -rectTransform.rect.size.y * 0.5f);
                    break;
                case GridLayoutGroup.Corner.LowerRight:
                    localPoint -= rectTransform.rect.size * 0.5f;
                    break;
            }

            // Adjust for padding
            localPoint -= padding;

            // Calculate grid position based on start axis
            int gridX, gridY;
            if (gridLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
            {
                gridX = Mathf.FloorToInt(localPoint.x / (cellSize.x + spacing.x));
                gridY = Mathf.FloorToInt(localPoint.y / (cellSize.y + spacing.y));

                // Reverse if needed based on start corner
                if (gridLayout.startCorner == GridLayoutGroup.Corner.UpperRight || 
                    gridLayout.startCorner == GridLayoutGroup.Corner.LowerRight)
                {
                    gridX = gridWidth - 1 - gridX;
                }
                if (gridLayout.startCorner == GridLayoutGroup.Corner.LowerLeft || 
                    gridLayout.startCorner == GridLayoutGroup.Corner.LowerRight)
                {
                    gridY = gridHeight - 1 - gridY;
                }
            }
            else // Vertical
            {
                gridX = Mathf.FloorToInt(localPoint.x / (cellSize.x + spacing.x));
                gridY = Mathf.FloorToInt(localPoint.y / (cellSize.y + spacing.y));

                // Swap X and Y for vertical layout (IDE0180)
                (gridY, gridX) = (gridX, gridY);

                // Reverse if needed based on start corner
                if (gridLayout.startCorner == GridLayoutGroup.Corner.UpperRight || 
                    gridLayout.startCorner == GridLayoutGroup.Corner.LowerRight)
                {
                    gridX = gridWidth - 1 - gridX;
                }
                if (gridLayout.startCorner == GridLayoutGroup.Corner.LowerLeft || 
                    gridLayout.startCorner == GridLayoutGroup.Corner.LowerRight)
                {
                    gridY = gridHeight - 1 - gridY;
                }
            }

            // Clamp to grid boundaries
            gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
            gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);
            
            // Calculate final index based on layout
            int targetIndex;
            if (gridLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
            {
                targetIndex = gridY * gridWidth + gridX;
            }
            else
            {
                targetIndex = gridX * gridHeight + gridY;
            }

            if (targetIndex >= 0 && targetIndex < slots.Length && IsSlotEmpty(targetIndex))
            {
                Debug.Log(targetIndex);
                
                inventory.RemoveIcon(droppedSlot.index);
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
}
