using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    using IndieLINY.AI.Core;

    public class RotatingSearch : StateBehaviour
    {
        private Vector2 _dir;

        private Vector2 _firstDir;
        private Vector2 _secondDir;

        private SightPerception _sightPerception;

        private IEnumerator _co;

        public bool IsEnd => _co == null;
        
        
        public void OnStateEnter(SightPerception sightPerception, float angle, float duration)
        {
            _dir = sightPerception.Direction;
            _sightPerception = sightPerception;

            _firstDir = Quaternion.Euler(0f, 0f, angle * 0.5f) * _dir;
            _secondDir= Quaternion.Euler(0f, 0f, angle * -0.5f) * _dir;

            StartCoroutine(_co = OnUpdate(duration));
        }
        

        public IEnumerator OnUpdate(float duration)
        {
            var wait = new WaitForEndOfFrame();

            float t = 0f;
            while (true)
            {
                if (t >= 1f)
                {
                    break;
                }
                
                var dir = Vector3.Slerp(_dir, _firstDir, t);
                t += Time.deltaTime / (duration * 0.3333f);
            
                _sightPerception.LookAtDirection(dir);

                yield return wait;
            }

            _dir = _sightPerception.Direction;
            t = 0f;
            while (true)
            {
                if (t >= 1f)
                {
                    break;
                }
                
                var dir = Vector3.Slerp(_dir, _secondDir, t);
                t += Time.deltaTime / (duration * 0.6666f);
            
                _sightPerception.LookAtDirection(dir);

                yield return wait;
            }

            _co = null;
        }

        public void OnStateExit()
        {
            if(_co != null)
                StopCoroutine(_co);

            _co = null;
        }
    }
    
    [UnitCategory("IndieLINY/AI/Unit")]
    [UnitTitle("RotatingSearch")]
    public class RotatingSearchUnit : UBaseEnterUpdateExit
    {
        [DoNotSerialize] private ControlOutput _cUpdate;
        [DoNotSerialize] private ValueInput _vSightPerception;
        [DoNotSerialize] private ValueInput _vRotationAngle;
        [DoNotSerialize] private ValueInput _vRotationDuration;
        
        protected override ControlOutput OnEnter(Flow flow)
        {
            BehaviourBinder
                .GetBinder(flow)
                .GetBehaviour<RotatingSearch>()
                .OnStateEnter(
                    flow.GetValue<SightPerception>(_vSightPerception),
                    flow.GetValue<float>(_vRotationAngle),
                    flow.GetValue<float>(_vRotationDuration)
                    );
            
            return null;
        }

        protected override ControlOutput OnUpdate(Flow flow)
        {
            //TODO: sightPerception rotating implementation
            

            return BehaviourBinder
                .GetBinder(flow)
                .GetBehaviour<RotatingSearch>()
                .IsEnd
                ? _cUpdate
                : null;
        }

        protected override ControlOutput OnExit(Flow flow)
        {
            BehaviourBinder
                .GetBinder(flow)
                .GetBehaviour<RotatingSearch>()
                .OnStateExit();
            
            return null;
        }

        protected override void OnDefinition()
        {
            _vSightPerception = ValueInput<SightPerception>("SightPerception");
            _vRotationDuration = ValueInput<float>("rotating duration");
            _vRotationAngle = ValueInput<float>("rotating angle");
            _cUpdate = ControlOutput("End of rotation");
        }
    }
    
}