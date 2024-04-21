using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;


public class House : OrderedObject, IOrderedDescriptor
{
    [Range(1, 10)] [SerializeField] private int _currentFloor = 1;

    public int MaxFloor { get; private set; } = 1;

    private List<HouseModule> _modules = new(10);

    public AsyncReactiveProperty<HouseEvent> EventReactiveProperty { get; private set; } = new(new HouseEvent());

    private void Awake()
    {
        Descriptor = this;
        Owner = this;

        BindModules();

        OnUpdate().Forget();
    }

    private void Start()
    {
        ResetModules();
        OnOutside();
    }

    public override void Init()
    {
        // do nothing here after
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
        bool first = true;

        while (queue.Any())
        {
            var queuedTransform = queue.Dequeue();

            if (first == false && queuedTransform.TryGetComponent<House>(out var house))
            {
                continue;
            }
            
            first = false;

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

        foreach (HouseModule module in _modules)
        {
            if (module.FloorNumer >= MaxFloor)
            {
                MaxFloor = module.FloorNumer;
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
            module.ForEach(x => x.Descriptor.OnEvent(EHouseBroadcastEvent.Reset));
        }
        
        foreach (HousePass housePass in CurrentFloorModule.Pass)
        {
            housePass.gameObject.SetActive(true);
        }
        foreach (var modules in ExcludedCurrentFloorModule)
        {
            foreach (var pass in modules.Pass)
            {
                pass.gameObject.SetActive(false);
            }
        }
    }

    public OrderedObject Owner { get; set; }

    public void OnEvent(EHouseBroadcastEvent e)
    {
        ResetModules();
        
        _modules.ForEach(x => x.ForEach(y => y.Descriptor.OnEvent(e)));
    }

    private async UniTaskVoid OnUpdate()
    {
        while (true)
        {
            var e = await EventReactiveProperty.WaitAsync();
            if (e.TriggerActor != null && e.TriggerActor.IsPlayer == false) return;

            ResetModules();
            Action callback;

            switch (e.PassEvent)
            {
                case EHousePassEvent.None:
                    return;
                case EHousePassEvent.Inside:
                    callback = OnInside;
                    break;
                case EHousePassEvent.Outside:
                    callback = OnOutside;
                    break;
                case EHousePassEvent.Back:
                    callback = OnBack;
                    break;
                default:
                    return;
            }

            callback?.Invoke();
        }
    }

    public void OnInside()
    {
        var module = CurrentFloorModule;

        foreach (OrderedObject obj in module.Front)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
        }

        foreach (OrderedObject obj in module.FrontCollider)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
        }

        foreach (OrderedObject obj in module.Back)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
        }

        foreach (OrderedObject obj in module.BackCollider)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
        }

        foreach (OrderedObject obj in module.Inside)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
        }

        foreach (OrderedObject obj in module.Floor)
        {
            obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
        }

        foreach (HouseModule emodule in ExcludedCurrentFloorModule)
        {
            foreach (OrderedObject obj in emodule.Front)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in emodule.FrontCollider)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in emodule.Back)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in emodule.BackCollider)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in emodule.Inside)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in emodule.Floor)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }
        }
    }

    public void OnOutside()
    {
        var module = _modules;

        foreach (HouseModule cmodule in module)
        {
            foreach (OrderedObject obj in cmodule.Front)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
            }

            foreach (OrderedObject obj in cmodule.FrontCollider)
            {
                if (cmodule.FloorNumer == 1)
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
                }
                else
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                }
            }

            foreach (OrderedObject obj in cmodule.Back)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
            }

            foreach (OrderedObject obj in cmodule.BackCollider)
            {
                if (cmodule.FloorNumer == 1)
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
                }
                else
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                }
            }

            foreach (OrderedObject obj in cmodule.Inside)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in cmodule.Floor)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
            }
        }
    }

    public void OnBack()
    {
        var cmodule = _modules;

        foreach (HouseModule module in cmodule)
        {
            foreach (OrderedObject obj in module.Front)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in module.FrontCollider)
            {
                if (module.FloorNumer == 1 || module.FloorNumer == CurrentFloor)
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
                }
                else
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                }
            }

            foreach (OrderedObject obj in module.Back)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.SetTransparently);
            }

            foreach (OrderedObject obj in module.BackCollider)
            {
                if (module.FloorNumer == 1 || module.FloorNumer == CurrentFloor)
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableCollider);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.EnableVisible);
                }
                else
                {
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
                    obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableCollider);
                }
            }

            foreach (OrderedObject obj in module.Inside)
            {
                obj.Descriptor.OnEvent(EHouseBroadcastEvent.DisableVisible);
            }

            foreach (OrderedObject obj in module.Floor)
            {
                obj.Descriptor.OnEvent(
                    module.FloorNumer == 1 ? EHouseBroadcastEvent.EnableVisible : EHouseBroadcastEvent.DisableVisible
                );
            }
        }
    }

    public void Reset()
    {
        ResetModules();
    }
}