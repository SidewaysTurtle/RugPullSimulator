using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Camera _Camera;
    public RectTransform _GameObjectRectTransform;
    public Canvas _Canvas;
    public RectTransform _CanvasRectTransform;
    
    private Vector2 dragOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _GameObjectRectTransform, 
            eventData.position, 
            _Camera, 
            out dragOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _CanvasRectTransform,
            eventData.position,
            _Camera,
            out mousePos);

        // Calculate new position with offset
        Vector2 newPos = mousePos - dragOffset;

        // Apply screen bounds
        float width = _GameObjectRectTransform.rect.width * _GameObjectRectTransform.localScale.x;
        float height = _GameObjectRectTransform.rect.height * _GameObjectRectTransform.localScale.y;
        
        newPos.x = Mathf.Clamp(newPos.x, -(_CanvasRectTransform.rect.width/2) + width/2, (_CanvasRectTransform.rect.width/2) - width/2);
        newPos.y = Mathf.Clamp(newPos.y, -(_CanvasRectTransform.rect.height/2) + height/2, (_CanvasRectTransform.rect.height/2) - height/2);

        _GameObjectRectTransform.anchoredPosition = newPos;
    }
}
