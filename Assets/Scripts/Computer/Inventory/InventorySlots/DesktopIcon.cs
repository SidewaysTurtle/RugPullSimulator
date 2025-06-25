using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DesktopIcon : IconInventorySlot, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private Image imageObject;
    [SerializeField] private Image focusBackgroundImage; // Blue background for focus state
    private CanvasGroup canvasGroup; // Cache reference
    private Icon currentIcon; // Store the current icon data
    private Selectable selectable; // Unity's built-in selectable component

    protected override void Awake()
    {
        base.Awake();
        if (PhysicalRepresentation != null)
        {
            canvasGroup = PhysicalRepresentation.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = PhysicalRepresentation.AddComponent<CanvasGroup>();
            }
            
            // Add Selectable component for Unity's focus system
            selectable = PhysicalRepresentation.GetComponent<Selectable>();
            if (selectable == null)
            {
                selectable = PhysicalRepresentation.AddComponent<Button>();
                // Configure the button to not have any visual transitions by default
                Button button = selectable as Button;
                if (button != null)
                {
                    button.transition = Selectable.Transition.None;
                }
            }
        }
        
        // Create focus background if it doesn't exist
        CreateFocusBackground();
        SetAlpha(0);
    }

    /// <summary>
    /// Creates the blue focus background image if it doesn't exist
    /// </summary>
    private void CreateFocusBackground()
    {
        if (focusBackgroundImage == null && PhysicalRepresentation != null)
        {
            // Create a new GameObject for the focus background
            GameObject focusBackground = new GameObject("FocusBackground");
            focusBackground.transform.SetParent(PhysicalRepresentation.transform, false);
            
            // Add RectTransform and set it to fill the parent
            RectTransform focusRect = focusBackground.AddComponent<RectTransform>();
            focusRect.anchorMin = Vector2.zero;
            focusRect.anchorMax = Vector2.one;
            focusRect.offsetMin = Vector2.zero;
            focusRect.offsetMax = Vector2.zero;
            
            // Add Image component with blue color
            focusBackgroundImage = focusBackground.AddComponent<Image>();
            focusBackgroundImage.color = new Color(0.2f, 0.6f, 1f, 0.3f); // Light blue with transparency
            focusBackgroundImage.raycastTarget = false; // Don't interfere with clicks
            
            // Set as first child so it renders behind the icon
            focusBackground.transform.SetAsFirstSibling();
            
            // Start with focus background disabled
            focusBackground.SetActive(false);
        }
    }

    public override void SetIcon(Icon icon, int slotIndex)
    {
        index = slotIndex;  // Make sure we store the index
        currentIcon = icon; // Store the icon data
        
        if (icon == null) 
        {
            ClearSlot();
            return;
        }

        textObject.text = icon.name;
        imageObject.sprite = icon.image;
        SetAlpha(1);
        
        // Enable selectable when icon is present
        if (selectable != null)
        {
            selectable.interactable = true;
        }
        
        // Only hide the visual representation, keep the slot active
        if (PhysicalRepresentation != null)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 1;
        }
    }

    public override void ClearSlot()
    {
        currentIcon = null; // Clear the stored icon data
        textObject.text = "";
        SetAlpha(0);
        SetFocused(false); // Clear focus when clearing slot
        
        // Disable selectable when no icon is present
        if (selectable != null)
        {
            selectable.interactable = false;
            // Deselect if currently selected
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == selectable.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        
        if (PhysicalRepresentation != null)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 0;
        }
        index = -1;  // Reset index when cleared
    }

    /// <summary>
    /// Sets the focus state of this icon
    /// </summary>
    /// <param name="focused">Whether the icon should be focused</param>
    public void SetFocused(bool focused)
    {
        if (focusBackgroundImage != null)
        {
            focusBackgroundImage.gameObject.SetActive(focused);
        }
    }

    /// <summary>
    /// Gets whether this icon is currently focused
    /// </summary>
    /// <returns>True if focused, false otherwise</returns>
    public bool IsFocused()
    {
        return EventSystem.current != null && 
               EventSystem.current.currentSelectedGameObject == PhysicalRepresentation;
    }

    /// <summary>
    /// Gets the current icon data
    /// </summary>
    /// <returns>The current icon data, or null if none</returns>
    public Icon GetCurrentIcon()
    {
        return currentIcon;
    }

    // Unity's focus event handlers
    public void OnSelect(BaseEventData eventData)
    {
        SetFocused(true);
        currentIcon?.InvokeFocusEvent();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetFocused(false);
        currentIcon?.InvokeUnfocusEvent();
    }

    private void SetAlpha(float alpha)
    {
        if (textObject != null)
        {
            Color textColor = textObject.color;
            textColor.a = alpha;
            textObject.color = textColor;
        }

        if (imageObject != null)
        {
            Color imageColor = imageObject.color;
            imageColor.a = alpha;
            imageObject.color = imageColor;
        }
    }

    private void EnsureCanvasGroup()
    {
        if (canvasGroup == null && PhysicalRepresentation != null)
        {
            canvasGroup = PhysicalRepresentation.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = PhysicalRepresentation.AddComponent<CanvasGroup>();
            }
        }
    }

    public override void OnBeginDrag()
    {
        if (!IsEmpty())
        {
            SetAlpha(0f);
        }
    }

    public override void OnEndDrag()
    {
        if (!IsEmpty())
        {
            SetAlpha(1f);
        }
    }

    public Sprite GetIconSprite()
    {
        return imageObject.sprite;
    }

    public string GetIconText()
    {
        return textObject.text;
    }

    public void TransferDataFrom(DesktopIcon source)
    {
        if (source == null) return;
        
        textObject.text = source.GetIconText();
        imageObject.sprite = source.GetIconSprite();
        currentIcon = source.GetCurrentIcon(); // Transfer icon data
        SetAlpha(1);
        PhysicalRepresentation.SetActive(true);
    }

    public void ClearData()
    {
        currentIcon = null;
        textObject.text = "";
        imageObject.sprite = null;
        SetAlpha(0);
        SetFocused(false);
        PhysicalRepresentation.SetActive(false);
    }

    public bool IsEmpty()
    {
        // Check if both the image and text are effectively invisible
        return imageObject.color.a < 0.1f && textObject.color.a < 0.1f;
    }
}
