using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mental : Stat
{
    //하루가 종료되면 감소되는 스텟
    private int _decreaseValue;
    
    public Mental()
    {
        _defaultValue = 100;
        _minSteminaValue = 0;
        _maxSteminaValue = 100;
        _decreaseValue = 10;
        
        _currentSteminaValue = _defaultValue;
        
        _statStateTable = new Dictionary<StatState, string>()
        {
            { StatState.Normal, "행복함" },
            { StatState.Warning, "슬픔" },
            { StatState.Danger, "우울함" },
            { StatState.Fatal, "겁에질림" }
        };
        
        _currentState = StatState.Normal;
        
        TimeManager.Instance.EndRoutineCallback += OnEndRoutine;
    }
    
    private void OnEndRoutine()
    {
        _currentSteminaValue = Mathf.Clamp(_currentSteminaValue - _decreaseValue, _minSteminaValue, _maxSteminaValue);
        Debug.Log($"정신력: {_currentSteminaValue}");
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
