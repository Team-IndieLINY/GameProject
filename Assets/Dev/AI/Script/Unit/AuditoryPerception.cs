using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("AuditoryPerception")]
    public class AuditoryPerceptionUnit : UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vAuditoryPerceptionComponent;
        [DoNotSerialize] private ValueOutput _vOutput;
        
        private AotList _contractList = new AotList();
        protected override ControlOutput OnExecute(Flow flow)
        {
            Debug.Assert(_vAuditoryPerceptionComponent != null);
            var perception = flow.GetValue<AuditoryPerception>(_vAuditoryPerceptionComponent);
            
            _contractList.Clear();
            _contractList.AddRange(perception.Contracts);
            
            return _contractList.Count > 0 ? OutputTrigger : null;
        }

        protected override void OnDefinition()
        {
            _vAuditoryPerceptionComponent = ValueInput<AuditoryPerception>("AuditoryPerceptionComponent");
            _vOutput = ValueOutput<AotList>("FindTargetList", x=>_contractList);
        }
    }

}