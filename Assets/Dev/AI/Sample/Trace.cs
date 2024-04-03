using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI.Sample
{
    [UnitCategory("IndieLINY/AI")]
    [UnitTitle("SampleTrace")]
    public class Trace : UBaseEnterUpdateExit
    {
        [DoNotSerialize] public ValueInput Target;
        [DoNotSerialize] public ValueInput TraceData;
        [DoNotSerialize] public ValueInput Agent;

        private TraceData _data;
        private Transform _transform;
        private NavMeshAgent _agent;

        protected override ControlOutput OnEnter(Flow flow)
        {
            _data = flow.GetValue<TraceData>(TraceData);
            _transform = flow.GetValue<Transform>(Target);
            _agent = flow.GetValue<NavMeshAgent>(Agent);
            
            return null;
        }

        protected override ControlOutput OnUpdate(Flow flow)
        {
            if (_transform == false) return null;
            if (_agent.transform == false) return null;

            _agent.SetDestination(_transform.position);
            
            return null;
        }

        protected override ControlOutput OnExit(Flow flow)
        {
            
            return null;
        }

        protected override void OnDefinition()
        {
            Target = ValueInput<Transform>("target", null);
            TraceData = ValueInput<TraceData>("trace data", null);
            Agent = ValueInput<NavMeshAgent>("nav agent", null);
        }
    }

}