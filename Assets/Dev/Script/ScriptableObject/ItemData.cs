using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _itemSprite;
    
    public string ItemName => _itemName;
    public Sprite ItemSprite => _itemSprite;
}
