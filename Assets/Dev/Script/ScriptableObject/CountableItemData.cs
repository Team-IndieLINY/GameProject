using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "IndieLINY/ItemData/CountableItemData", fileName = "CountableItemData",order = Int32.MaxValue)]
public class CountableItemData : ItemData
{
    [SerializeField] private int _maxStack;

    public int MaxStack => _maxStack;
}
