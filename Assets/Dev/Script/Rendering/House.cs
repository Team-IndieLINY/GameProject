using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

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
    Show,
    Hide
}

public struct HouseEvent
{
    public EHousePassEvent PassEvent;
    public OrderedActor TriggerActor;
}



public class House : OrderedObject, IOrderedState
{
    [Range(1, 10)] [SerializeField] private int _currentFloor = 1;

    private List<HouseModule> _modules = new(10);

    public AsyncReactiveProperty<HouseEvent> EventReactiveProperty { get; private set; } = new(new HouseEvent());

    private Action<HouseEvent> _state;

    private void Awake()
    {
        State = this;
        Owner = this;
        
        BindModules();

        OnUpdate().Forget();
    }

    public override void Init()
    {
        // do nothing here after
    }

    private async UniTaskVoid OnUpdate()
    {
        while (true)
        {
            var e = await EventReactiveProperty.WaitAsync();
            if (e.TriggerActor.IsPlayer == false) return;

            ResetModules();

            switch (e.PassEvent)
            {
                case EHousePassEvent.None:
                    return;
                case EHousePassEvent.Inside:
                    _state = OnInside;
                    break;
                case EHousePassEvent.Outside:
                    _state = OnOutside;
                    break;
                case EHousePassEvent.Back:
                    _state = OnBack;
                    break;
                default:
                    return;
            }

            _state.Invoke(e);
        }
    }

    private void BindModules()
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

        while (queue.Any())
        {
            var queuedTransform = queue.Dequeue();
            
            if (queuedTransform.TryGetComponent<HouseModule>(out var module))
            {
                _modules.Add(module);
                module.Init(this);
            }
            else
            {
                TransformEnqueue(queue, queuedTransform);
            }
            
        }
    }

    public override Color Color { get; set; }
    public override bool IsEnabledRenderer { get; set; }
    public int CurrentFloor => _currentFloor;
    public override int Stencil { get; set; }
    public override bool CollisionEnabled { get; set; }


    private HouseModule CurrentFloorModule
    {
        get
        {
            foreach (var module in _modules)
            {
                if (module.FloorNumer == CurrentFloor)
                {
                    return module;
                }
            }

            Debug.Assert(_modules != null);
            return null;
        }
    }

    private List<HouseModule> _excludedCurrentFloorModule = new();

    private List<HouseModule> ExcludedCurrentFloorModule
    {
        get
        {
            _excludedCurrentFloorModule.Clear();

            foreach (var module in _modules)
            {
                if (module.FloorNumer != CurrentFloor)
                {
                    _excludedCurrentFloorModule.Add(module);
                }
            }

            return _excludedCurrentFloorModule;
        }
    }

    private void ResetModules()
    {
        foreach (HouseModule module in _modules)
        {
            module.ForEach(x=>x.State.OnEvent(EHouseBroadcastEvent.Reset));
        }
    }

    public OrderedObject Owner { get; set; }
    public void OnEvent(EHouseBroadcastEvent e)
    {
        throw new NotImplementedException();
    }

    public void OnInside(HouseEvent e)
    {
        var module = CurrentFloorModule;
        
        foreach (OrderedObject obj in module.Front)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Hide);
        }
        foreach (OrderedObject obj in module.FrontCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.Back)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.BackCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.Floor)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
    }

    public void OnOutside(HouseEvent e)
    {
        var module = CurrentFloorModule;
        
        foreach (OrderedObject obj in module.Front)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.FrontCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.Back)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.BackCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in module.Floor)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
    }

    public void OnBack(HouseEvent e)
    {
        var cmodule = CurrentFloorModule;
        var emodule = ExcludedCurrentFloorModule;
        
        foreach (OrderedObject obj in cmodule.Front)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Hide);
        }
        foreach (OrderedObject obj in cmodule.FrontCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in cmodule.Back)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Hide);
        }
        foreach (OrderedObject obj in cmodule.BackCollider)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }
        foreach (OrderedObject obj in cmodule.Floor)
        {
            obj.State.OnEvent(EHouseBroadcastEvent.Show);
        }

        foreach (var module in emodule)
        {
            foreach (OrderedObject obj in module.Front)
            {
                obj.State.OnEvent(EHouseBroadcastEvent.Hide);
            }
            foreach (OrderedObject obj in module.FrontCollider)
            {
                obj.State.OnEvent(EHouseBroadcastEvent.Hide);
            }
            foreach (OrderedObject obj in module.Back)
            {
                obj.State.OnEvent(EHouseBroadcastEvent.Hide);
            }
            foreach (OrderedObject obj in module.BackCollider)
            {
                obj.State.OnEvent(EHouseBroadcastEvent.Hide);
            }
            foreach (OrderedObject obj in module.Floor)
            {
                obj.State.OnEvent(EHouseBroadcastEvent.Hide);
            }
        }
    }

    public void Reset()
    {
        ResetModules();
    }
}