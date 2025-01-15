using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManager : MonoBehaviour, IBeginDragHandler, IPointerClickHandler, IEndDragHandler
{
    public delegate void OnIconBeginDrag(IconInventorySlot iconInventorySlot);
    public static event OnIconBeginDrag OnBeginDragEvent;
    public delegate void OnDoubleClick(IconInventorySlot slot);
    public static event OnDoubleClick OnDoubleClickEvent;
    public delegate void OnEndDragged(Vector3 position);
    public static event OnEndDragged OnEndDraggedEvent;
    public delegate void OnIconDrop(IconInventorySlot iconInventorySlot);
    public static event OnIconDrop OnDropEvent;
    public IconInventorySlot self;
    public DragUI dragUI;
    public void OnBeginDrag(PointerEventData eventdata)
    {
        OnBeginDragEvent?.Invoke(self);
    }
    public void OnEndDrag(PointerEventData eventdata)
    {
        OnEndDraggedEvent?.Invoke(eventdata.pointerDrag.transform.localPosition);
        OnDropEvent?.Invoke(self);
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            OnDoubleClickEvent?.Invoke(self);
        }
    }
}
