using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;

public class Stemina
{
    private readonly float _currentEndurance = 0f;
    private readonly float _maxEndurance = 150f;

    private readonly Dictionary<ESteminaType, float> _steminaTable;

    #region public method

    public Stemina()
    {
        _steminaTable = new Dictionary<ESteminaType, float>()
        {
            {ESteminaType.Endurance, _currentEndurance}
        };
    }
    
    public float GetStemina(ESteminaType type)
    {
        //default value 가 magic number 같음
        return _steminaTable.GetValueOrDefault(type, -1f);
    }

    public void ResetStemina(ESteminaType type)
    {
        float steminaAmount = _steminaTable.GetValueOrDefault(type, -1f);
        Debug.Assert(Math.Abs(steminaAmount - (-1f)) > 0, "입력한 ESteminaType은 없는 타입입니다.");

        _steminaTable[type] = 150f;
    }
    
    public void SetStemina(ESteminaType type, float normalizedValue)
    {
        if (normalizedValue > 1f)
        {
            normalizedValue = 1f;
        }
        else if(normalizedValue < 0f)
        {
            normalizedValue = 0f;
        }
        
        float steminaAmount = _steminaTable.GetValueOrDefault(type, -1f);
        
        Debug.Assert(Math.Abs(steminaAmount - (-1f)) > 0, "입력한 ESteminaType은 없는 타입입니다.");

        _steminaTable[type] = ConvertNormalizedSteminaToOriginStemina(normalizedValue);
    }
    
    public void Increase(ESteminaType type, float normalizedValue)
    {
        float steminaAmount = _steminaTable.GetValueOrDefault(type, -1f);
        
        Debug.Assert(Math.Abs(steminaAmount - (-1f)) > 0, "입력한 ESteminaType은 없는 타입입니다.");
        
        steminaAmount += ConvertNormalizedSteminaToOriginStemina(normalizedValue);

        _steminaTable[type] = steminaAmount;

        if (_steminaTable[type] > 150f)
        {
            _steminaTable[type] = 150f;
        }
    }
    
    public void Decrease(ESteminaType type, float normalizedValue)
    {
        float steminaAmount = _steminaTable.GetValueOrDefault(type, -1f);
        
        Debug.Assert(Math.Abs(steminaAmount - (-1f)) > 0, "입력한 ESteminaType은 없는 타입입니다.");
        
        steminaAmount -= ConvertNormalizedSteminaToOriginStemina(normalizedValue);

        _steminaTable[type] = steminaAmount;
        
        if (_steminaTable[type] < 0)
        {
            _steminaTable[type] = 0f;
        }
    }

    #endregion

    public float ConvertNormalizedSteminaToOriginStemina(float normalizedValue)
    {
         float result = normalizedValue * _maxEndurance;

         return result;
    }
    
    public float ConvertOriginSteminaToNormalizedStemina(float originValue)
    {
        float result = originValue / _maxEndurance;

        return result;
    }

}
