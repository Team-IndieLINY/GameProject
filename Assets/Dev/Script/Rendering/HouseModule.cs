using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseModule : MonoBehaviour
{
    private House _masterHouse;
    
    private List<OrderedObject> _front = new();
    private List<OrderedObject> _frontCollider = new();
    private List<OrderedObject> _back = new();
    private List<OrderedObject> _backCollider = new();
    private List<OrderedObject> _floor = new();
    private List<OrderedObject> _inside = new();

    private List<HousePass> _pass = new();
    
    [Range(1, 10)]
    [SerializeField] private int _floorNumer = 1;

    public IReadOnlyList<OrderedObject> Front => _front;

    public IReadOnlyList<OrderedObject> FrontCollider => _frontCollider;

    public IReadOnlyList<OrderedObject> Back => _back;

    public IReadOnlyList<OrderedObject> BackCollider => _backCollider;

    public IReadOnlyList<OrderedObject> Floor => _floor;

    public IReadOnlyList<OrderedObject> Inside => _inside;

    public int FloorNumer => _floorNumer;

    public void Init(House master)
    {
        _masterHouse = master;
        BindOrderedObjects();
    }

    public void ForEach(Action<OrderedObject> callback)
    {
        foreach (var obj in Front)
            callback(obj);
        foreach (var obj in FrontCollider)
            callback(obj);
        foreach (var obj in BackCollider)
            callback(obj);
        foreach (var obj in Floor)
            callback(obj);
        foreach (var obj in Inside)
            callback(obj);
    }

    private void BindOrderedObjects()
    {
        void TransformEnqueue(Queue<Transform> queue, Transform queuedTransform)
        {
            for (int i = 0; i < queuedTransform.childCount; i++)
            {
                queue.Enqueue(queuedTransform.GetChild(i));
            }
        }
        
        var queue = new Queue<Transform>(10);

        queue.Enqueue(transform);
        bool first = true;
        bool queued = false;

        while (queue.Any())
        {
            var queuedTransform = queue.Dequeue();
            if (first && queuedTransform.TryGetComponent<House>(out var house))
            {
                first = false;
                Add(house);
                TransformEnqueue(queue, queuedTransform);
                continue;
            }
            
            if (queuedTransform.TryGetComponent<OrderedObject>(out var com))
            {
                Add(com);
                com.Init();
            }
            if (queuedTransform.TryGetComponent<HousePass>(out var pass))
            {
                _pass.Add(pass);
                pass.Init(_masterHouse.EventReactiveProperty);
            }
            
            if(queuedTransform.TryGetComponent<House>(out _))
            {
                continue;
            }
            
            TransformEnqueue(queue, queuedTransform);
        }
    }
    
    private void Add(OrderedObject obj)
    {
        List<OrderedObject> addingArray = null;

        switch (obj.Type)
        {
            case EOrderedObjectType.Front:
                addingArray = _front;
                break;
            case EOrderedObjectType.FrontCollision:
                addingArray = _frontCollider;
                break;
            case EOrderedObjectType.Back:
                addingArray = _back;
                break;
            case EOrderedObjectType.BackCollision:
                addingArray = _backCollider;
                break;
            case EOrderedObjectType.Floor:
                addingArray = _floor;
                break;
            case EOrderedObjectType.Inside:
                addingArray = _inside;
                break;
            default:
                Debug.Assert(false, $"정의되지 않은 EOrderedObjectType({obj.gameObject.name}): " + obj.Type);
                return;
        }
        
        Debug.Assert(addingArray != null);
        addingArray.Add(obj);
    }
}
