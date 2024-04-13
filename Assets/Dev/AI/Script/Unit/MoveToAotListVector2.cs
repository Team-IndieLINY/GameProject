using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("Move to AotList Vector2")]
    public class MoveToAotListVector2 : UBaseI1O1Start
    {
        [DoNotSerialize] protected override string OutputDesc => "End of Move";

        [DoNotSerialize] private ValueInput _vAotList;
        [DoNotSerialize] private ValueInput _vNavAgent;

        [DoNotSerialize] private AotList _aotList;
        [DoNotSerialize] private NavMeshAgent _agent;
        [DoNotSerialize] private List<Vector2> _points;
        [DoNotSerialize] private int _currentIndex;

        protected override ControlOutput OnStart(Flow flow)
        {
            _aotList = flow.GetValue<AotList>(_vAotList);
            _agent = flow.GetValue<NavMeshAgent>(_vNavAgent);

            if (_points == null)
            {
                _points = new List<Vector2>(10);
            }
            
            _points.Clear();
            _points.AddRange(_aotList);
            _currentIndex = 0;
            
            return null;
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            if (_currentIndex >= _points.Count)
            {
                return OutputTrigger;
            }

            var point = _points[_currentIndex];

            if (Vector2.SqrMagnitude(point - (Vector2)_agent.transform.position) <= (0.5f * 0.5f))
            {
                if (_currentIndex >= _points.Count)
                {
                    return OutputTrigger;
                }
                point = _points[_currentIndex++];
            }

            _agent.SetDestination(point);

            Debug.DrawLine(_agent.transform.position, _points[0]);
            for(int i=0;i<_points.Count-1;i++)
            {
                Debug.DrawLine(_points[i], _points[i + 1]);
            }

            return null;
        }

        protected override void OnDefinition()
        {
            _vAotList = ValueInput<AotList>("Vector2 AotList");
            _vNavAgent = ValueInput<NavMeshAgent>("agent");
        }
    }
}