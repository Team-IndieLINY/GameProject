using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HousePass : MonoBehaviour
{
    private House _house;
    private AsyncReactiveProperty<(EHouseDirection, OrderedActor)> _property;
    [SerializeField] private EHouseDirection _enterDirection;
    [SerializeField] private EHouseDirection _exitDirection;


    public void Init(House house)
    {
        _house = house;
        _property = _house.OnPass;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedActor obj))
        {
            _property.Value = (_enterDirection, obj);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedActor obj))
        {
            _property.Value = (_exitDirection,obj);
        }
    }
}
