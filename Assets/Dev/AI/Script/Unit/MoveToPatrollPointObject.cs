using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.AI.Core;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


namespace IndieLINY.AI
{
    public class MoveToPatrollPointObjectStateBehaivour : StateBehaviour
    {
        private UniTask? _currentTask;
        private CancellationTokenSource _source;

        public bool OnExecute(InteractiveDecoratorObject obj)
        {
            if (_source == null || _source.IsCancellationRequested)
            {
                _source = new CancellationTokenSource();
                _currentTask = obj.Interact(_source.Token);
            }
            
            if (_currentTask!.Value.Status != UniTaskStatus.Pending)
            {
                _currentTask = null;
                _source = null;

                return true;
            }

            return false;
        }

        public bool Cancel()
        {
            if (_source is { IsCancellationRequested: false })
            {
                _source.Cancel();
                _currentTask = null;
                _source = null;

                return true;
            }

            return false;
        }
    }

    [UnitCategory("IndieLINY/Unit")]
    [UnitTitle("InteractToObject")]
    public class InteractToObjectUnit : UBaseI1O1
    {
        private ControlInput _cCancelInput;
        private ControlOutput _cCancelOutput;
        private ValueInput _vObject;

        protected override ControlOutput OnExecute(Flow flow)
        {
            return BehaviourBinder.GetBinder(flow)
                .GetBehaviour<MoveToPatrollPointObjectStateBehaivour>()
                .OnExecute(flow.GetValue<InteractiveDecoratorObject>(_vObject))
                ? OutputTrigger
                : null;
        }

        private ControlOutput OnCanceled(Flow flow)
        {
            return BehaviourBinder.GetBinder(flow)
                .GetBehaviour<MoveToPatrollPointObjectStateBehaivour>()
                .Cancel()
                ? 
                _cCancelOutput : null;
        }

        protected override void OnDefinition()
        {
            _cCancelInput = ControlInput("Cancel", OnCanceled);
            _cCancelOutput = ControlOutput("Canceled");
            _vObject = ValueInput<InteractiveDecoratorObject>("InteractiveDecoratorObject");
        }
    }
}