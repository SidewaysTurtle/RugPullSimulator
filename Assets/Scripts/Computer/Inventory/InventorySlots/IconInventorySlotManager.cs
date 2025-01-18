using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconInventorySlotManager : IconInventorySlot
{
    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private Image imageObject;

    protected override void Awake()
    {
        base.Awake();
        SetAlpha(0);
    }

    public void SetIcon(Icon icon, int slotIndex)
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
        PhysicalRepresentation.SetActive(true);
    }

    public void ClearSlot()
    {
        textObject.text = "";
        SetAlpha(0);
        PhysicalRepresentation.SetActive(false);
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

    public override void OnBeginDrag()
    {
        SetAlpha(0.4f); // Make semi-transparent during drag
    }

    public override void OnEndDrag()
    {
        SetAlpha(1f); // Restore full opacity after drag
    }

    public Sprite GetIconSprite()
    {
        return imageObject.sprite;
    }

    public string GetIconText()
    {
        return textObject.text;
    }

    public void TransferDataFrom(IconInventorySlotManager source)
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
