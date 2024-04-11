using System.Collections;
using System.Collections.Generic;
using IndieLINY.Singleton;
using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInventory : Inventory
{
    private static PlayerInventory _instance = null;
    public static PlayerInventory Instance
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
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        _inventoryUI = GetComponent<PlayerInventoryUI>();
    }

    private void Start()
    {
        SetInventory();
    }
}
