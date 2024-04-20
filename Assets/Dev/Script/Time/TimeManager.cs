using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IndieLINY.Singleton;
using UnityEditor;
using UnityEngine;

[Singleton(ESingletonType.Global)]
public class TimeManager : MonoBehaviourSingleton<TimeManager>
{
    public enum ETime
    {
        Day,
        Night
    }

    private ETime _timeType = ETime.Day;
    
    private float _currentTime = 0f;
    
    //하루 루틴이 시작되었을 때 콜백 함수를 등록하는 Action이다. (동작 순서에 무관한 함수를 등록하는 것을 추천)
    public event Action StartRoutineCallback;

    //하루 루틴이 종료되었을 때 콜백 함수를 등록하는 함수이다. (동작 순서에 무관한 함수를 등록하는 것을 추천)
    public event Action EndRoutineCallback;

    private Coroutine _passTimeCoroutine;
    
    [SerializeField] private int _delayTime;

    [SerializeField] private int _minTime;
    [SerializeField] private int _maxTime;
    
    [SerializeField] private int _nightStartTime;

    public ETime TimeType => _timeType;

    private void Awake()
    {
        StartRoutine();
    }

    //하루를 시작하는 함수이다.
    public void StartRoutine()
    {
        _timeType = ETime.Day;
        _currentTime = _minTime;
        
        TriggerStartRoutineEvent();
        
        _passTimeCoroutine = StartCoroutine(PassTime());
    }

    //시간을 흐르게하는 함수이다.
    public IEnumerator PassTime()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(_delayTime);
        while (true)
        {
            yield return waitForSeconds;

            _currentTime = Mathf.Clamp(_currentTime + _delayTime, _minTime, _maxTime);
            
            Debug.Log(_currentTime);

            UpdateTimeState();

            if (_currentTime >= _maxTime)
            {
                TriggerEndRoutineEvent();
                yield break;
            }
        }
    }

    public void PauseTime()
    {
        if (_passTimeCoroutine == null)
        {
            return;
        }
        
        StopCoroutine(_passTimeCoroutine);
        _passTimeCoroutine = null;
    }
    
    private void UpdateTimeState()
    {
        _timeType = (_currentTime >= _nightStartTime) ? ETime.Night : ETime.Day;
    }

    private void TriggerStartRoutineEvent()
    {
        StartRoutineCallback?.Invoke();
    }
    
    private void TriggerEndRoutineEvent()
    {
        EndRoutineCallback?.Invoke();
    }

    public override void PostInitialize()
    {
        
    }

    public override void PostRelease()
    {
    }
}
