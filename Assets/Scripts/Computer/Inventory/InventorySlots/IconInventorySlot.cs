using UnityEngine;

[System.Serializable]
public abstract class IconInventorySlot : MonoBehaviour
{
    public int index;
    public GameObject PhysicalRepresentation;
    private Vector3 originalPosition;
    protected virtual void Awake()
    {
        DragManager.OnEndDraggedEvent += OnEndDrag;
        InventoryPhysical.OnSetSlotEvent += SetSlot;
        InventoryPhysical.OnRemoveSlotEvent += RemoveSlot;
    }
    [HideInInspector] public float distanceToDroppedIcon = float.MaxValue;
    public void OnEndDrag(Vector3 position)
    {
        distanceToDroppedIcon = Vector3.Distance(position, transform.localPosition);
        if(distanceToDroppedIcon > 0.106067f)
        {
            distanceToDroppedIcon = float.MaxValue;
        }
        
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
