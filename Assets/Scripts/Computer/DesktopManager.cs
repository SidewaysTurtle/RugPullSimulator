using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopManager : InventoryPhysical
{
    protected override void Awake()
    {
        base.Awake();
    }

    void OnEnable()
    {
        iconInventorySlots = new List<IconInventorySlot>();

        // This is a bad way to do this. Slow.
        iconInventorySlots.AddRange(IconInventorySlot.FindObjectsByType<IconInventorySlot>(FindObjectsSortMode.InstanceID));
        iconInventorySlots.Sort((a, b) => a.index - b.index);
    }

    protected override void Start()
    {
        base.Start();
    }
}
