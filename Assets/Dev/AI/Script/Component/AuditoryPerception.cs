using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieLINY.AI
{
    public class AuditoryPerception : MonoBehaviour
    {
        public CollisionInteraction MasterInteraction => _interaction;
        [SerializeField] private CollisionInteraction _interaction;
        [SerializeField] private AuditoryPerceptionData _data;
        
        private CircleCollider2D _collider;

        public AuditoryPerceptionData Data => _data;

        public List<CollisionInteraction> Contracts { get; private set; }

        [SerializeField] private List<AuditorySpeaker> _ignoreAuditorySpeakers;
        public List<AuditorySpeaker> IgnoreAuditorySpeakers => _ignoreAuditorySpeakers;

        [SerializeField] private PerceptionMeter _perceptionMeter;

        private void Awake()
        {
            Contracts = new List<CollisionInteraction>(2);
            _collider = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            _collider.radius = Data.ListeningRadius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<AuditorySpeaker>(out var speaker))
            {
                if (IgnoreAuditorySpeakers.Contains(speaker)) return;
                if (Contracts.Contains(speaker.MasterInteraction)) return;

                Contracts.Add(speaker.MasterInteraction);
                
                if (_perceptionMeter)
                    _perceptionMeter.Schedule(speaker.MasterInteraction, 1f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<AuditorySpeaker>(out var speaker))
            {
                bool success = Contracts.Remove(speaker.MasterInteraction);
                
                if (success && _perceptionMeter)
                    _perceptionMeter.UnSchedule(speaker.MasterInteraction);
            }
        }
    }
}