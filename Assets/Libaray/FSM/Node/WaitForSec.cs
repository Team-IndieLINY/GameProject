using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace IndieLINY.AI.Sample
{
    [UnitCategory("IndieLINY/AI")]
    [UnitTitle("WaitForSec")]
    public class WaitForSec : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;
        [DoNotSerialize] public ValueInput WaitSec;
        [DoNotSerialize] public ValueInput Unscaled;

        private float _timer;
        private float? _targetTime;

        private bool _isUnScaled;

        protected override void Definition()
        {
            InputTrigger = ControlInput("Wait", OnExecute);
            OutputTrigger = ControlOutput("EndOfWait");

            WaitSec = ValueInput("second", 0f);
            Unscaled = ValueInput("unscaled", false);
        }

        private ControlOutput OnExecute(Flow flow)
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
}