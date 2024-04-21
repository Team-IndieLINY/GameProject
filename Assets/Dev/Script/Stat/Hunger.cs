using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : Stat
{
    //하루가 종료되면 감소되는 스텟
    private int _decreaseValue;
    
    public Hunger()
    {
        _defaultValue = 100;
        _minSteminaValue = 0;
        _maxSteminaValue = 100;
        _decreaseValue = 15;
        
        _currentSteminaValue = _defaultValue;
        
        _statStateTable = new Dictionary<StatState, string>()
        {
            { StatState.Normal, "배부름" },
            { StatState.Warning, "배고픔" },
            { StatState.Danger, "굶주림" },
            { StatState.Fatal, "영양실조" }
        };
        
        _currentState = StatState.Normal;

        TimeManager.Instance.EndRoutineCallback += OnEndRoutine;
    }

    private void OnEndRoutine()
    {
        _currentSteminaValue = Mathf.Clamp(_currentSteminaValue - _decreaseValue, _minSteminaValue, _maxSteminaValue);
        Debug.Log($"배고픔: {_currentSteminaValue}");
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
