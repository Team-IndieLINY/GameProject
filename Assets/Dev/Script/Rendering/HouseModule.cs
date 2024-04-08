using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HouseModule : MonoBehaviour
{
    [NonSerialized] public List<OrderedObject> fronts = new();
    [NonSerialized] public List<OrderedObject> backs = new();
    [NonSerialized] public List<OrderedObject> floors = new();
    [NonSerialized] public List<OrderedObject> fronts_collider = new();
    [NonSerialized] public List<OrderedObject> backs_collider = new();
    [NonSerialized] public List<OrderedObject> inners = new();
    [NonSerialized] public StairLine[] stairs;

    public int floor;

    private void Awake()
    {
        var objects = GetComponentsInChildren<OrderedObject>(true);

        var myCom = GetComponent<OrderedObject>();

        if (myCom)
        {
            Add(myCom);
        }
        
        foreach (var obj in objects)
        {
            if (obj == false) continue;

            Add(obj);
        }
    }
    
    private void Add(OrderedObject obj)
    {
        List<OrderedObject> addingArray = null;

        switch (obj.Type)
        {
            case EOrderedObjectType.Front:
                addingArray = fronts;
                break;
            case EOrderedObjectType.FrontCollision:
                addingArray = fronts_collider;
                break;
            case EOrderedObjectType.Back:
                addingArray = backs;
                break;
            case EOrderedObjectType.BackCollision:
                addingArray = backs_collider;
                break;
            case EOrderedObjectType.Floor:
                addingArray = floors;
                break;
            case EOrderedObjectType.Stair:
                break;
            case EOrderedObjectType.Inner:
                addingArray = inners;
                break;
            default:
                Debug.Assert(false);
                break;
        }
            
        Debug.Assert(addingArray != null) ;
        addingArray.Add(obj);
    }
}
