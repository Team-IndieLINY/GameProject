using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class HousePass : MonoBehaviour
{
    [FormerlySerializedAs("EnterState")] [SerializeField] private EHousePassEvent _enterPassEvent;
    [FormerlySerializedAs("ExitState")] [SerializeField] private EHousePassEvent _exitPassEvent;

    public EHousePassEvent EnterState1 => _enterPassEvent;
    public EHousePassEvent ExitState1 => _exitPassEvent;

    private AsyncReactiveProperty<HouseEvent> _eventReactiveProperty;

    public void Init(AsyncReactiveProperty<HouseEvent> property)
    {
        _eventReactiveProperty = property;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedActor obj))
        {
            _eventReactiveProperty.Value = new HouseEvent
            {
                TriggerActor = obj,
                PassEvent = _enterPassEvent
            };
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedActor obj))
        {
            _eventReactiveProperty.Value = new HouseEvent
            {
                TriggerActor = obj,
                PassEvent = _exitPassEvent
            };
        }
    }
}
