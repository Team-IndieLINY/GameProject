using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Debug")]
    public class DebugLog : UBaseI1O1
    {
        [DoNotSerialize] public ValueInput InputString;

        protected override void OnDefinition()
        {
            InputString = ValueInput<string>("String", "");
        }
        
        protected override ControlOutput OnExecute(Flow flow)
        {
            Debug.Log(flow.GetValue(InputString));
            return OutputTrigger;
        }
    }
}