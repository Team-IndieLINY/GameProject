using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;

namespace IndieLINY.AI
{
    public class AuditorySpeaker : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _collider;

        public List<CollisionInteraction> Contracts { get; private set; }
        
        
        public CollisionInteraction MasterInteraction => _interaction;
        [SerializeField] private CollisionInteraction _interaction;

        private IEnumerator _coplay;
        
        public void Play(float duration, float radius)
        {
            Contracts = new List<CollisionInteraction>(2);

            if (_coplay != null)
            {
                StopCoroutine(_coplay);
            }

            StartCoroutine(_coplay = CoPlay(duration, radius));
        }

        private IEnumerator CoPlay(float duration, float radius)
        {
            _collider.radius = radius;
            yield return new WaitForSeconds(duration);
            _collider.radius = 0f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<CollisionInteraction>(out var otherInteraction))
            {
                if (otherInteraction.ListeningOnly) return;
                if (Contracts.Contains(otherInteraction)) return;
                
                Contracts.Add(otherInteraction);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<CollisionInteraction>(out var otherInteraction))
            {
                if (otherInteraction.ListeningOnly) return;
                Contracts.Remove(otherInteraction);
            }
        }
    }

}