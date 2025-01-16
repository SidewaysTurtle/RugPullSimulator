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
        index = slotIndex;
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
}
