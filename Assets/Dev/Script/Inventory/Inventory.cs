using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryUI))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryData _inventoryData;
    [SerializeField] private Slot[] _slots;
    
    private InventoryUI _inventoryUI;

    #region event method

    private void Awake()
    {
        _inventoryUI = GetComponent<InventoryUI>();
    }

    private void Start()
    {
        SetInventory();
    }

    #endregion

    #region public method

    #endregion
    
    #region private method

    private void SetInventory()
    {
        _slots = new Slot[_inventoryData.CellCount];
        _inventoryUI.SetInventoryUI(_inventoryData);
    }

    #endregion

}
