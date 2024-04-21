using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


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

    public IOrderedDescriptor Descriptor { get; protected set; }
    
}
