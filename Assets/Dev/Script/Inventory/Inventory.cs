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
    
    #region private method
    #endregion

    #region public method
    
    public void AddItem(Item item)
    {
        foreach (var slot in _slots)
        {
            item = slot.AddItem(item);
            slot.SlotUI.UpdateSlotUI(slot.Item);

            if (item == null)
            {
                return;
            }
        }
        
        Debug.Log("Inventory is Full");
    }

    public void RemoveAtItem(int slotIndex)
    {
        
    }

    #endregion
    protected void SetInventory()
    {
        _slots = new Slot[_inventoryData.CellCount];

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = new Slot();
        }

        foreach (var slot in _slots)
        {
            _inventoryUI.SlotContainerVisualElement.Add(slot.SlotUI);
        }
        
        _inventoryUI.SetInventoryUI(_inventoryData);
    }

    protected bool IsFull()
    {
        foreach (var slot in _slots)
        {
            if (slot.IsFull() is not true)
            {
                return false;
            }
        }

        return true;
    }


}
