using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using IndieLINY.Event;
using UnityEngine;

[RequireComponent(typeof(SteminaView))]
public class SteminaController : MonoBehaviour, ISteminaController
{
    private readonly Stemina _stemina = new();
    private SteminaView _steminaView;

    private void Awake()
    {
        _steminaView = GetComponent<SteminaView>();
    }

    private void Start()
    {
        ResetStemina(ESteminaType.Endurance);
        _steminaView.UpdateView(
            _stemina.ConvertOriginSteminaToNormalizedStemina(_stemina.GetStemina(ESteminaType.Endurance)));
    }

    public float GetStemina(ESteminaType type)
    {
        return _stemina.GetStemina(type);
    }

    public void SetStemina(ESteminaType type, float normalizedValue)
    {
        _stemina.SetStemina(type, normalizedValue);
        _steminaView.UpdateView(normalizedValue);
    }

    public void Increase(ESteminaType type, float normalizedValue)
    {
        _stemina.Increase(type, normalizedValue);
        _steminaView.UpdateView(
            _stemina.ConvertOriginSteminaToNormalizedStemina(_stemina.GetStemina(type)));
    }

    public void Decrease(ESteminaType type, float normalizedValue)
    {
        _stemina.Decrease(type, normalizedValue);
        _steminaView.UpdateView(
            _stemina.ConvertOriginSteminaToNormalizedStemina(_stemina.GetStemina(type)));
    }

    public void ResetStemina(ESteminaType type)
    {
        _stemina.ResetStemina(type);
        _steminaView.UpdateView(
            _stemina.ConvertOriginSteminaToNormalizedStemina(_stemina.GetStemina(type)));
    }
}
