using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultDescriptor : IOrderedDescriptor
{
    public OrderedObject Owner { get; set; }

    public void OnEvent(EHouseBroadcastEvent e)
    {
        if (e == EHouseBroadcastEvent.EnableVisible)
        {
            Owner.IsEnabledRenderer = true;
        }
        if (e == EHouseBroadcastEvent.DisableVisible)
        {
            Owner.IsEnabledRenderer = false;
        }
        if (e == EHouseBroadcastEvent.EnableCollider)
        {
            Owner.CollisionEnabled = true;
        }
        if (e == EHouseBroadcastEvent.DisableCollider)
        {
            Owner.CollisionEnabled = false;
        }
        if (e == EHouseBroadcastEvent.Reset)
        {
            Owner.IsEnabledRenderer = true;
            Owner.CollisionEnabled = true;
        }
        if (e == EHouseBroadcastEvent.SetTransparently)
        {
            Owner.IsEnabledRenderer = false;
        }
    }
}