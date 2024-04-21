using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteminaController : MonoBehaviour
{
    private SteminaView _steminaView;
    private int _currentStemina = 500;

    private readonly int _defaultStemina = 700;
    private readonly int _minStemina = 0;
    private readonly int _maxStemina = 700;

    private void Awake()
    {
        _steminaView = GetComponent<SteminaView>();
    }

    private void Start()
    {
        _steminaView.SetSteminaView(_minStemina, _maxStemina, _currentStemina);
        ResetStemina();
        _steminaView.UpdateView(_currentStemina);
    }

    public int GetStemina()
    {
        return _currentStemina;
    }

    public void SetStemina(int statValue)
    {
        _currentStemina = statValue;
        _steminaView.UpdateView(_currentStemina);
    }

    public void Increase(int statValue)
    {
        _currentStemina = Mathf.Clamp(_currentStemina + statValue, _minStemina, _maxStemina);
        _steminaView.UpdateView(_currentStemina);
    }

    public void Decrease(int statValue)
    {
        _currentStemina = Mathf.Clamp(_currentStemina - statValue, _minStemina, _maxStemina);
        _steminaView.UpdateView(_currentStemina);
    }

    public void ResetStemina()
    {
        _currentStemina = _defaultStemina;
        _steminaView.UpdateView(_currentStemina);
    }
}
