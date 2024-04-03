using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.Sample
{
    [CreateAssetMenu(menuName = "IndieLINY/FSM/Sample/TraceData", fileName = "TraceData")]
    public class TraceData : ScriptableObject
    {
        [SerializeField] private float _movementSpeedFactor;

        public float MovementSpeedFactor => _movementSpeedFactor;
    }    
}
