using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        [SerializeField] private PerceptionMeter _perceptionMeter;
        [SerializeField] private LayerMask _cullingLayer;

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


        private Vector2 _lastValidLookatDir;
        
        public void LookAtPoint(Vector2 point)
        {
            //TODO: MasterInteraction의 transform으로 변경할 것

            var dir = point - (Vector2)transform.position;

            if (Mathf.Approximately(dir.sqrMagnitude, 0f))
            {
                dir = _lastValidLookatDir;
            }
            else
            {
                _lastValidLookatDir = dir;
            }

            transform.right = dir;
        }
        public void LookAtDirection(Vector2 dir)
        {
            //TODO: MasterInteraction의 transform으로 변경할 것

            if (Mathf.Approximately(dir.sqrMagnitude, 0f))
            {
                dir = _lastValidLookatDir;
            }
            else
            {
                _lastValidLookatDir = dir;
            }

            transform.right = dir;
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

        private bool DoRaycastToTarget(Transform target)
        {
            Vector2 myPos =
                MasterInteraction.transform.position;
            Vector2 targetPos 
                = target.position;

            var hits = Physics2D.RaycastAll(
                myPos,
                targetPos - myPos,
                Distance,
                _cullingLayer.value
            );

            float minDis = Mathf.Infinity;
            Transform minTransform = null;

            foreach (var hit in hits)
            {
                var dis = Vector2.SqrMagnitude(hit.point - myPos);
                Debug.DrawLine(myPos, hit.point);
                if (dis <= minDis)
                {
                    minDis = dis;
                    minTransform = hit.transform;
                }
            }
            

            if (minTransform == target) return true;

            return false;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent<CollisionInteraction>(out var otherInteraction))
            {
                if (otherInteraction.ListeningOnly) return;

                var raycastResult = DoRaycastToTarget(otherInteraction.ContractInfo.Transform);
                var isContain = Contracts.Contains(otherInteraction);
                
                if (raycastResult == false)
                {
                    Contracts.Remove(otherInteraction);
                
                    if(_perceptionMeter)
                        _perceptionMeter.UnSchedule(otherInteraction);

                    return;
                }
                
                if(isContain) return;
                
                Contracts.Add(otherInteraction);
                
                if(_perceptionMeter)
                    _perceptionMeter.Schedule(otherInteraction, 1f);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<CollisionInteraction>(out var otherInteraction))
            {
                if (otherInteraction.ListeningOnly) return;
                Contracts.Remove(otherInteraction);
                
                if(_perceptionMeter)
                    _perceptionMeter.UnSchedule(otherInteraction);
            }
        }
    }

 }