using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/AI/AuditoryPerception", fileName = "AuditoryPerception")]
    public class AuditoryPerceptionData : ScriptableObject
    {
        [Header("감청 범위 (meter)")]
        [SerializeField] private float _listeningRadius;

        public float ListeningRadius => _listeningRadius;
    }
}
