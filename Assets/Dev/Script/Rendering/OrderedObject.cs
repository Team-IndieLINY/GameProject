using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

public interface IOrderedState
{
    public OrderedObject Owner { get; set; }
    public void OnEvent(EHouseBroadcastEvent e);
}

public class DefaultOrderedState : IOrderedState
{
    public OrderedObject Owner { get; set; }

    public void OnEvent(EHouseBroadcastEvent e)
    {
        if (e == EHouseBroadcastEvent.Hide)
        {
            Owner.IsEnabledRenderer = false;
        }
        if (e == EHouseBroadcastEvent.Show)
        {
            Owner.IsEnabledRenderer = true;
        }
        if (e == EHouseBroadcastEvent.Reset)
        {
            Owner.IsEnabledRenderer = true;
        }
    }
}

public abstract class OrderedObject : MonoBehaviour
{
    public static readonly int StencilField = Shader.PropertyToID("_Stencil");
    public static readonly int DefaultStencilNumber = 2;
    
    [SerializeField] private EOrderedObjectType _type;
    public EOrderedObjectType Type => _type;

    public abstract Color Color { get; set; }

    public abstract bool IsEnabledRenderer { get; set; }


    public abstract int Stencil { get; set; }

    public abstract bool CollisionEnabled { get; set; }

    public abstract void Init();

    public IOrderedState State { get; protected set; }
    
}
