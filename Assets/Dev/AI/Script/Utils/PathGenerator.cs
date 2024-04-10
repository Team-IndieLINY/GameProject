using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.Common.TreeGrouper;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieLINY.AI
{
    [System.Serializable]
    public struct BoundPoint
    {
        public Vector2 Pos;
        public Vector2 Size;

        // only leaf node having value of ContactCollider
        public Collider2D ContactCollider;
        public bool Collision;
        public bool IsCorner;

        public Bounds GetBounds()
        {
            return new Bounds(Pos, Size * 2f);
        }
    }

    public class BoundNode
    {
        public BoundPoint Bound;
        public BoundNode Parent;
        public BoundNode[] Childs = new BoundNode[4];
        public List<BoundNode> Neighbor = new List<BoundNode>(4);
        public int Depth = 1;

        public bool IsLeaf
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Childs.Length; i++)
                {
                    if (Childs[i] != null) count++;
                }

                return count == 0;
            }
        }

        public void GetLeafCondition(ref List<BoundNode> container, Func<BoundNode, bool> predicate)
        {
            Debug.Assert(container != null);
            Debug.Assert(predicate != null);

            if (IsLeaf)
            {
                if (predicate(this))
                {
                    container.Add(this);
                }
            }
            else
            {
                foreach (BoundNode child in Childs)
                {
                    if (child == null) continue;
                    child.GetLeafCondition(ref container, predicate);
                }
            }
        }
    }

    public class PathGenerator : MonoBehaviour
    {
        [SerializeField] private LayerMask _wallLayer;

        [SerializeField] private Vector2 _size;

        [SerializeField] private int _maxDepth = 2;
        [SerializeField] private int _maxDefaultDepth = 2;
        [SerializeField] private int _outterCornerSampleingCount;
        [SerializeField] private int _innerCornerSampleingCount;


        [SerializeField] private bool DEBUG_BoudingBox;
        [SerializeField] private bool DEBUG_NeighvorBoudingBox;
        [SerializeField] private bool DEBUG_CornerBoudingBox;
        [SerializeField] private bool DEBUG_NeighborLine;

        private Vector2 _leftUpDir;
        private Vector2 _rightUpDir;
        private Vector2 _leftDownDir;
        private Vector2 _rightDownDir;

        private Vector2 LeftUpDir => transform.TransformDirection(_leftUpDir);
        private Vector2 RightUpDir => transform.TransformDirection(_rightUpDir);
        private Vector2 LeftDownDir => transform.TransformDirection(_leftDownDir);
        private Vector2 RightDownDir => transform.TransformDirection(_rightDownDir);

        private BoundNode _rootNode;

        private void Awake()
        {
            Generate();
            GenerateNeighbor(_rootNode);
            GenerateCorner();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                Generate();
                GenerateNeighbor(_rootNode);
                GenerateCorner();
            }
        }

        private void Generate()
        {
            _leftUpDir = new Vector2(-1f, 1f).normalized;
            _rightUpDir = new Vector2(1f, 1f).normalized;
            _leftDownDir = new Vector2(-1f, -1f).normalized;
            _rightDownDir = new Vector2(1f, -1f).normalized;

            List<BoundPoint> points = new List<BoundPoint>(100);

            _rootNode = new BoundNode()
            {
                Bound = new BoundPoint()
                {
                    Pos = transform.position,
                    Size = _size,
                    ContactCollider = null
                },
                Parent = null,
            };

            GeneratePoints(transform.position, _size, 1, _rootNode);
        }

        private void GenerateCorner()
        {
            var list = new List<BoundNode>(100);
            _rootNode.GetLeafCondition(ref list, x =>
            {
                if (x.Bound.Collision) return false;

                var count = x.Neighbor.Count(y => y.Bound.Collision);
                if (count == 0) return false;

                return count < _outterCornerSampleingCount ||
                       count >= _innerCornerSampleingCount;
            });

            foreach (BoundNode node in list)
            {
                node.Bound.IsCorner = true;
            }
        }

        private void GeneratePoints(Vector2 parentPoint, Vector2 parentSize, int depth, BoundNode parentNode)
        {
            var overlapCollider =
                Physics2D.OverlapBox(parentPoint, parentSize * 2f, transform.eulerAngles.z, _wallLayer.value);
            
            parentNode.Bound.Collision = overlapCollider;

            bool pass = false;

            if (overlapCollider && depth <= _maxDepth)
            {
                pass = true;
            }

            if (overlapCollider == false && depth <= _maxDepth)
            {
                if (depth <= _maxDefaultDepth)
                {
                    pass = true;
                }
            }

            if (pass == false)
            {
                parentNode.Bound.Collision = overlapCollider;
                parentNode.Bound.ContactCollider = overlapCollider;

                return;
            }

            depth += 1;
            float magnitude = parentSize.magnitude * 0.5f;
            Vector2 size = parentSize * 0.5f;

            parentNode.Childs[0] = new BoundNode()
            {
                Bound = new BoundPoint()
                {
                    Pos = parentPoint + LeftUpDir * magnitude,
                    Size = size,
                },
                Parent = parentNode,
                Depth = depth
            };
            parentNode.Childs[1] = new BoundNode()
            {
                Bound = new BoundPoint()
                {
                    Pos = parentPoint + RightUpDir * magnitude,
                    Size = size
                },
                Parent = parentNode,
                Depth = depth
            };
            parentNode.Childs[2] = new BoundNode()
            {
                Bound = new BoundPoint()
                {
                    Pos = parentPoint + LeftDownDir * magnitude,
                    Size = size
                },
                Parent = parentNode,
                Depth = depth
            };
            parentNode.Childs[3] = new BoundNode()
            {
                Bound = new BoundPoint()
                {
                    Pos = parentPoint + RightDownDir * magnitude,
                    Size = size
                },
                Parent = parentNode,
                Depth = depth
            };

            GeneratePoints(parentNode.Childs[0].Bound.Pos, size, depth, parentNode.Childs[0]);
            GeneratePoints(parentNode.Childs[1].Bound.Pos, size, depth, parentNode.Childs[1]);
            GeneratePoints(parentNode.Childs[2].Bound.Pos, size, depth, parentNode.Childs[2]);
            GeneratePoints(parentNode.Childs[3].Bound.Pos, size, depth, parentNode.Childs[3]);
        }

        private void GenerateNeighbor(BoundNode parentNode)
        {
            List<BoundNode> leafs = new List<BoundNode>(100);

            parentNode.GetLeafCondition(ref leafs, x => x.IsLeaf);

            foreach (BoundNode node in leafs)
            {
                SetNeighbor(parentNode, node);
            }
        }

        public bool Intersects(Bounds a, Bounds b)
        {
            a.center = transform.InverseTransformPoint(a.center);
            b.center = transform.InverseTransformPoint(b.center);

            return a.Intersects(b);
        }

        private void SetNeighbor(BoundNode parentNode, BoundNode targetNode)
        {
            Debug.Assert(targetNode.IsLeaf);
            if (parentNode.IsLeaf)
            {
                Bounds a = parentNode.Bound.GetBounds();
                Bounds b = targetNode.Bound.GetBounds();

                a.center = transform.InverseTransformPoint(a.center);
                a.size += (Vector3)Vector2.one * 0.01f;
                b.center = transform.InverseTransformPoint(b.center);

                var isIntersacts = a.Intersects(b);

                if (isIntersacts)
                {
                    targetNode.Neighbor.Add(parentNode);
                }
            }
            else
            {
                foreach (var child in parentNode.Childs)
                {
                    SetNeighbor(child, targetNode);
                }
            }
        }

        public void GetNodeFromBox(Vector2 point, Vector2 size, List<BoundNode> nodes, [CanBeNull] Func<BoundNode, bool> condition = null)
        {
            Queue<BoundNode> currentTargets = new Queue<BoundNode>(10);

            currentTargets.Enqueue(_rootNode);
            Bounds bounds = new Bounds(point, size);
            bool hasCondition = condition != null;
            
            while (currentTargets.Any())
            {
                int length = currentTargets.Count;
                for(int i=0; i<length;i++)
                {
                    var target = currentTargets.Dequeue();

                    if (hasCondition && condition.Invoke(target) == false) continue;
                    
                    if (Intersects(target.Bound.GetBounds(), bounds))
                    {
                        if (target.IsLeaf)
                        {
                            nodes.Add(target);
                        }
                        else
                        {
                            foreach (BoundNode child in target.Childs)
                            {
                                currentTargets.Enqueue(child);
                            }
                        }
                    }
                }
            }
        }

        public BoundNode GetNodeFromPoint(Vector2 point)
        {
            BoundNode currentNode = _rootNode;
            
            var m = transform.worldToLocalMatrix;
            m.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            
            while (true)
            {
                bool isContinue = false;
                if (currentNode.IsLeaf) return currentNode;
                
                foreach (var node in currentNode.Childs)
                {
                    var bounds = node.Bound.GetBounds();
                    bounds.center = m.MultiplyPoint(bounds.center);

                    if (bounds.Contains(m.MultiplyPoint(point)))
                    {
                        currentNode = node;
                        isContinue = true;
                        break;
                    }
                }

                if (isContinue == false)
                {
                    return null;
                }
                
            }
        }

        public BoundNode Root => _rootNode;
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, _size * 2);

            List<BoundNode> noCollisionLeafs = new List<BoundNode>(100);
            List<BoundNode> neighborLeafs = new List<BoundNode>(100);
            List<BoundNode> cornerLeafs = new List<BoundNode>(100);
            if (_rootNode == null) return;

            _rootNode.GetLeafCondition(ref noCollisionLeafs, x => x.Bound.Collision == false);
            _rootNode.GetLeafCondition(ref neighborLeafs, x => x.Bound.Collision == false && x.Neighbor.Any(y => y.Bound.Collision));
            _rootNode.GetLeafCondition(ref cornerLeafs, x => x.Bound is { Collision: false, IsCorner: true });


            foreach (BoundNode node in noCollisionLeafs)
            {
                if (DEBUG_NeighborLine == false) break;

 
                foreach (BoundNode neighbor in node.Neighbor)
                {

                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine( node.Bound.Pos, neighbor.Bound.Pos);
                }
            }
            
            foreach (var node in noCollisionLeafs)
            {
                if (DEBUG_BoudingBox == false) break;

                var m = Matrix4x4.TRS(
                    node.Bound.Pos,
                    transform.rotation,
                    Vector3.one
                );

                Gizmos.matrix = m;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Vector3.zero, 0.1f);
                Gizmos.DrawWireCube(Vector3.zero, node.Bound.Size * 2);
            }

            foreach (var node in neighborLeafs)
            {
                if (DEBUG_NeighvorBoudingBox == false) break;

                var m = Matrix4x4.TRS(
                    node.Bound.Pos,
                    transform.rotation,
                    Vector3.one
                );

                Gizmos.matrix = m;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Vector3.zero, 0.1f);
                Gizmos.DrawWireCube(Vector3.zero, node.Bound.Size * 2);
            }

            foreach (var node in cornerLeafs)
            {
                if (DEBUG_CornerBoudingBox == false) break;

                var m = Matrix4x4.TRS(
                    node.Bound.Pos,
                    transform.rotation,
                    Vector3.one
                );

                Gizmos.matrix = m;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(Vector3.zero, 0.1f);
                Gizmos.DrawWireCube(Vector3.zero, node.Bound.Size * 2);
            }
        }
    }
}