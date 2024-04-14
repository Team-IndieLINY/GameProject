using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IndieLINY;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace IndieLINY.Editor
{
#if UNITY_EDITOR
    using IndieLINY.AI.Core;
    using UnityEditor;

    [CustomEditor(typeof(PointPath))]
    internal class PointPathEditor : Editor
    {
        private Rect _sceneSize;

        private Vector2 _bttonSize => new Vector2(150f, 100f);
        private Vector2 _bttonOffset => new Vector2(15f, 15f);
        public Vector2 PointBoxSize = new Vector2(50f, 50f);

        private bool _isAdding;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSelectionChanged;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSelectionChanged;

            _isAdding = false;

            GeneratePath();
        }

        private void GeneratePath()
        {
            var pointPath = (PointPath)target;
            var startPoint = pointPath.Points.Find(x => x.IsStartPoint);

            if (startPoint == null)
            {
                Debug.LogError("시작 Patroll point가 없습니다!");
                return;
            }

            List<PatrollPoint> patrollPoints = new List<PatrollPoint>(pointPath.Points.Count);
            HashSet<GUID> stacks = new HashSet<GUID>();

            PointInfo currentPoint = startPoint;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < 1000 && patrollPoints.Count <= 1000)
            {
                stacks.Add(currentPoint.Guid);
                GUID? guid = null;
                foreach (var nextGuid in currentPoint.Neighbors)
                {
                    if (nextGuid != currentPoint.Guid && stacks.Contains(nextGuid) == false)
                    {
                        guid = nextGuid;
                        break;
                    }
                }

                if (guid.HasValue == false)
                {
                    patrollPoints.Add(new PatrollPoint()
                    {
                        _interactiveDecoratorObject = currentPoint.InteractiveDecoratorObject,
                        _position = currentPoint.Position
                    });
                    break;
                }
                
                var nextPoint = pointPath.Points.Find(x=>x.Guid == guid);

                patrollPoints.Add(new PatrollPoint()
                {
                    _interactiveDecoratorObject = currentPoint.InteractiveDecoratorObject,
                    _position = currentPoint.Position,
                    _nextPosition = nextPoint.Position
                });
                
                currentPoint = nextPoint;
            }
            watch.Stop();

            pointPath._patrollPoints = patrollPoints;

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSelectionChanged(SceneView sceneView)
        {
            _sceneSize = EditorWindow.GetWindow<SceneView>().position;
        }

        private void OnSceneGUI()
        {
            var pointPath = (PointPath)target;

            if (pointPath.Points.Count == 0)
            {
                pointPath.Points.Add(new PointInfo()
                {
                    Guid = GUID.Generate(),
                    Position = (Vector2)pointPath.transform.position,
                    Neighbors = new List<GUID>(),
                    IsStartPoint =  true
                });
            }

            foreach (var point in pointPath.Points)
            {
                ValidatePoint(point);
                DrawPoint(point);
            }

            if (OnPointClickAdd() == false)
            {
                OnClickBoundBox();
            }

            var selected = pointPath.Selected;

            if (selected != null)
            {
                DrawPoint(selected);
                DrawButton(selected);
            }
        }

        private bool OnPointClickAdd()
        {
            var pointPath = (PointPath)target;
            var selected = pointPath.Selected;
            var mousePos = UnityEngine.Event.current.mousePosition;

            if (_isAdding && selected != null)
            {
                var e = UnityEngine.Event.current;
                Color c = Color.green;
                var pos = HandleUtility.GUIPointToWorldRay(mousePos).origin;
                Handles.DrawDottedLine(selected.Position, pos, 4.0f);

                InteractiveDecoratorObject interactiveDecoratorObject = GetInteractiveObject(mousePos);
                if (interactiveDecoratorObject != null)
                {
                    c = Color.yellow;
                }

                c.a = 0.5f;

                Handles.color = c;

                Handles.DotHandleCap(
                    1,
                    pos,
                    Quaternion.identity,
                    0.2f,
                    EventType.Repaint
                );

                if (e.type == EventType.KeyDown && e.control)
                {
                    if (interactiveDecoratorObject)
                    {
                        pos = interactiveDecoratorObject.InteractingPositionWorld;
                    }

                    var newInfo = OnAddPoint(pos);
                    newInfo.InteractiveDecoratorObject = interactiveDecoratorObject;

                    selected.Neighbors.Add(newInfo.Guid);
                    newInfo.Neighbors.Add(selected.Guid);

                    pointPath.Selected = newInfo;
                    _isAdding = false;

                    return true;
                }
            }

            return false;
        }

        private void OnClickBoundBox()
        {
            var pointPath = (PointPath)target;

            var camera = SceneView.lastActiveSceneView.camera;

            var e = UnityEngine.Event.current;
            if (e.type == EventType.KeyDown && e.control)
            {
                float minDis = Mathf.Infinity;
                PointInfo minPointInfo = null;

                foreach (var point in pointPath.Points)
                {
                    var newPos = HandleUtility.WorldToGUIPoint(point.Position);
                    Bounds a = new Bounds(UnityEngine.Event.current.mousePosition, PointBoxSize);

                    if (a.Contains(newPos))
                    {
                        var dis = ((Vector2)point.Position - UnityEngine.Event.current.mousePosition).sqrMagnitude;

                        if (dis < minDis)
                        {
                            minPointInfo = point;
                            minDis = dis;
                        }
                    }
                }

                if (minPointInfo != null)
                {
                    pointPath.Selected = minPointInfo;
                }
            }
        }

        [CanBeNull]
        private InteractiveDecoratorObject GetInteractiveObject(Vector2 sceneViewPos)
        {
            var ray = HandleUtility.GUIPointToWorldRay(sceneViewPos);
            var resutls = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);


            foreach (var result in resutls)
            {
                if (result.transform.TryGetComponent<InteractiveDecoratorObject>(out var com))
                {
                    return com;
                }
            }

            return null;
        }

        private void ValidatePoint(PointInfo point)
        {
            var pointPath = (PointPath)target;
            List<GUID> guids = new List<GUID>(2);
            foreach (var guid in point.Neighbors)
            {
                if (pointPath.Points.Any(x => x.Guid == guid))
                {
                    guids.Add(guid);
                }
            }

            if (guids.Count != point.Neighbors.Count)
            {
                point.Neighbors = guids;
            }
        }

        private void DrawPoint(PointInfo point)
        {
            var size = 0.2f;

            if (point.InteractiveDecoratorObject)
            {
                Handles.color = Color.yellow;
            }
            else if (point.IsStartPoint)
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = Color.green;
            }

            Handles.DotHandleCap(
                1,
                point.Position,
                Quaternion.identity,
                size,
                EventType.Repaint
            );

            var pos = Handles.FreeMoveHandle(
                point.Position,
                size,
                Vector3.zero,
                Handles.RectangleHandleCap
            );

            var pointPath = target as PointPath;
            foreach (GUID guid in point.Neighbors)
            {
                var foundPoint = pointPath.Points.Find(x => x.Guid == guid);

                if (foundPoint == null)
                {
                    continue;
                }

                Handles.color = Color.green;
                Handles.DrawLine(foundPoint.Position, point.Position, 2f);
            }

            if (Mathf.Approximately((point.Position - pos).sqrMagnitude, 0f) == false)
            {
                point.Position = pos;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawButton(PointInfo point)
        {
            var pos = WorldPointToViewportSpace(point.Position);
            var width = _sceneSize.width;
            var height = _sceneSize.height;

            var rect = new Rect(
                pos.x * width + _bttonOffset.x,
                height - (pos.y * height + _bttonOffset.y),
                _bttonSize.x,
                _bttonSize.y
            );

            GUILayout.BeginArea(rect);

            if (point.Neighbors.Count <= 1 && GUILayout.Button("Add"))
            {
                _isAdding = true;
            }

            if (point.IsStartPoint == false && point.Neighbors.Count <= 1 && GUILayout.Button("Remove"))
            {
                OnRemovePoint();
            }

            if (point.IsStartPoint == false && point.Neighbors.Count <= 1)
            {
                if (GUILayout.Button("Set Start"))
                {
                    OnStartPoint();
                }
            }

            if (point.InteractiveDecoratorObject == true)
            {
                if (GUILayout.Button("Move to object"))
                {
                    point.Position = point.InteractiveDecoratorObject.InteractingPositionWorld;
                }
            }

            if (point.InteractiveDecoratorObject == true)
            {
                if (GUILayout.Button("clear object"))
                {
                    point.InteractiveDecoratorObject = null;
                }
            }

            var arr = Physics2D.OverlapCircleAll(point.Position, 1f)
                .Select(x => x.GetComponent<InteractiveDecoratorObject>())
                .Where(x => x != null)
                .ToList();
            if (arr.Any() && GUILayout.Button("Set object"))
            {
                point.InteractiveDecoratorObject = arr[0];
                point.Position = point.InteractiveDecoratorObject.InteractingPositionWorld;
            }

            GUILayout.EndArea();
        }

        private Vector2 WorldPointToScreenSpace(Vector3 pos)
        {
            Debug.Assert(SceneView.lastActiveSceneView != null);

            var camera = SceneView.lastActiveSceneView.camera;

            pos = camera.WorldToViewportPoint(pos);

            pos = new Vector2(
                pos.x * _sceneSize.width,
                _sceneSize.height - pos.y * _sceneSize.height
            );


            return pos * camera.orthographicSize;
        }

        private Vector2 WorldPointToViewportSpace(Vector3 pos)
        {
            Debug.Assert(SceneView.lastActiveSceneView != null);

            var camera = SceneView.lastActiveSceneView.camera;
            return camera.WorldToViewportPoint(pos);
        }

        private Vector2 ScreenPointToWorldSpace(Vector2 pos)
        {
            Debug.Assert(SceneView.lastActiveSceneView != null);

            var camera = SceneView.lastActiveSceneView.camera;

            pos = new Vector2(
                pos.x / _sceneSize.width,
                1f - pos.y / _sceneSize.height
            );

            return camera.ViewportToWorldPoint(pos);
        }

        private Ray GetScreenRay(Vector2 sceneViewPos)
        {
            var camera = SceneView.lastActiveSceneView.camera;

            sceneViewPos = new Vector2(
                sceneViewPos.x / _sceneSize.width,
                1f - sceneViewPos.y / _sceneSize.height
            );

            return camera.ViewportPointToRay(sceneViewPos);
        }

        private bool OverlapPoint(Vector2 aPos, Vector2 aSize, Vector2 bPos, Vector2 bSize)
        {
            Bounds a = new Bounds(aPos, aSize);
            Bounds b = new Bounds(bPos, bSize);

            return a.Intersects(b);
        }

        private PointInfo OnAddPoint(Vector2 worldPos)
        {
            var pointPath = (PointPath)target;


            PointInfo info = new PointInfo()
            {
                Guid = GUID.Generate(),
                Position = worldPos,
                Neighbors = new List<GUID>()
            };

            pointPath.Points.Add(info);

            return info;
        }

        private void OnRemovePoint()
        {
            var pointPath = (PointPath)target;
            var index = pointPath.Points.FindIndex(x => x.Guid == pointPath.SelectedGuid);
            pointPath.Points.RemoveAt(index);
        }

        private void OnStartPoint()
        {
            var pointPath = (PointPath)target;

            pointPath.Points.ForEach(x => x.IsStartPoint = false);

            var selected = pointPath.Selected;
            if (selected != null)
            {
                selected.IsStartPoint = true;
            }
        }
    }
#endif
}