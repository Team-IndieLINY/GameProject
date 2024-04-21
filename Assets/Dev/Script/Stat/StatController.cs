using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using IndieLINY.Event;
using UnityEngine;

[RequireComponent(typeof(SteminaView))]
public class StatController : MonoBehaviour, ISteminaController
{
    private Stat _hunger;
    private Stat _thirst;
    private Stat _mental;
    private SteminaView _steminaView;

    private void Awake()
    {
        _hunger = new Hunger();
        _thirst = new Thirsty();
        _mental = new Mental();
        _steminaView = GetComponent<SteminaView>();
    }

    private void Start()
    {
        ResetStemina();
    }

    public float GetStemina()
    {
        return _hunger.GetStemina();
    }

    public void SetStemina(int statValue)
    {
        _hunger.SetStemina(statValue);
        _steminaView.UpdateView(statValue);
    }

    public void Increase(int statValue)
    {
        _hunger.Increase(statValue);
    }

    public void Decrease(int statValue)
    {
        _hunger.Decrease(statValue);
    }

    public void ResetStemina()
    {
        _hunger.ResetStemina();
    }
}
