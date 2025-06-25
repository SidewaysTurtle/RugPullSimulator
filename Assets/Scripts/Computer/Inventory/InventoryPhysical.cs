using System.Collections.Generic;
using UnityEngine;

using SerializedJSONSystem; // Fix typo in namespace name

[System.Serializable, ExecuteInEditMode]
public class InventoryPhysical : MonoBehaviour
{
    // Delegates
    public delegate void OnSetSlot(Inventory inventory);
    public static event OnSetSlot OnSetSlotEvent;
    public delegate void OnRemoveSlot();
    public static event OnRemoveSlot OnRemoveSlotEvent;
    public delegate void OnCreateWindow(Icon icon, IconInventorySlot slot);
    public static event OnCreateWindow OnCreateWindowEvent;
    
    // Instance of the inventory Scriptable Object
    protected Inventory inventory;
    protected IconInventorySlot[] slots;
    [SerializeField] protected string KeyName = "Inventory";

    // public: gets used in drag manager

    protected virtual void Awake() // Right after Awake in execution order
    {
        // it requires the inventory scriptable object
        IconInputManager.OnDoubleClickEvent += DoubleClickEvent;
    }
    /// <summary>
    /// Populates the inventory with some icons.
    /// </summary>
    public void PopulateInitial()
    {
        OnSetSlotEvent?.Invoke(inventory);
    }
    
    /// <summary>
    /// Removes the icon from all the non-specified slots.
    /// </summary>
    public void Clear() 
    {
        OnRemoveSlotEvent?.Invoke();
    }

    /// <summary>
    /// Removes the icon from all slots then populates the inventory with the specified icons.
    /// </summary>
    public void Refresh() 
    {
        // TODO: This is a bit of a hack.
            // This should be some sort of "should assemble" 
            // flag that gets called etc etc
        Clear();
        PopulateInitial();
    }

    // Remove OnDrop method as it's handled in DesktopManager

    public void DoubleClickEvent(IconInventorySlot slot)
    {
        Icon icon;
        // Make the classes subscribed to this event call the appropriate method if the type is correct...
        if(inventory.GetIcon(slot.index, out icon))
            OnCreateWindowEvent?.Invoke(icon, slot);
    }

    void OnApplicationQuit()
    {
        if (inventory != null)
        {
            SerializedJSON<Inventory>.SaveScriptableObject(KeyName, inventory);
        }
    }

    [ContextMenu("Delete Saved Desktop JSON")]
    public void DeleteSavedDesktopJSON()
    {
        SerializedJSON<Inventory>.DeleteScriptableObject(KeyName);
    }
}
