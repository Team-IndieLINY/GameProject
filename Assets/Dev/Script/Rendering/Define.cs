using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;



public enum EHousePassEvent
{
    None,
    Inside,
    Outside,
    Back,
}

public enum EHouseBroadcastEvent
{
    None,
    Reset,
    EnableVisible,
    EnableCollider,
    DisableVisible,
    DisableCollider,
    
    SetTransparently,
}

public struct HouseEvent
{
    public EHousePassEvent PassEvent;
    [CanBeNull] public OrderedActor TriggerActor;
}


[System.Serializable]
public enum EOrderedObjectType
{
    Front,
    FrontCollision,
    Back,
    BackCollision,
    Floor,
    Inside
}

public interface IOrderedDescriptor
{
    public OrderedObject Owner { get; set; }
    public void OnEvent(EHouseBroadcastEvent e);
}