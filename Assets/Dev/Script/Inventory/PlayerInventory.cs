using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInventory : Inventory
{
    [SerializeField] private CountableItemData _testItem;
    private void Awake()
    {
        _inventoryUI = GetComponent<PlayerInventoryUI>();
    }

    private void Start()
    {
        SetInventory();

        AddItem(new CountableItem(_testItem, 3));
        AddItem(new CountableItem(_testItem, 2));
        AddItem(new CountableItem(_testItem, 2));
    }
}
