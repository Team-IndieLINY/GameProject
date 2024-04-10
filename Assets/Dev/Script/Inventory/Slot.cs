using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Slot
{
    private Item _item;
    public Item Item => _item;

    #region public method
    
    
    //아이템을 슬롯에 추가하고 남는 아이템을 반환합니다.
    [CanBeNull]
    public Item AddItem([NotNull] Item item)
    {
        if (IsEmpty())
        {
            _item = item;
            return null;
        }

        if (IsFull())
        {
            return item;
        }

        if (_item.Equals(item) is not true)
        {
            return item;
        }

        CountableItem countableItem = (CountableItem)item;

        int sumAmount = countableItem.CurrentAmount + ((CountableItem)_item).CurrentAmount;
        int itemMaxAmount = ((CountableItemData)countableItem.ItemData).MaxStack;

        if (sumAmount > itemMaxAmount)
        {
            int restAmount = sumAmount - itemMaxAmount;
            
            ((CountableItem)_item).SetAmount(itemMaxAmount);
            countableItem.SetAmount(restAmount);
            
            return countableItem;
        }
        
        ((CountableItem)_item).SetAmount(sumAmount);

        return null;
        //UI 추가
    }

    public void RemoveItem()
    {
        
    }

    //현재 슬롯이 꽉 차있는지 체크합니다. (카운트 가능한 아이템이 아닌 경우에도 true를 반환합니다.)
    public bool IsFull()
    {
        if (IsEmpty())
        {
            return false;
        }
        
        if (_item is not CountableItem countableItem)
        {
            return true;
        }

        Debug.Assert(countableItem.ItemData == null, "countableItem.ItemData == null");

        if (countableItem.CurrentAmount >= ((CountableItemData)countableItem.ItemData).MaxStack)
        {
            return true;
        }

        return false;
    }

    public bool IsEmpty()
    {
        if (_item == null)
            return true;

        return false;
    }
    #endregion
}
