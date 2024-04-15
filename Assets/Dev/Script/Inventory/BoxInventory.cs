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

    #region private method

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
    }

    #endregion


    #region public method

    //박스 인벤토리에서 루팅이 진행 중인지 알려주는 함수
    public bool IsLooting()
    {
        foreach (var slot in _slots)
        {
            if (slot.SlotUI.IsLooting())
            {
                return true;
            }
        }

        return false;
    }

    //박스 인벤토리에서 진행되고 있는 루팅을 캔슬하는 함수
    public void CancelLooting()
    {
        foreach (var slot in _slots)
        {
            if (slot.SlotUI.IsLooting())
            {
                slot.SlotUI.CancelLoot();
            }
        }
    }

    public async void TakeAll()
    {
        if (PlayerInventory.Instance.IsFull())
        {
            return;
        }

        if (IsLooting())
        {
            CancelLooting();
            return;
        }

        
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].IsEmpty())
            {
                continue;
            }

            try
            {
                await _slots[i].SlotUI.Loot();
            }
            catch
            {
                return;
            }

            i--;
        }
    }
    #endregion
}
