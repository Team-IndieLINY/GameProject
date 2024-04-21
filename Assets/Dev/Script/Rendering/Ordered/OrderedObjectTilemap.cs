using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer))]
public class OrderedObjectTilemap : OrderedObject
{
    private TilemapRenderer _renderer;
    private TilemapCollider2D _collider;
    
    public override Color Color
    {
        get => _renderer.material.color;
        set => _renderer.material.color = value;
    }

    public override bool IsEnabledRenderer
    {
        get => _renderer.enabled;
        set => _renderer.enabled = value;
    }
    public override int Stencil { get; set; }

    public override bool CollisionEnabled
    {
        get
        {
            if (_collider)
            {
                return _collider.enabled;
            }

            return false;
        }
        set
        {
            if (_collider)
            {
                _collider.enabled = value;
            }
        }
    }
    
    public override void Init()
    {
        Descriptor = new DefaultDescriptor()
        {
            Owner = this
        };
        
        _renderer = GetComponent<TilemapRenderer>();
        Debug.Assert(_renderer);

        _collider = GetComponent<TilemapCollider2D>();
    }
}