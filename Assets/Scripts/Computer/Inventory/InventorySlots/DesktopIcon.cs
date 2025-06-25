using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DesktopIcon : IconInventorySlot
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private Image imageObject;
    private CanvasGroup canvasGroup; // Cache reference

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
        }
        SetAlpha(0);
    }

    public override void SetIcon(Icon icon, int slotIndex)
    {
        index = slotIndex;  // Make sure we store the index
        if (icon == null) 
        {
            ClearSlot();
            return;
        }

        textObject.text = icon.name;
        imageObject.sprite = icon.image;
        SetAlpha(1);
        // Only hide the visual representation, keep the slot active
        if (PhysicalRepresentation != null)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 1;
        }
    }

    public override void ClearSlot()
    {
        textObject.text = "";
        SetAlpha(0);
        if (PhysicalRepresentation != null)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 0;
        }
        index = -1;  // Reset index when cleared
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
        SetAlpha(1);
        PhysicalRepresentation.SetActive(true);
    }

    public void ClearData()
    {
        textObject.text = "";
        imageObject.sprite = null;
        SetAlpha(0);
        PhysicalRepresentation.SetActive(false);
    }

    public bool IsEmpty()
    {
        // Check if both the image and text are effectively invisible
        return imageObject.color.a < 0.1f && textObject.color.a < 0.1f;
    }
}
