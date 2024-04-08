using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrderedObjectTilemap : OrderedObject
{
    public override Color Color
    {
        get => _tilemap.color;
        set => _tilemap.color = value;
    }

    public override bool IsEnabled
    {
        get => _renderer.enabled;
        set => _renderer.enabled = value;
    }

    public override int CurrentFloor { get; set; }

    private TilemapRenderer _renderer;
    private Tilemap _tilemap;

    private TilemapCollider2D _collider;
    private CompositeCollider2D _compositeCollider;
    public override Renderer Renderer => _renderer;

    public override bool CollisionEnabled
    {
        get => _compositeCollider.enabled;
        set
        {
            if (_collider)
                _collider.enabled = value;
        }
    }

    private protected override void Awake()
    {
        _renderer = GetComponent<TilemapRenderer>();
        _tilemap = GetComponent<Tilemap>();
        _collider = GetComponent<TilemapCollider2D>();
        _compositeCollider = GetComponent<CompositeCollider2D>();

        base.Awake();
    }
}