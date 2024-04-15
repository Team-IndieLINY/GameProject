using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI
{
    using IndieLINY.AI.Core;

    public class PatrollPointPathFinder : StateBehaviour
    {
        private int? _currentIndex;

        [CanBeNull]
        public PatrolPoint GetNextPoint(PatrolPointPath path)
        {
            if (_currentIndex == null)
            {
                if (path.PatrollPoints.Any())
                {
                    _currentIndex = 0;
                    return path.PatrollPoints[_currentIndex.Value];
                }

                return null;
            }
            else
            {
                _currentIndex++;
                if (path.PatrollPoints.Count <= _currentIndex)
                {
                    _currentIndex = 0;
                }

                return path.PatrollPoints[_currentIndex.Value];
            }
        }
    }

    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("PatrollPointPathFinder")]
    public class PatrolPointPathFinderUnit : Unit
    {
        private ValueInput _vPatrollPointPath;
        private ValueOutput _vPoint;

        private ControlInput _cStart;
        private ControlOutput _cOutput;

        private PatrolPoint _lastPoint;

        private ControlOutput OnExecute(Flow flow)
        {
            _lastPoint = BehaviourBinder.GetBinder(flow)
                .GetBehaviour<PatrollPointPathFinder>()
                .GetNextPoint(
                    flow.GetValue<PatrolPointPath>(_vPatrollPointPath)
                );

            Debug.Assert(_lastPoint != null, "patrol point가 null 입니다.");

            return _cOutput;
        }

        protected override void Definition()
        {
            _vPatrollPointPath = ValueInput<PatrolPointPath>("Patrol point path");
            _vPoint = ValueOutput("Patrol point", x => _lastPoint);

            _cOutput = ControlOutput("");
            _cStart = ControlInput("OnEnterState", OnExecute);
        }
    }
}