using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedObjectSprite : OrderedObject
{
    private SpriteRenderer _renderer;
    private Collider2D _collider2d;

    public override Color Color
    {
        get=>_renderer.color;
        set => _renderer.color = value;
    }

    public override bool IsEnabled
    {
        get => _renderer.enabled;
        set => _renderer.enabled = value;
    }
    public override int CurrentFloor { get; set; }
    public override Renderer Renderer => _renderer;

    public override bool CollisionEnabled
    {
        get => _collider2d && _collider2d.enabled;
        set
        {
            if (_collider2d)
            {
                _collider2d.enabled = value;
            }
        }
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider2d = GetComponent<Collider2D>();
        base.Awake();
    }
}
