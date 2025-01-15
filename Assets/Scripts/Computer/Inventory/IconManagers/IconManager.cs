using UnityEngine;
using UnityEngine.EventSystems;

/*
    FIXME: Forgot the use of this
*/

[System.Serializable]
public abstract class IconManager<T> : MonoBehaviour
{
    // OnDrop only tests if the mouse is over the canvas.
    // public void OnDrop(PointerEventData data)
    // {
    //     OnDropEvent(data);
    // }

    /// <summary> 
    /// Sets the slot to the specified icon.
    /// </summary>
    /// <param name="instance">The instance of TextIcon.</param>
    public abstract void SetSlot(T instance);

    // Remove the icon from the slot, and destroy the associated gameobject.
    /// <summary>
    /// Removes the icon from the slot.
    /// </summary>
    public abstract void RemoveSlot(T instance);
    public abstract void CreateWindow(IconInventorySlot slot);
}
