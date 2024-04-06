using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieLINY.AI
{
    public class SightPerception : MonoBehaviour
    {
        public float Fov;
        public float Distance;
        public int Iteration;
        public Vector2 Forward = Vector2.right;
        
        [SerializeField] private PolygonCollider2D _collider;

        public CollisionInteraction MasterInteraction => _interaction;
        [SerializeField] private CollisionInteraction _interaction;

        public List<CollisionInteraction> Contracts { get; private set; }

        private void Awake()
        {
            MeshUpdate();

            Contracts = new(2);
            _collider.isTrigger = true;
        }
        private void OnValidate()
        {
            MeshUpdate();
        }

        public void MeshUpdate()
        {
            GenerateVisionMesh(Fov, Distance, Iteration, Forward);
        }

        public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation)
            => _collider.CreateMesh(useBodyPosition, useBodyRotation);

        private void GenerateVisionMesh(float fov, float distance, int iteration, Vector2 forward)
        {
            if (iteration < 10)
            {
                iteration = 10;
            }

            if (iteration > 1000)
            {
                iteration = 1000;
            }

            if (fov > 360f)
            {
                fov = 360f;
            }
            
            Vector2[] arr = new Vector2[iteration + 1 + 1];

            float stepAngle = fov / iteration;
            float angle = 0f;
            Vector2 standDir = Quaternion.AngleAxis(fov * -0.5f, Vector3.forward) * forward;
            

            arr[0] = Vector2.zero;
            for (int i = 0; i < iteration + 1; i++)
            {
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                angle += stepAngle;
                arr[i + 1] = (q * standDir).normalized * distance;
            }


            _collider.SetPath(0, arr);
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