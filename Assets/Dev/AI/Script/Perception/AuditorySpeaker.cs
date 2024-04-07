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

        public bool IsPlaying => _coplay != null;

        private void Awake()
        {
            Contracts = new List<CollisionInteraction>(2);
        }
        public void Stop()
        {
            if (_coplay != null)
            {
                StopCoroutine(_coplay);
                _coplay = null;
            }
        }

        private float _savedRadialSpeed;
        private float _savedRadius;
        public void Play(float radialSpeed, float radius)
        {
            _savedRadialSpeed = radialSpeed;
            _savedRadius = radius;

            Stop();
            StartCoroutine(_coplay = CoPlay());
        }

        public void PlayWithNoStop(float radialSpeed, float radius)
        {
            _savedRadialSpeed = radialSpeed;
            _savedRadius = radius;
            
            if (_coplay == null)
            {
                StartCoroutine(_coplay = CoPlay());
            }
        }

        private IEnumerator CoPlay()
        {
            _collider.radius = 0f;
            
            var wait =  new WaitForEndOfFrame();

            
            while (true)
            {
                _collider.radius = Mathf.MoveTowards(_collider.radius, _savedRadius, Time.deltaTime * _savedRadialSpeed);
                
                yield return wait;
            }
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