using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IndieLINY.AI.Core;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("MoveToPatrollPointPosition")]
    public class MoveToPatrollPointPositionUnit : UBaseI1O1
    {
        private ValueInput _vPoint;
        private ValueInput _vAgent;

        protected override string InputDesc => "Update";
        protected override string OutputDesc => "End of Update";

        protected override ControlOutput OnExecute(Flow flow)
        {
            var point = flow.GetValue<PatrolPoint>(_vPoint);
            var agent = flow.GetValue<NavMeshAgent>(_vAgent);

            agent.SetDestination(point.Position);

            return OutputTrigger;
        }

        protected override void OnDefinition()
        {
            _vPoint = ValueInput<PatrolPoint>("patrol point(position)");
            _vAgent = ValueInput<NavMeshAgent>("nav agent");
        }
    }
}