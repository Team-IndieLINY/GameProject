using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] protected string _itemName;
    [SerializeField] protected Sprite _itemSprite;
    [SerializeField] protected float _lootingTime;
    
    public string ItemName => _itemName;
    public Sprite ItemSprite => _itemSprite;
    public float LootingTime => _lootingTime;
}
