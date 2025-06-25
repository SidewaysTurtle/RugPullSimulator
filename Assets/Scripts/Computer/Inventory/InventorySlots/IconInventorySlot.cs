using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public abstract class IconInventorySlot : MonoBehaviour
{
    public int index;
    public GameObject PhysicalRepresentation;
    
    protected virtual void Awake()
    {
        InventoryPhysical.OnSetSlotEvent += SetSlot;
        InventoryPhysical.OnRemoveSlotEvent += RemoveSlot;
    }

    public virtual void SetIcon(Icon icon, int slotIndex)
    {

    }

    public virtual void ClearSlot()
    {
    }

    public virtual void OnBeginDrag()
    {
    }

    public virtual void OnEndDrag()
    {
    }

    protected virtual void SetSlot(Inventory inventory)
    {
        PhysicalRepresentation.SetActive(true);
    } 

    protected virtual void RemoveSlot()
    {
        PhysicalRepresentation.transform.localPosition = transform.localPosition;
        PhysicalRepresentation.SetActive(false);
    }
}
