using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
