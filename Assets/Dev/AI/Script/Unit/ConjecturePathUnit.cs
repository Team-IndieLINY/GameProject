using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/AI/Unit")]
    [UnitTitle("Get ConjecturePath")]
    public class ConjecturePathUnit : UBaseI1O1Start
    {
        [DoNotSerialize] private ValueInput _vCornerFinder;
        [DoNotSerialize] private ValueInput _vAgent;
        [DoNotSerialize] private ValueInput _vLastSight;

        [DoNotSerialize] private NavMeshAgent _agent;
        [DoNotSerialize] private CornerFinder _finder;
        [DoNotSerialize] private BoundNode _curCornerNode;
 
        protected override string StartDesc => "Start";
        protected override string InputDesc => "Update";
        protected override string OutputDesc => "End Of Update";

        protected override ControlOutput OnStart(Flow flow)
        {
            _finder = flow.GetValue<CornerFinder>(_vCornerFinder);
            _agent = flow.GetValue<NavMeshAgent>(_vAgent);
            var lastSight = flow.GetValue<Vector2>(_vLastSight);

            _curCornerNode = _finder.GetCornerNode(_agent.transform.position, _finder.Size);
            
            if (_curCornerNode != null)
            {
                _curCornerNode = _finder.GetNextCornerNode(_curCornerNode, lastSight - _curCornerNode.Bound.Pos);

                if (_curCornerNode != null)
                    _agent.SetDestination(_curCornerNode.Bound.Pos);
            }
            
            return null;
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            if (_curCornerNode == null)
            {
                _curCornerNode = _finder.GetCornerNode(_agent.transform.position, _finder.Size);

                if (_curCornerNode == null) return OutputTrigger;
                _agent.SetDestination(_curCornerNode.Bound.Pos);
            }

            if (_agent.remainingDistance <= 2f)
            {
                _curCornerNode =
                    _finder.GetNextCornerNode(_curCornerNode, _agent.desiredVelocity);


                Debug.Log(_curCornerNode);
                if (_curCornerNode != null)
                    _agent.SetDestination(_curCornerNode.Bound.Pos);
            }


            return OutputTrigger;
        }

        protected override void OnDefinition()
        {
            _vCornerFinder = ValueInput<CornerFinder>("CornerFinder");
            _vAgent = ValueInput<NavMeshAgent>("Nav agent");
            _vLastSight = ValueInput<Vector2>("LasSight");
        }
    }
}