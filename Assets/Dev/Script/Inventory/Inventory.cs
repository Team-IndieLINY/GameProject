using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryData _inventoryData;
    [SerializeField] private Slot[] _slots;
    
    protected InventoryUI _inventoryUI;

    public Slot[] Slots => _slots;
    
    #region private method

    protected void SetInventory()
    {
        _slots = new Slot[_inventoryData.CellCount];

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = new Slot();
        }

        foreach (var slot in _slots)
        {
            _inventoryUI.BodyVisualElement.Add(slot.SlotUI);
        }
        
        _inventoryUI.SetInventoryUI(_inventoryData);
    }

    #endregion

}
