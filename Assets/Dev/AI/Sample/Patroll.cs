using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI.Sample
{
    [UnitCategory("IndieLINY/AI/Action")]
    [UnitTitle("SamplePatroll")]
    public class Patroll : UBaseEnterUpdateExit
    {
        [DoNotSerialize] public ValueInput PatrollData;
        [DoNotSerialize] public ValueInput Agent;
        [DoNotSerialize] public ValueInput PatrollPoints;

        private NavMeshAgent _agent;
        private List<Transform> _patrollPoints;
        private PatrollData _data;

        private int _currentIndex;

        protected override ControlOutput OnEnter(Flow flow)
        {
            _agent = flow.GetValue<NavMeshAgent>(Agent);
            _data = flow.GetValue<PatrollData>(PatrollData);
            _patrollPoints = flow.GetValue<AotList>(PatrollPoints).OfType<Transform>().ToList();
            _currentIndex = 0;

            return null;
        }

        protected override ControlOutput OnUpdate(Flow flow)
        {
            Debug.Assert(_patrollPoints != null);
            Debug.Assert(_patrollPoints[_currentIndex]);

            
            
            var point = _patrollPoints[_currentIndex];
            
            Debug.Assert(point);
            Debug.Assert(_agent);

            if (Vector2.SqrMagnitude(point.position - _agent.transform.position) <= 0.1f)
            {
                _currentIndex++;
            }

            if (_currentIndex >= _patrollPoints.Count)
            {
                _currentIndex = 0;
            }
            
            point = _patrollPoints[_currentIndex];
            _agent.SetDestination(point.position);
            
            return null;
        }

        protected override ControlOutput OnExit(Flow flow)
        {
            _agent = null;
            _patrollPoints = null;
            _currentIndex = 0;
            
            return null;
        }

        protected override void OnDefinition()
        {
            PatrollPoints = ValueInput<AotList>("patroll_points", null);
            PatrollData = ValueInput<PatrollData>("patrollData", null);
            Agent = ValueInput<NavMeshAgent>("nav agent", null);
        }
    }

}