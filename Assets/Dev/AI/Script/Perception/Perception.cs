using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieLINY.AI
{
    public class Perception : MonoBehaviour
    {
        public float Fov;
        public float Distance;
        public int Iteration;
        public Vector2 Forward = Vector2.right;
        
        [SerializeField] private PolygonCollider2D _collider;

        public ICollisionInteraction Interaction => _interaction;
        [SerializeField] private CollisionInteractionMono _interaction;

        private void Awake()
        {
            MeshUpdate();
        }

        private void Start()
        {
            _collider.isTrigger = true;

            Interaction.OnContractActor += x =>
            {
                Debug.Log("Asd");
            };
        }

        private void OnValidate()
        {
            MeshUpdate();
        }

        public void MeshUpdate()
        {
            GenerateVisionMesh(Fov, Distance, Iteration, Forward);
        }

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
    }

 }