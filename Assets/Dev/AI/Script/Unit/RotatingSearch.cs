using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/AI/Unit")]
    [UnitTitle("RotatingSearch")]
    public class RotatingSearch : Unit
    {
        [DoNotSerialize] private ControlInput _cEnter;
        [DoNotSerialize] private ControlInput _cUpdate;
        [DoNotSerialize] private ControlOutput _cEnd;
        [DoNotSerialize] private ValueInput _vTarget;
        [DoNotSerialize] private ValueInput _vRotationDuration;

        private CollisionInteraction _interaction;
        private float _currentTime;
        private float _duration;
        private Vector3 _currentDir;
        private Vector3 _targetDir;
        
        private ControlOutput OnEnter(Flow flow)
        {
            _currentTime = 0f;
            _interaction = flow.GetValue<CollisionInteraction>(_vTarget);
            _duration = flow.GetValue<float>(_vRotationDuration);

            _currentDir = _interaction.transform.right;
            _targetDir =  Quaternion.Euler(0f, 0f, 356) * _currentDir;

            return null;
        }

        private ControlOutput OnUpdate(Flow flow)
        {
            //TODO: sightPerception rotating implementation
            
            return _cEnd;
        }

        protected override void Definition()
        {
            _cEnter = ControlInput("OnEnter", OnEnter);
            _cUpdate = ControlInput("OnUpdate", OnUpdate);
            _cEnd = ControlOutput("EndOfRotate");
            _vTarget = ValueInput<CollisionInteraction>("rotating target");
            _vRotationDuration = ValueInput<float>("rotating duration");
        }
    }

}