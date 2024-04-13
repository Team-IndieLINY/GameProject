using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY.AI.Core;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/AI/Unit")]
    [UnitTitle("CornerConjector")]
    public class CornerConjectorUnit : UBaseEnterUpdateExit
    {
        [DoNotSerialize] private ControlOutput _cUpdate;
        [DoNotSerialize] private ValueInput _vAgent;
        [DoNotSerialize] private ValueInput _vLastSight;
        [DoNotSerialize] private ValueInput _vCornerDetectionBoxSize;
        [DoNotSerialize] private ValueInput _vRandomWeightProbability;
        [DoNotSerialize] private ValueInput _vRandomWeight;

        protected override ControlOutput OnEnter(Flow flow)
        {
            var behaviour =
                BehaviourBinder
                    .GetBinder(flow)
                    .GetBehaviour<CornerConjector>();

            behaviour.Generator = PathGenerator.Test_Instance;
            behaviour.RandomWeight = flow.GetValue<float>(_vRandomWeight);
            behaviour.RandomWeightProbability = flow.GetValue<float>(_vRandomWeightProbability);
            behaviour.Size = flow.GetValue<Vector2>(_vCornerDetectionBoxSize);
            
            behaviour.OnEnterState(
                    flow.GetValue<Vector2>(_vLastSight),
                    flow.GetValue<NavMeshAgent>(_vAgent)
                );
            
            return null;
        }

        protected override ControlOutput OnUpdate(Flow flow)
        {
            var behaviour =
                BehaviourBinder
                    .GetBinder(flow)
                    .GetBehaviour<CornerConjector>();

            behaviour.Generator = PathGenerator.Test_Instance;
            behaviour.RandomWeight = flow.GetValue<float>(_vRandomWeight);
            behaviour.RandomWeightProbability = flow.GetValue<float>(_vRandomWeightProbability);
            behaviour.Size = flow.GetValue<Vector2>(_vCornerDetectionBoxSize);
            
            BehaviourBinder
                .GetBinder(flow)
                .GetBehaviour<CornerConjector>()
                .OnUpdate();

            return _cUpdate;
        }

        protected override ControlOutput OnExit(Flow flow)
        {
            BehaviourBinder
                .GetBinder(flow)
                .GetBehaviour<CornerConjector>()
                .OnExitState();

            return null;
        }

        protected override void OnDefinition()
        {
            _vAgent = ValueInput<NavMeshAgent>("Nav Agent");
            _vLastSight = ValueInput<Vector2>("LastSight");
            _vCornerDetectionBoxSize = ValueInput<Vector2>("CornerDetectionBoxSize");
            _vRandomWeight = ValueInput<float>("RandomWeight");
            _vRandomWeightProbability = ValueInput<float>("RandomWeightProbability");
            _cUpdate = ControlOutput("End of update");
        }
    }
    
    
    public class CornerConjector : StateBehaviour
    {
        private BoundNode _curCornerNode;
        private NavMeshAgent _agent;

        public PathGenerator Generator { get; set; }
        public Vector2 Size { get; set; }

        private float _randomWeightProbability;
        public float RandomWeightProbability
        {
            get => _randomWeightProbability;
            set
            {
                _randomWeightProbability = value;

                if (_randomWeightProbability > 1f)
                    _randomWeightProbability = 1f;

                if (_randomWeightProbability < 0f)
                    _randomWeightProbability = 0f;

            }
        }

        private float _randomWeight;
        public float RandomWeight
        {
            get => _randomWeight;
            set => _randomWeight = value;
        }
        public void OnEnterState(Vector2 lastSightPoint, NavMeshAgent agent)
        {
            _agent = agent;
            
            _curCornerNode = GetCornerNode(_agent.transform.position, Size);
            
            if (_curCornerNode != null)
            {
                _curCornerNode = GetNextCornerNode(_curCornerNode, (lastSightPoint-(Vector2)_curCornerNode.Bound.Pos));


                if (_curCornerNode != null)
                {
                    _agent.SetDestination(_curCornerNode.Bound.Pos);
                }
            }

        }

        [CanBeNull]
        private Collider2D GetNeighborCollider([NotNull] BoundNode node)
        {
            foreach (var neighbor in node.Neighbor)
            {
                if (neighbor.Bound.Collision)
                {
                    return neighbor.Bound.ContactCollider;
                }
            }

            return null;
        }
        
        public void OnUpdate()
        {
            if (_curCornerNode == null)
            {
                _curCornerNode = GetCornerNode(_agent.transform.position, Size);

                if (_curCornerNode != null)
                    _agent.SetDestination(_curCornerNode.Bound.Pos);
            }

            if (_agent.remainingDistance <= 1f && _curCornerNode != null)
            {
                _curCornerNode = GetNextCornerNode(_curCornerNode, _agent.desiredVelocity);
                
                if (_curCornerNode != null)
                {
                    _agent.SetDestination(_curCornerNode.Bound.Pos);
                }
            }
        }

        public void OnExitState()
        {
            _curCornerNode = null;
        }
        public BoundNode GetCornerNode(Vector2 point, Vector2 size)
        {
            var node = GetCornerNode(point, size, point, new List<BoundNode>(0));

            return node;
        }

        [CanBeNull]
        public BoundNode GetNextCornerNode([NotNull] BoundNode node, Vector2 dir)
        {
            dir = dir.normalized;

            float dot = Mathf.Infinity;
            BoundNode result = null;
            foreach (BoundNode neighbor in node.CornerNeighbor)
            {
                Debug.Assert(neighbor != node);
                var toNeighborDir = (neighbor.Bound.Pos - node.Bound.Pos).normalized;

                float tempDot = 1f - Vector2.Dot(toNeighborDir, dir);
                float weight = 0f;

                if (Random.Range(0f, 1f) > RandomWeightProbability)
                {
                    weight += RandomWeight;
                }
                
                tempDot += weight;
                
                if (dot > tempDot)
                {
                    dot = tempDot;
                    result = neighbor;
                }
            }

            return result;

        }
        
        private BoundNode GetCornerNode(Vector2 originPoint, Vector2 boundingSize, Vector2 point,
            List<BoundNode> excludeNodes)
        {
            HashSet<BoundNode> hashSet = new HashSet<BoundNode>();
            Queue<BoundNode> currentNodes = new Queue<BoundNode>(100);

            var pointNode = Generator.GetNodeFromPoint(point);

            if (pointNode == null) return null;
            if (pointNode._Bound._Collision) return null;

            currentNodes.Enqueue(pointNode);

            while (currentNodes.Count > 0)
            {
                int length = currentNodes.Count;
                BoundNode minNode = null;
                float minDis = Mathf.Infinity;
                for (int i = 0; i < length; i++)
                {
                    var node = currentNodes.Dequeue();
                    if (node._Bound._Collision) continue;
                    if (hashSet.Contains(node)) continue;
                    if (Generator.Intersects(
                            node._Bound.GetBounds(),
                            new Bounds(originPoint, boundingSize))
                        == false)
                        continue;

                    if (node._Bound._IsCorner && excludeNodes.Contains(node) == false)
                    {
                        float dis = Vector2.SqrMagnitude(point - node._Bound._Pos);
                        if (minDis >= dis)
                        {
                            minDis = dis;
                            minNode = node;
                        }
                    }

                    hashSet.Add(node);
                    foreach (var neighbor in node._Neighbor)
                    {
                        if (neighbor._Bound._Collision) continue;
                        if (Generator.Intersects(
                                neighbor._Bound.GetBounds(),
                                new Bounds(originPoint, boundingSize))
                            == false)
                            continue;
                        currentNodes.Enqueue(neighbor);
                    }
                }

                if (minNode != null)
                {
                    return minNode;
                }
            }

            return null;
        }
    }
}