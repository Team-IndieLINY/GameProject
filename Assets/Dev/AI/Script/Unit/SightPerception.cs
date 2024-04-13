using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("SightPerception")]
    public class SightPerceptionUnit : UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vSightPerceptionComponent;
        [DoNotSerialize] private ValueOutput _vOutput;
        
        private AotList _contractList = new AotList();
        protected override ControlOutput OnExecute(Flow flow)
        {
            Debug.Assert(_vSightPerceptionComponent != null);
            var perception = flow.GetValue<SightPerception>(_vSightPerceptionComponent);
            
            _contractList.Clear();
            _contractList.AddRange(perception.Contracts);
            
            return _contractList.Count > 0 ? OutputTrigger : null;
        }

        protected override void OnDefinition()
        {
            _vSightPerceptionComponent = ValueInput<SightPerception>("SightPerceptionComponent");
            _vOutput = ValueOutput<AotList>("FindTargetList", x=>_contractList);
        }
    }

}