using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableItemData : ItemData
{
    [SerializeField] private int _maxStack;

    public int MaxStack => _maxStack;
}
