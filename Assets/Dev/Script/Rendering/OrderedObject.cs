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
    Stair,
    Inner
}
public abstract class OrderedObject : MonoBehaviour
{
    [SerializeField] private EOrderedObjectType _type;
    public EOrderedObjectType Type => _type;

    private Material _backupMaterial;
    private Material _createdMaterial;

    public abstract Color Color { get; set; }

    public abstract bool IsEnabled { get; set; }

    public abstract int CurrentFloor { get; set; } 

    public int Stencil
    {
        get => Renderer.material.GetInt("_Stencil");
    }

    public void SetStencilState(bool value)
    {
        if (value)
        {
            _createdMaterial.SetInt("_Stencil", 1);
            Renderer.material = _createdMaterial;
        }
        else
        {
            Renderer.sharedMaterial = _backupMaterial;
        }
    }

    public abstract Renderer Renderer { get; }

    public abstract bool CollisionEnabled { get; set; }

    private protected virtual void Awake()
    {
        _backupMaterial = Renderer.sharedMaterial;
        _backupMaterial.SetInt("Stencil", 2);
        _createdMaterial = Renderer.material;
        Renderer.material = _backupMaterial;
    }
}
