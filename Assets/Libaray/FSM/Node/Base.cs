using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    public abstract class UBaseI1O1 : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;

        protected sealed override void Definition()
        {
            InputTrigger = ControlInput("input", OnExecute);
            OutputTrigger = ControlOutput("output");
            
            OnDefinition();
        }

        protected abstract ControlOutput OnExecute(Flow flow);
        
        protected abstract void OnDefinition();
    }
    
    public abstract class UBaseEnterUpdateExit : Unit
    {
        [DoNotSerialize] public ControlInput InputEnter;
        [DoNotSerialize] public ControlInput InputUpdate;
        [DoNotSerialize] public ControlInput InputExit;
        
        [DoNotSerialize] public ControlOutput OutputTrigger;

        protected sealed override void Definition()
        {
            InputEnter = ControlInput("enter", Enter);
            InputUpdate = ControlInput("update", Update);
            InputExit = ControlInput("exit", Exit);
            OutputTrigger = ControlOutput("output");
            OnDefinition();
        }

        private ControlOutput Enter(Flow flow)
        {
            OnEnter(flow);

            return OutputTrigger;
        }
        private ControlOutput Update(Flow flow)
        {
            OnUpdate(flow);

            return OutputTrigger;
        }
        private ControlOutput Exit(Flow flow)
        {
            OnExit(flow);

            return OutputTrigger;
        }

        protected abstract void OnEnter(Flow flow);
        protected abstract void OnUpdate(Flow flow);
        protected abstract void OnExit(Flow flow);
        
        protected abstract void OnDefinition();
    }
}