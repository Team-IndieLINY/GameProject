using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum EHouseDirection
{
    None,
    Front,
    Back,
    Inside,
    Stair
}

public enum EHouseState
{
    Inside,
    Front,
    Back,
}

public class House : OrderedObject
{
    private List<HouseModule> _modules = new();

    private List<HousePass> _housePasses = new();

    public AsyncReactiveProperty<(EHouseDirection, OrderedActor)> OnPass;

    private EHouseDirection _currentValue;

    private Action _stateCallback;
    [SerializeField] private int _currentFloor = 1;

    public override int CurrentFloor
    {
        get => _currentFloor;
        set
        {
            if (value == _currentFloor) return;
            if (Type == EOrderedObjectType.Inner) return;

            _currentFloor = value;

            foreach (HouseModule module in _modules)
            {
                module.fronts.ForEach(x => x.CurrentFloor = value);
                module.fronts_collider.ForEach(x => x.CurrentFloor = value);
                module.backs.ForEach(x => x.CurrentFloor = value);
                module.backs_collider.ForEach(x => x.CurrentFloor = value);
                module.floors.ForEach(x => x.CurrentFloor = value);
                module.inners.ForEach(x => x.CurrentFloor = value);
            }
            
            _stateCallback?.Invoke();
        }
    }

    private void Start()
    {
        if (Type == EOrderedObjectType.Inner) CurrentFloor = 1;
        
        OnPass = new AsyncReactiveProperty<(EHouseDirection, OrderedActor)>((EHouseDirection.None, null));
        _currentValue = EHouseDirection.Front;

        OnUpdate().Forget();

        InitHousePass(transform);
        InitHouseModule(transform);

        OnFront();
    }

    private void OnValidate()
    {
        _stateCallback?.Invoke();
    }

    private void InitHousePass(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            if (child.TryGetComponent<House>(out var house))
            {
                continue;
            }

            if (child.TryGetComponent<HousePass>(out var pass))
            {
                _housePasses.Add(pass);
                pass.Init(this);
            }

            InitHousePass(child);
        }
    }

    private void InitHouseModule(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);

            if (child.TryGetComponent<House>(out var house))
            {
                continue;
            }

            if (child.TryGetComponent<HouseModule>(out var module))
            {
                _modules.Add(module);
            }

            InitHouseModule(child);
        }
    }

    private async UniTaskVoid OnUpdate()
    {
        while (true)
        {
            var tuple = await OnPass.WaitAsync();

            ModuleUpdate(tuple);
        }
    }

    public void ModuleUpdate((EHouseDirection, OrderedActor) tuple)
    {
        var dir = tuple.Item1;
        var orderedObject = tuple.Item2;

        if (orderedObject == null) return;

        if (_currentValue == EHouseDirection.Inside && dir == EHouseDirection.Back)
        {
            return;
        }
        else
        {
            _currentValue = dir;
        }

        switch (dir)
        {
            case EHouseDirection.None:
                break;
            case EHouseDirection.Front:
                if (CurrentFloor != 1) return;
                if (orderedObject.IsPlayer)
                {
                    _stateCallback = OnFront;
                }

                break;
            case EHouseDirection.Back:
                if (CurrentFloor != 1) return;
                if (orderedObject.IsPlayer)
                {
                    _stateCallback = OnBack;
                }

                break;
            case EHouseDirection.Inside:
                if (orderedObject.IsPlayer)
                {
                    _stateCallback = OnInsideOnFloor;
                }

                break;
            case EHouseDirection.Stair:
                if (orderedObject.IsPlayer)
                {
                    _stateCallback = OnStair;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _stateCallback?.Invoke();
    }

    private void SetEnable(bool value, List<OrderedObject> arr)
    {
        foreach (var item in arr)
        {
            item.IsEnabled = value;
        }
    }

    private void SetAlpha(HouseModule module, float value, bool exclusive1F)
    {
        SetAlpha(module.fronts, value, exclusive1F);
        SetAlpha(module.backs, value, exclusive1F);
        SetAlpha(module.floors, value, exclusive1F);

        if (module.floor != 1 && exclusive1F) return;
        SetAlpha(module.fronts_collider, value, exclusive1F);
        SetAlpha(module.backs_collider, value, exclusive1F);
    }

    private void SetAlpha(List<OrderedObject> objects, float value, bool exclusive1F)
    {
        foreach (var item in objects)
        {
            var c = item.Color;
            c.a = value;
            item.Color = c;
        }
    }

    private void OnFront()
    {
        foreach (var module in _modules)
        {
            SetEnable(true, module.fronts);
            SetEnable(true, module.backs);
            SetEnable(true, module.floors);
            SetEnable(true, module.fronts_collider);
            SetEnable(true, module.backs_collider);

            SetAlpha(module, 1f, false);

            module.fronts_collider.ForEach(x => x.CollisionEnabled = module.floor == CurrentFloor);
            module.backs_collider.ForEach(x => x.CollisionEnabled = module.floor == CurrentFloor);
        }
    }

    private void OnBack()
    {
        foreach (var module in _modules)
        {
            SetEnable(false, module.fronts);
            SetEnable(false, module.backs);
            SetEnable(false, module.floors);
            SetEnable(true, module.fronts_collider);
            SetEnable(true, module.backs_collider);

            SetAlpha(module.backs, 1f, false);

            module.fronts.ForEach(x =>
            {
                if (module.floor != CurrentFloor) x.IsEnabled = false;
            });
            module.backs.ForEach(x =>
            {
                if (module.floor != CurrentFloor) x.IsEnabled = false;
            });
            module.floors.ForEach(x =>
            {
                if (module.floor != CurrentFloor) x.IsEnabled = false;
            });
            module.fronts_collider.ForEach(x =>
            {
                if (module.floor != CurrentFloor) x.IsEnabled = false;
            });
            module.backs_collider.ForEach(x =>
            {
                if (module.floor != CurrentFloor) x.IsEnabled = false;
            });
        }
    }

    private void OnStair()
    {
        foreach (var module in _modules)
        {
            SetEnable(false, module.fronts);
            SetEnable(true, module.backs);
            SetEnable(true, module.floors);
            SetEnable(false, module.fronts_collider);
            SetEnable(true, module.backs_collider);


            SetAlpha(module, 1f, false);
        }

        foreach (var module in _modules)
        {
            foreach (var col in module.backs_collider)
            {
                col.GetComponent<Collider2D>().enabled = false;
            }

            foreach (var col in module.fronts_collider)
            {
                col.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    private void OnInsideOnFloor()
    {
        //_backPass.GetComponent<Collider2D>().enabled = false;
        foreach (var module in _modules)
        {
            SetAlpha(module, 1f, false);

            module.fronts.ForEach(x =>
            {
                x.IsEnabled = false;
                x.CollisionEnabled = true;
            });
            module.backs.ForEach(x =>
            {
                x.IsEnabled = module.floor == CurrentFloor;
                x.CollisionEnabled = module.floor == CurrentFloor;
            });
            module.floors.ForEach(x =>
            {
                x.IsEnabled = module.floor == CurrentFloor;
                x.CollisionEnabled = module.floor == CurrentFloor;
            });
            module.fronts_collider.ForEach(x =>
            {
                x.IsEnabled = module.floor == CurrentFloor;
                x.CollisionEnabled = module.floor == CurrentFloor;
            });
            module.backs_collider.ForEach(x =>
            {
                x.IsEnabled = module.floor == CurrentFloor;
                x.CollisionEnabled = module.floor == CurrentFloor;
            });
            module.inners.ForEach(x =>
            {
                x.IsEnabled = module.floor == CurrentFloor;
                x.CollisionEnabled = module.floor == CurrentFloor;
            });
        }
    }

    public override Color Color
    {
        get => Color.white;
        set
        {
            foreach (var module in _modules)
            {
                SetAlpha(module.fronts, value.a, false);
                SetAlpha(module.backs, value.a, false);
                SetAlpha(module.floors, value.a, false);
                SetAlpha(module.fronts_collider, value.a, false);
                SetAlpha(module.backs_collider, value.a, false);
            }
        }
    }

    public override bool IsEnabled
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public override Renderer Renderer => GetComponentInChildren<TilemapRenderer>();


    private bool _colliderEnabled = true;

    public override bool CollisionEnabled
    {
        get => _colliderEnabled;
        set
        {
            _colliderEnabled = value;

            foreach (var module in _modules)
            {
                module.fronts.ForEach(x => x.CollisionEnabled = value);
                module.backs.ForEach(x => x.CollisionEnabled = value);
                module.floors.ForEach(x => x.CollisionEnabled = value);
                module.fronts_collider.ForEach(x => x.CollisionEnabled = value);
                module.backs_collider.ForEach(x => x.CollisionEnabled = value);
                module.inners.ForEach(x => x.CollisionEnabled = value);
            }

            foreach (var pass in _housePasses)
            {
                pass.gameObject.SetActive(value);
            }
        }
    }
}