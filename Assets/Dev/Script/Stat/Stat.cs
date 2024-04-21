using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;

public abstract class Stat
{
    public enum StatState
    {
        Normal,
        Warning,
        Danger,
        Fatal
    }

    protected StatState _currentState;
    protected Dictionary<StatState, string> _statStateTable;
    
    protected int _currentSteminaValue;
    
    //앞으로 ScriptableObject로 대체될 것
    protected int _maxSteminaValue;
    protected int _minSteminaValue;
    protected int _defaultValue;
    
    #region public method
    
    public float GetStemina()
    {
        //default value 가 magic number 같음
        return _currentSteminaValue;
    }

    public void ResetStemina()
    {
        _currentSteminaValue = _defaultValue;
        UpdateStatState();
    }
    
    public void SetStemina(int steminaValue)
    {
        _currentSteminaValue = Mathf.Clamp(steminaValue, _minSteminaValue, _maxSteminaValue);
        UpdateStatState();
    }
    
    public void Increase(int steminaValue)
    {
        _currentSteminaValue = Mathf.Clamp(_currentSteminaValue + steminaValue, _minSteminaValue, _maxSteminaValue);
        UpdateStatState();
    }
    
    public void Decrease(int steminaValue)
    {
        _currentSteminaValue = Mathf.Clamp(_currentSteminaValue - steminaValue, _minSteminaValue, _maxSteminaValue);
        UpdateStatState();
    }

    protected abstract void UpdateStatState();

    #endregion
}
