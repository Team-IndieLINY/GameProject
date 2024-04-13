using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieLINY.AI
{
    public abstract class UBaseI1O1 : Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;

        protected virtual string InputDesc => "input";
        protected virtual string OutputDesc => "output";

        protected sealed override void Definition()
        {
            InputTrigger = ControlInput(InputDesc, OnExecute);
            OutputTrigger = ControlOutput(OutputDesc);

            OnDefinition();
        }

        protected abstract ControlOutput OnExecute(Flow flow);

        protected abstract void OnDefinition();
    }

    public abstract class UBaseI1O1Start: Unit
    {
        [DoNotSerialize] public ControlInput InputTrigger;
        [DoNotSerialize] public ControlInput StartTrigger;
        [DoNotSerialize] public ControlOutput OutputTrigger;

        protected virtual string InputDesc => "input";
        protected virtual string OutputDesc => "output";
        protected virtual string StartDesc => "Start";

        protected sealed override void Definition()
        {
            InputTrigger = ControlInput(InputDesc, OnExecute);
            StartTrigger = ControlInput(StartDesc, OnStart);
            OutputTrigger = ControlOutput(OutputDesc);

            OnDefinition();
        }

        protected abstract ControlOutput OnStart(Flow flow);
        protected abstract ControlOutput OnExecute(Flow flow);

        protected abstract void OnDefinition();
    }

    public abstract class UBaseEnterUpdateExit : Unit
    {
        [DoNotSerialize] protected ControlInput InputEnter;
        [DoNotSerialize] protected ControlInput InputUpdate;
        [DoNotSerialize] protected ControlInput InputExit;

        protected sealed override void Definition()
        {
            InputEnter = ControlInput("enter", OnEnter);
            InputUpdate = ControlInput("update", OnUpdate);
            InputExit = ControlInput("exit", OnExit);

            OnDefinition();
        }

        protected abstract ControlOutput OnEnter(Flow flow);
        protected abstract ControlOutput OnUpdate(Flow flow);
        protected abstract ControlOutput OnExit(Flow flow);

        protected abstract void OnDefinition();
    }
}