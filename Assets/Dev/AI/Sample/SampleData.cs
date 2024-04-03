using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/FSM/Sample/PatrollData", fileName = "PatrollData")]
    public class PatrollData : ScriptableObject
    {
        [SerializeField] private float _movementSpeedFactor;

        public float MovementSpeedFactor => _movementSpeedFactor;
    }
}