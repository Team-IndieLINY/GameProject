using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.Sample
{
    
    [CreateAssetMenu(menuName = "IndieLINY/FSM/Sample/DetectionData", fileName = "DetectionData")]
    public class DetectionData : ScriptableObject
    {
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _fov;

        public float MaxDistance => _maxDistance;
        public float FOV => _fov;
    }

}