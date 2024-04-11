using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxInventoryUI))]
public class BoxInventory : Inventory
{
    private static BoxInventory _instance = null;
    public static BoxInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance;
        }
    }
    
    [SerializeField] private CountableItemData _testItem;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _inventoryUI = GetComponent<BoxInventoryUI>();
    }

    private void Start()
    {
        SetInventory();
        
        AddItem(new CountableItem(_testItem, 3));
        AddItem(new CountableItem(_testItem, 2));
        AddItem(new CountableItem(_testItem, 2));
    }
}
