using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class OrderedObjectSprite : OrderedObject
{
    private SpriteRenderer _renderer;

    public override Color Color
    {
        get => _renderer.color;
        set => _renderer.color = value;
    }

    public override bool IsEnabledRenderer
    {
        get => _renderer.enabled;
        set => _renderer.enabled = value;
    }

    public override int Stencil { get; set; }
    public override bool CollisionEnabled { get; set; }
    public override void Init()
    {
        Descriptor = new DefaultDescriptor()
        {
            Owner = this
        };
        _renderer = GetComponent<SpriteRenderer>();
        
        Debug.Assert(_renderer);
    }
}
