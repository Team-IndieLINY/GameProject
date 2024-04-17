using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/AI/SightPerception", fileName = "SightPerception")]
    public class SightPerceptionData : ScriptableObject
    {
        [Header("시야 범위 (degree)")]
        [SerializeField] private float _fov;
        [Header("시야 범위 (meter)")]
        [SerializeField] private float _distance;
        public float Fov => _fov;
        public float Distance => _distance;
    }

}