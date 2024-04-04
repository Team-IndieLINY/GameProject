using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace IndieLINY.AI.Sample
{
    [UnitCategory("IndieLINY/Utils")]
    [UnitTitle("WaitForSec")]
    public class WaitForSec : UBaseI1O1
    {
        [DoNotSerialize] public ValueInput WaitSec;
        [DoNotSerialize] public ValueInput Unscaled;

        private float _timer;
        private float? _targetTime;

        private bool _isUnScaled;

        protected override string InputDesc => "Wait";
        protected override string OutputDesc => "EndOfWait";

        protected override void OnDefinition()
        {
            WaitSec = ValueInput("second", 0f);
            Unscaled = ValueInput("unscaled", false);
        }

        protected override ControlOutput OnExecute(Flow flow)
        {
            if (_targetTime.HasValue == false)
            {
                _targetTime = flow.GetValue<float>(WaitSec);
                _isUnScaled = flow.GetValue<bool>(Unscaled);
                _timer = 0f;
            }

            if (_timer < _targetTime.Value)
            {
                _timer += _isUnScaled ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            else
            {
                _targetTime = null;
                return OutputTrigger;
            }

            return null;
        }
    }
    
    [UnitCategory("IndieLINY/Utils")]
    [UnitTitle("WaitForNextFrame")]
    public class WaitForNextFrame : UBaseI1O1
    {
        protected override string InputDesc => "Wait";
        protected override string OutputDesc => "NextFrame";
        
        private bool _pass;

        protected override ControlOutput OnExecute(Flow flow)
        {
            if (_pass == false)
            {
                _pass = true;
                return null;
            }
            else
            {
                _pass = false;
                return OutputTrigger;
            }
        }

        protected override void OnDefinition() { }
    }
}