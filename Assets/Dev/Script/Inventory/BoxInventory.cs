using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxInventoryUI))]
public class BoxInventory : Inventory
{
    private void Awake()
    {
        _inventoryUI = GetComponent<BoxInventoryUI>();
    }

    private void Start()
    {
        SetInventory();
    }
}
