using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("ToMoveNavPoint")]
    public class ToMoveNavPoint : UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vNavAgent;
        [DoNotSerialize] private ValueInput _vTargetPoint;

        private NavMeshAgent _agent;
        private Vector2 _targetPoint;


        protected override void OnDefinition()
        {
            _vNavAgent = ValueInput<NavMeshAgent>("nav agent");
            _vTargetPoint = ValueInput<Vector2>("target point");
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            var agent = flow.GetValue<NavMeshAgent>(_vNavAgent);
            var point = flow.GetValue<Vector2>(_vTargetPoint);

            agent.SetDestination(point);

            return OutputTrigger;
        }
    }
    
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("ToMoveNavCollisionInteraction")]
    public class ToMoveNavCollisionInteraction : UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vNavAgent;
        [DoNotSerialize] private ValueInput _vTarget;


        protected override void OnDefinition()
        {
            _vNavAgent = ValueInput<NavMeshAgent>("nav agent");
            _vTarget = ValueInput<CollisionInteraction>("target");
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            var agent = flow.GetValue<NavMeshAgent>(_vNavAgent);
            var target = flow.GetValue<CollisionInteraction>(_vTarget);

            agent.SetDestination(target.ContractInfo.Transform.position);
            return OutputTrigger;
        }
    }
    
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("ToMoveNavTransform")]
    public class ToMoveNavTransform: UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vNavAgent;
        [DoNotSerialize] private ValueInput _vTarget;

        protected override void OnDefinition()
        {
            _vNavAgent = ValueInput<NavMeshAgent>("nav agent");
            _vTarget = ValueInput<Transform>("target");
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            var agent = flow.GetValue<NavMeshAgent>(_vNavAgent);
            var target = flow.GetValue<Transform>(_vTarget);

            agent.SetDestination(target.position);
            return OutputTrigger;
        }
    }

}