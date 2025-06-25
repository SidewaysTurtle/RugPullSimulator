using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragManager : MonoBehaviour, IBeginDragHandler, IPointerClickHandler, IEndDragHandler
{
    public delegate void OnIconBeginDrag(DesktopIcon iconInventorySlot);
    public static event OnIconBeginDrag OnBeginDragEvent;
    public delegate void OnDoubleClick(DesktopIcon slot);
    public static event OnDoubleClick OnDoubleClickEvent;
    public delegate void OnEndDragged(Vector3 position);
    public static event OnEndDragged OnEndDraggedEvent; // For mouse position, but its not really used
    public delegate void OnIconDrop(DesktopIcon iconInventorySlot);
    public static event OnIconDrop OnDropEvent;
    public DesktopIcon self;
    public DragUI dragUI;

    private struct DragVisual
    {
        public GameObject visualObject;
        public Image iconImage;
        public TextMeshProUGUI label;

        public void Initialize(Transform parent, Sprite icon, string text)
        {
            visualObject = new GameObject("DragVisual");
            visualObject.transform.SetParent(parent);
            RectTransform visualRect = visualObject.AddComponent<RectTransform>();
            
            // Add VerticalLayoutGroup
            VerticalLayoutGroup layout = visualObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 5f;
            layout.childControlHeight = false; // Changed to false
            layout.childControlWidth = false;  // Changed to false
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            
            // Create icon container
            GameObject iconObj = new GameObject("Icon");
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconObj.transform.SetParent(visualObject.transform);
            iconImage = iconObj.AddComponent<Image>();
            iconImage.sprite = icon;
            iconImage.raycastTarget = false;
            iconImage.preserveAspect = true;
            iconRect.sizeDelta = new Vector2(130, 130); // Updated size to 130x130
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            
            // Create text container
            GameObject textObj = new GameObject("Label");
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textObj.transform.SetParent(visualObject.transform);
            label = textObj.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.color = Color.black;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 1;
            label.fontSizeMax = 72;
            label.raycastTarget = false;
            textRect.sizeDelta = new Vector2(160, 30);
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            
            // Set the container size
            visualRect.sizeDelta = new Vector2(160, 180);  // Increased overall container size
            visualRect.anchorMin = new Vector2(0.5f, 0.5f);
            visualRect.anchorMax = new Vector2(0.5f, 0.5f);
            visualRect.pivot = new Vector2(0.5f, 0.5f);
        }

        public void UpdatePosition(Vector3 position)
        {
            if (visualObject != null)
            {
                visualObject.transform.position = position;
            }
        }

        public void Destroy()
        {
            if (visualObject != null)
            {
                GameObject.Destroy(visualObject);
            }
        }
    }

    private DragVisual currentDragVisual;
    private DesktopIcon currentDraggedSlot;
    private Vector3 dragOffset; // Store offset between mouse and icon

    private void SetupDragVisual()
    {
        if (currentDragVisual.visualObject != null)
        {
            // Ensure drag visual doesn't block raycasts
            CanvasGroup canvasGroup = currentDragVisual.visualObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f; // Set drag visual to half opacity
        }
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        // Only allow drag if the slot is not empty
        if (self != null && !self.IsEmpty())
        {
            currentDraggedSlot = self;
            currentDragVisual.Initialize(transform, 
                currentDraggedSlot.GetIconSprite(), 
                currentDraggedSlot.GetIconText());
            SetupDragVisual();
            currentDraggedSlot.OnBeginDrag();
            OnBeginDragEvent?.Invoke(self);
            // Calculate offset between mouse and icon position
            RectTransform slotRect = currentDraggedSlot.GetComponent<RectTransform>();
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(slotRect, eventdata.position, eventdata.pressEventCamera, out localMousePos);
            dragOffset = localMousePos;
        }
    }

    private void Update()
    {
        if (currentDraggedSlot != null)
        {
            // Place the drag visual at the mouse position minus the offset
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mousePos - dragOffset;
            currentDragVisual.UpdatePosition(worldPos);
        }
    }

    public void OnEndDrag(PointerEventData eventdata)
    {
        if (currentDraggedSlot != null)
        {
            currentDraggedSlot.OnEndDrag();
            currentDragVisual.Destroy();
            OnEndDraggedEvent?.Invoke(eventdata.position);
            OnDropEvent?.Invoke(currentDraggedSlot); // This should be correct, as currentDraggedSlot is the slot being dragged
            currentDraggedSlot = null;
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            OnDoubleClickEvent?.Invoke(self);
        }
    }
}
