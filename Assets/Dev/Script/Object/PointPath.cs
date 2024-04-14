using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY;
using JetBrains.Annotations;
using UnityEngine;

namespace IndieLINY.AI.Core
{
#if UNITY_EDITOR
    using UnityEditor;
    [System.Serializable]
    internal class PointInfo
    {
        [SerializeField] internal List<GUID> Neighbors;
        [SerializeField] internal GUID Guid;
        [SerializeField] internal bool IsStartPoint;

        [SerializeField] internal Vector3 Position;
        [SerializeField] internal InteractiveDecoratorObject InteractiveDecoratorObject;
    }
#endif

    [System.Serializable]
    public class PatrollPoint
    {
        [SerializeField] internal InteractiveDecoratorObject _interactiveDecoratorObject;
        [SerializeField] internal Vector3 _position;
        [SerializeField] internal Vector3 _nextPosition;

        public InteractiveDecoratorObject InteractiveDecoratorObject => _interactiveDecoratorObject;
        public Vector3 Position => _position;
        public Vector3 NextPosition => _nextPosition;
    }

    [ExecuteInEditMode]
    public class PointPath : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField][HideInInspector] internal List<PointInfo> Points = new();
        [SerializeField][HideInInspector] internal GUID SelectedGuid;

        [CanBeNull]
        internal PointInfo Selected
        {
            get
            {
                var info = Points.Find(x => x.Guid == SelectedGuid);

                return info;
            }
            set => SelectedGuid = value.Guid;
        }
#endif

        [SerializeField] internal List<PatrollPoint> _patrollPoints= new();

        public IReadOnlyList<PatrollPoint> PatrollPoints => _patrollPoints;

    }
}

