using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY.Event;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IndieLINY.AI
{
    public class SightPerception : MonoBehaviour
    {
        [SerializeField] private SightPerceptionData _data;

        public SightPerceptionData Data => _data;

        public int Iteration;
        public Vector2 Forward = Vector2.right;
        
        [SerializeField] private PolygonCollider2D _collider;

        public CollisionInteraction MasterInteraction => _interaction;
        [SerializeField] private CollisionInteraction _interaction;

        [SerializeField] private PerceptionMeter _perceptionMeter;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private LayerMask _cullingLayer;
        [SerializeField] private Renderer _sightVisualizer;

        public List<CollisionInteraction> Contracts { get; private set; }

        public Vector2 Direction { get; set; } 

        private void Awake()
        {
            MeshUpdate();

            Contracts = new(2);
            _collider.isTrigger = true;
            
            Direction = transform.right;
        }
        private void OnValidate()
        {
            MeshUpdate();
        }


        private void LateUpdate()
        {
            var sqrtMag = Vector2.SqrMagnitude(_agent.desiredVelocity);

            if (sqrtMag > Mathf.Epsilon * Mathf.Epsilon)
            {
                Direction = _agent.desiredVelocity.normalized;
            }

            if (_sightVisualizer)
            {
                _sightVisualizer.transform.localScale = Vector3.one * Data.Distance * 2f;
                _sightVisualizer.transform.up = -(Quaternion.Euler(0f, 0f,Data.Fov * 0.5f) * Direction);
                var er = _sightVisualizer.transform.rotation.eulerAngles;
                er.x = 45f;
                _sightVisualizer.transform.eulerAngles = er;
                _sightVisualizer.material.SetFloat("_Fan", Data.Fov / 360f);

                er = transform.rotation.eulerAngles;
                er.x = 45f;
                transform.eulerAngles = er;
            }
        }

        public void MeshUpdate()
        {
            GenerateVisionMesh(Data.Fov, Data.Distance, Iteration, Forward);
        }

        public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation)
            => _collider.CreateMesh(useBodyPosition, useBodyRotation);


        
        public void LookAtPoint(Vector2 point)
        {
            //TODO: MasterInteraction의 transform으로 변경할 것

            var dir = point - (Vector2)transform.position;

            if (Mathf.Approximately(dir.sqrMagnitude, 0f))
            {
                dir = Direction.normalized;
            }
            else
            {
                Direction = dir.normalized;
            }

            transform.right = dir.normalized;
        }
        public void LookAtDirection(Vector2 dir)
        {
            //TODO: MasterInteraction의 transform으로 변경할 것

            if (Mathf.Approximately(dir.sqrMagnitude, 0f))
            {
                dir = Direction.normalized;
            }
            else
            {
                Direction = dir.normalized;
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
                Data.Distance,
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

                var targetTransform = (otherInteraction.ContractInfo.Interaction.Owner as MonoBehaviour)?.transform;

                if (targetTransform == false) return;
                
                var raycastResult = DoRaycastToTarget(targetTransform);
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
                var success = Contracts.Remove(otherInteraction);
                
                if(success && _perceptionMeter)
                    _perceptionMeter.UnSchedule(otherInteraction);
            }
        }
    }

 }