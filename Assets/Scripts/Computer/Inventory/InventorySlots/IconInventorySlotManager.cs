using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class IconInventorySlotManager : IconInventorySlot
{
    public TextMeshProUGUI textObject;
    public Image imageObject;
    protected override void Awake()
    {
        base.Awake();
        InventoryPhysical.OnSetSlotEvent += SetSlot;
    }

    // Not anymore...
        // Gets called DIRECTLY from Inventory Managers (i.e DesktopManager)
    protected override void SetSlot(Inventory inventory)
    {
        TextIcon icon;
        // FIXME
        // I didnt know how to pass the icon to the physical slot while using a delegate...
        // This is such a stupid solution but it works for now
        if (inventory.GetIcon(index, out icon))
        {
            base.SetSlot(inventory);
            textObject.text = icon.name;
            imageObject.sprite = icon.image;
        }
    }
}
