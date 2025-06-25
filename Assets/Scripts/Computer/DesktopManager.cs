using UnityEngine;
using UnityEngine.UI;
using SerializedJSONSystem;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DesktopManager : InventoryPhysical, IPointerClickHandler
{
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
            slots = GetComponentsInChildren<DesktopIcon>();
        }

        // So in reality I believe this takes the inventory and makes the physical inventory
        // from that, and to that it reads the json, and then the scriptable object

        RefreshAllSlots();
        IconInputManager.OnDropEvent += HandleDrop;

        SaveInventoryState(); // Ensure initial state is saved
    }

    void OnDestroy()
    {
        IconInputManager.OnDropEvent -= HandleDrop;
    }

    // Handle clicks on empty desktop areas to clear focus
    public void OnPointerClick(PointerEventData eventData)
    {
        // Clear focus when clicking on empty desktop space
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
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
                slots[filledSlots[i]].SetIcon(icon, filledSlots[i]);
            }
        }
    }
    private void HandleDrop(DesktopIcon droppedSlot, Vector2 screenPos)
    {
        if (droppedSlot == null) return;

        if (!(screenPos.x >= 0 && screenPos.x <= Screen.width && screenPos.y >= 0 && screenPos.y <= Screen.height))
        {
            // Dropped outside the grid: revert to original position (no change)
            Debug.Log($"[DesktopManager]: Dropped outside desktop enviroment.");
            RefreshAllSlots();
            return;
        }
        if (!inventory.GetIcon(droppedSlot.index, out Icon draggedIcon))
        {
            Debug.LogError("[DesktopManager]: Dragged icon not found in inventory for slot index: " + droppedSlot.index);
            RefreshAllSlots();
            return;
        }

        // The gridLayout's RectTransform is the container for the grid.
        RectTransform gridRectTransform = gridLayout.GetComponent<RectTransform>();

        // Convert mouse position to local point in the grid container
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRectTransform, Input.mousePosition, null, out Vector2 localPoint))
        {
            // The localPoint is relative to the pivot (usually the center).
            // We need to make it relative to the bottom-left corner of the RectTransform.
            Vector2 positionInRect = localPoint - gridRectTransform.rect.min;

            // Get grid properties
            Vector2 cellSize = gridLayout.cellSize;
            Vector2 spacing = gridLayout.spacing;
            RectOffset padding = gridLayout.padding;
            int gridWidth = Mathf.FloorToInt((gridRectTransform.rect.width - padding.horizontal + spacing.x) / (cellSize.x + spacing.x));
            int gridHeight = Mathf.FloorToInt((gridRectTransform.rect.height - padding.vertical + spacing.y) / (cellSize.y + spacing.y));

            // Calculate the position inside the content area (after padding)
            float contentX = positionInRect.x - padding.left;
            float contentY = positionInRect.y - padding.bottom;

            // Calculate the raw column and row, assuming origin is bottom-left
            int rawCol = Mathf.FloorToInt(contentX / (cellSize.x + spacing.x));
            int rawRow = Mathf.FloorToInt(contentY / (cellSize.y + spacing.y));

            // Now, adjust these raw coordinates based on the GridLayoutGroup's startCorner
            int finalCol = rawCol;
            int finalRow = rawRow;

            // If the start corner is on the right, the column order is reversed.
            if (gridLayout.startCorner == GridLayoutGroup.Corner.UpperRight || gridLayout.startCorner == GridLayoutGroup.Corner.LowerRight)
            {
                finalCol = (gridWidth - 1) - rawCol;
            }

            // If the start corner is at the top, the row order is reversed.
            if (gridLayout.startCorner == GridLayoutGroup.Corner.UpperLeft || gridLayout.startCorner == GridLayoutGroup.Corner.UpperRight)
            {
                finalRow = (gridHeight - 1) - rawRow;
            }

            // Clamp values to be within the grid boundaries
            finalCol = Mathf.Clamp(finalCol, 0, gridWidth - 1);
            finalRow = Mathf.Clamp(finalRow, 0, gridHeight - 1);

            // Calculate the final 1D index based on the startAxis
            int targetIndex;
            if (gridLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
            {
                targetIndex = finalRow * gridWidth + finalCol;
            }
            else // Vertical Axis
            {
                targetIndex = finalCol * gridHeight + finalRow;
            }

            // --- Final Check and Inventory Update ---
            if (targetIndex >= 0 && targetIndex < slots.Length)
            {
                if (inventory.SlotEmpty(targetIndex))
                {
                    Debug.Log($"Dropped on slot: {targetIndex} (Col: {finalCol}, Row: {finalRow})");

                    inventory.RemoveIcon(droppedSlot.index);
                    inventory.InsertIcon(targetIndex, draggedIcon);
                    SaveInventoryState();
                    RefreshAllSlots();
                }
                else
                {
                    // Slot is occupied: swap icons
                    Debug.Log($"Target slot {targetIndex} is occupied. Swapping icons.");

                    if (inventory.GetIcon(targetIndex, out Icon targetIcon))
                    {
                        // Remove both icons
                        inventory.RemoveIcon(droppedSlot.index);
                        inventory.RemoveIcon(targetIndex);

                        // Insert swapped icons
                        inventory.InsertIcon(targetIndex, draggedIcon);
                        inventory.InsertIcon(droppedSlot.index, targetIcon);

                        SaveInventoryState();
                        RefreshAllSlots();
                    }
                    else
                    {
                        Debug.LogWarning($"Could not retrieve icon at occupied slot {targetIndex} for swapping.");
                        RefreshAllSlots();
                    }
                }
            }
            else
            {
                // Invalid slot index: revert to original position (no change)
                Debug.Log($"Dropped on invalid slot index: {targetIndex}. Reverting to original position.");
                RefreshAllSlots();
            }
        }
    }

    private void SaveInventoryState()
    {
        SerializedJSON<Inventory>.SaveScriptableObject(KeyName, inventory);
    }

    private void LoadInventoryState()
    {
        SerializedJSON<Inventory>.LoadScriptableObject(KeyName, out inventory);
    }
}
