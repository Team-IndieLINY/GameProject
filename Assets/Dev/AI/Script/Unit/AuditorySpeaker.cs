using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("AuditorySpeaker")]
    public class AuditorySpeakerUnit : UBaseI1O1
    {
        [DoNotSerialize] private ValueInput _vRadius;
        [DoNotSerialize] private ValueInput _vRadialSpeed;
        [DoNotSerialize] private ValueInput _vAuditorySpeed;
        [DoNotSerialize] private ValueOutput _vOutput;
        protected override ControlOutput OnExecute(Flow flow)
        {
            var auditory = flow.GetValue<AuditorySpeaker>(_vAuditorySpeed);
            var radius = flow.GetValue<float>(_vRadius);
            var radialSpeed = flow.GetValue<float>(_vRadialSpeed);
            
            auditory.Play(radialSpeed, radius);

            return OutputTrigger;
        }

        protected override void OnDefinition()
        {
            _vRadius = ValueInput<float>("Radius");
            _vRadialSpeed = ValueInput<float>("Radial Speed");
            _vAuditorySpeed = ValueInput<AuditorySpeaker>("AuditorySpeaker");
        }
    }
}