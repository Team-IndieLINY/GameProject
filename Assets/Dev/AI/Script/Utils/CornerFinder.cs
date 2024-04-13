using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace IndieLINY.AI
{
    public class CornerFinder : MonoBehaviour
    {
        [SerializeField] private PathGenerator _pathGenerator;

        [SerializeField] private int _step;
        [SerializeField] private Vector2 _size;
        
        [FormerlySerializedAs("_randomWeightRange")]
        
        [Range(0f, 1f)]
        [SerializeField] private float _randomWeightProbability;
        [Range(0f, 1f)]
        [SerializeField] private float _randomWeight;
        
        private BoundNode _curCornerNode;
        private NavMeshAgent _agent;

        public PathGenerator Generator
        {
            get => _pathGenerator;
            set => _pathGenerator = value;
        }
        public int Step 
        {
            get => _step;
            set => _step = value;
        }
        public Vector2 Size 
        {
            get => _size;
            set => _size = value;
        }

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