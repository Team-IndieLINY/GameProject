using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieLINY.AI
{
    public class CornerFinder : MonoBehaviour
    {
        public PathGenerator Generator;
        public Transform Pivot;
        public int Step = 4;
        public float Fov;
        public float Distance;
        private List<BoundNode> _nodes = new List<BoundNode>(100);

        public Vector2 Size;

        private BoundNode _currentCorner;

        private void Update()
        {
            UpdateCloserPointWays();
        }

        private void UpdateCloserPointWays()
        {
            List<BoundNode> list = new List<BoundNode>(4);

            Vector2 before = Pivot.position;
            for (int i = 0; i < Step; i++)
            {
                var node = GetCornerNode(before, list);
                if (node == null) continue;
                before = node.Bound.Pos;
                list.Add(node);
            }

            if (list.Count > 0)
                Debug.DrawLine(Pivot.position, list[0].Bound.Pos);

            for (int i = 0; i < list.Count - 1; i++)
            {
                Debug.DrawLine(list[i].Bound.Pos, list[i + 1].Bound.Pos);
            }
        }

        private BoundNode GetCornerNode(Vector2 point, List<BoundNode> excludeNodes)
        {
            HashSet<BoundNode> hashSet = new HashSet<BoundNode>();
            Queue<BoundNode> currentNodes = new Queue<BoundNode>(100);

            var pointNode = Generator.GetNodeFromPoint(point);

            if (pointNode == null) return null;
            if (pointNode.Bound.Collision) return null;

            _currentCorner = pointNode;

            currentNodes.Enqueue(pointNode);

            while (currentNodes.Count > 0)
            {
                int length = currentNodes.Count;
                BoundNode minNode = null;
                float minDis = Mathf.Infinity;
                for (int i = 0; i < length; i++)
                {
                    var node = currentNodes.Dequeue();
                    if (node.Bound.Collision) continue;
                    if (hashSet.Contains(node)) continue;
                    if (Generator.Intersects(
                            node.Bound.GetBounds(),
                            new Bounds(Pivot.position, Size))
                        == false)
                        continue;

                    if (node.Bound.IsCorner && excludeNodes.Contains(node) == false)
                    {
                        float dis = Vector2.SqrMagnitude(point - node.Bound.Pos);
                        if (minDis >= dis)
                        {
                            minDis = dis;
                            minNode = node;
                        }
                    }

                    hashSet.Add(node);
                    foreach (var neighbor in node.Neighbor)
                    {
                        if (neighbor.Bound.Collision) continue;
                        if (Generator.Intersects(
                                neighbor.Bound.GetBounds(),
                                new Bounds(Pivot.position, Size))
                            == false)
                            continue;
                        currentNodes.Enqueue(neighbor);
                    }
                }

                if (minNode != null)
                {
                    _currentCorner = minNode;
                    return minNode;
                }
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            var m = Matrix4x4.TRS(
                Pivot.position,
                Generator.transform.rotation,
                Vector2.one
            );
            Gizmos.matrix = m;
            Gizmos.DrawWireCube(Vector3.zero, Size);
        }
    }
}