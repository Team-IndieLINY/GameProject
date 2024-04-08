using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedActor : MonoBehaviour
{
    [SerializeField] private bool _isPlayer = false;
    public bool IsPlayer => _isPlayer;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public int Order
    {
        get
        {
            return _renderer.sortingOrder;
        }
        set
        {
            _renderer.sortingOrder = value;
        }
    }
    
}
