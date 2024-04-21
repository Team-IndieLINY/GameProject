using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Stat // Start is called before the first frame update
{
    
    public Health()
    {
        _defaultValue = 100;
        _minSteminaValue = 0;
        _maxSteminaValue = 100;
        
        _currentSteminaValue = _defaultValue;

        _statStateTable = new Dictionary<StatState, string>()
        {
            { StatState.Normal, "건강함" },
            { StatState.Warning, "경상" },
            { StatState.Danger, "중상" },
            { StatState.Fatal, "치명상" }
        };
        
        _currentState = StatState.Normal;
    }

    protected override void UpdateStatState()
    {
        if (_currentSteminaValue >= 80)
        {
            _currentState = StatState.Normal;
        }
        else if (_currentSteminaValue >= 60)
        {
            _currentState = StatState.Warning;
        }
        else if(_currentSteminaValue >= 30)
        {
            _currentState = StatState.Danger;
        }
        else
        {
            _currentState = StatState.Fatal;
        }
    }
}