using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    private void Awake()
    {
        _inventoryUI = GetComponent<PlayerInventoryUI>();
    }

    private void Start()
    {
        SetInventory();
    }
}
