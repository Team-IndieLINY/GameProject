using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public abstract class Item
{
    [NotNull] private readonly ItemData _itemData;
    public ItemData ItemData => _itemData;

    protected Item(ItemData itemData)
    {
        _itemData = itemData;
    }

    public override bool Equals(object obj)
    {
        return ItemData.Equals((obj as Item)?.ItemData);
    }

    protected bool Equals(Item other)
    {
        return Equals(_itemData, other._itemData);
    }

    public override int GetHashCode()
    {
        return (_itemData != null ? _itemData.GetHashCode() : 0);
    }
}
