using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class CountableItem : Item
{
    [SerializeField] private int _currentAmount = 0;
    public int CurrentAmount => _currentAmount;

    public CountableItem(CountableItemData countableItemData, int currentAmount) : base(countableItemData)
    {
        if (currentAmount >= countableItemData.MaxStack)
        {
            _currentAmount = countableItemData.MaxStack;
        }
        else
        {
            _currentAmount = currentAmount;
        }
    }

    public void SetAmount(int amount)
    {
        Debug.Assert(ItemData != null, "ItemData == null");

        if (amount >= ((CountableItemData)ItemData).MaxStack)
        {
            _currentAmount = ((CountableItemData)ItemData).MaxStack;
            return;
        }
        
        //_currentAmount + amount가 MaxStack을 넘으면 안됨
        _currentAmount = amount;
    }

    public bool IsMax()
    {
        if (_currentAmount >= ((CountableItemData)ItemData).MaxStack)
        {
            return true;
        }
        
        return false;
    }


    public override object Clone()
    {
        return MemberwiseClone();
    }
}
