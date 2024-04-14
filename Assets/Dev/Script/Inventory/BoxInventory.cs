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

        AddItem(new CountableItem(_testItem, 3));
        AddItem(new CountableItem(_testItem, 2));
        AddItem(new CountableItem(_testItem, 2));
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
    #endregion
}
