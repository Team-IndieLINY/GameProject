using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderdInside<T> : OrderedObject, IOrderedDescriptor
    where T : Renderer
{
    public override Color Color { get; set; }
    public override bool IsEnabledRenderer { get; set; }
    public override int Stencil { get; set; }
    public override bool CollisionEnabled { get; set; }

    private T _renderer;
    
    public override void Init()
    {
        _renderer = GetComponent<T>();
    }

    public OrderedObject Owner { get; set; }
    public void OnEvent(EHouseBroadcastEvent e)
    {
        
    }
}
