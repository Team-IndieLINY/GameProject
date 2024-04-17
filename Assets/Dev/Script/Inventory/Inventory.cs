using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Inventory : MonoBehaviour
{
    [SerializeField] protected InventoryData _inventoryData;
    [SerializeField] protected Slot[] _slots;
    
    protected InventoryUI _inventoryUI;

    #region private method
    
    #endregion

    #region public method

    public void OpenInventory()
    {
        _inventoryUI.OpenInventory();
    }

    public void CloseInventory()
    {
        _inventoryUI.CloseInventory();
        if (this is BoxInventory)
        {
            (this as BoxInventory)?.CancelLooting();
        }
    }

    public bool IsOpened()
    {
        return _inventoryUI.IsOpened();
    }
    
    public void SetTooltipUI(Item item)
    {
        _inventoryUI.SetTooltipUI(item);
    }

    public void OpenTooltipUI()
    {
        _inventoryUI.OpenTooltipUI();
    }
    
    public void CloseTooltipUI()
    {
        _inventoryUI.CloseTooltipUI();
    }

    public void SetTooltipUIPosition(Vector2 mousePosition)
    {
        _inventoryUI.SetTooltipUIPosition(mousePosition);
    }
    
    public void AddItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item is CountableItem)
        {
            CountableItem countableItem = item as CountableItem;
            
            if (countableItem.CurrentAmount == 0)
            {
                return;
            }
        }
        
        foreach (var slot in _slots)
        {
            item = slot.AddItem(item);
            slot.SlotUI.UpdateSlotUI();
        }
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
            _slots[i].SetSlotInSlotUI();
        }

        foreach (var slot in _slots)
        {
            _inventoryUI.SlotContainerVisualElement.Add(slot.SlotUI);
        }
        
        _inventoryUI.SetInventoryUI(_inventoryData);
    }

    public void ResetInventory()
    {
        foreach (var slot in _slots)
        {
            slot.ResetSlot();
        }
    }

    public bool IsFull()
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
