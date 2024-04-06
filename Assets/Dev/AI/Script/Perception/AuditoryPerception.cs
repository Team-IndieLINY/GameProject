using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;

namespace IndieLINY.AI
{
    public class AuditoryPerception : MonoBehaviour
    {
        public CollisionInteraction MasterInteraction => _interaction;
        [SerializeField] private CollisionInteraction _interaction;

        public List<CollisionInteraction> Contracts { get; private set; }

        [SerializeField] private List<AuditorySpeaker> _ignoreAuditorySpeakers;
        public List<AuditorySpeaker> IgnoreAuditorySpeakers => _ignoreAuditorySpeakers;
        
        private void Awake()
        {
            Contracts = new List<CollisionInteraction>(2);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<AuditorySpeaker>(out var speaker))
            {
                if (IgnoreAuditorySpeakers.Contains(speaker)) return;
                if (Contracts.Contains(speaker.MasterInteraction)) return;
                
                Contracts.Add(speaker.MasterInteraction);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<AuditorySpeaker>(out var speaker))
            {
                Contracts.Remove(speaker.MasterInteraction);
            }
        }
    }

}