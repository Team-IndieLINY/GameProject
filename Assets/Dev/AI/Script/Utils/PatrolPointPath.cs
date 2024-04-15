using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY;
using JetBrains.Annotations;
using UnityEngine;

namespace IndieLINY.AI.Core
{
    #region Editor

#if UNITY_EDITOR
    using UnityEditor;

    [System.Serializable]
    internal class PointInfo
    {
        [SerializeField] internal List<string> Neighbors = new();
        [SerializeField] internal string Guid;
        [SerializeField] internal bool IsStartPoint;

        [SerializeField] internal Vector3 Position;
        [SerializeField] internal InteractiveDecoratorObject InteractiveDecoratorObject;
    }
#endif
    #endregion

    [System.Serializable]
    public class PatrolPoint
    {
        [SerializeField] internal InteractiveDecoratorObject _interactiveDecoratorObject;
        [SerializeField] internal Vector3 _position;
        [SerializeField] internal Vector3 _nextPosition;

        public InteractiveDecoratorObject InteractiveDecoratorObject => _interactiveDecoratorObject;
        public Vector3 Position => _position;
        public Vector3 NextPosition => _nextPosition;
    }

    [ExecuteInEditMode]
    public class PatrolPointPath : MonoBehaviour
    {
        #region Editor
#if UNITY_EDITOR
        [SerializeField] [HideInInspector]  internal List<PointInfo> Points= new();
        [SerializeField] [HideInInspector] internal string SelectedGuid;

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
        #endregion

        [SerializeField] internal List<PatrolPoint> _patrollPoints = new();

        public IReadOnlyList<PatrolPoint> PatrollPoints => _patrollPoints;
    }
}