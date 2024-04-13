using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndieLINY.Event;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace IndieLINY.AI
{
    [UnitCategory("IndieLINY/AI/Unit")]
    [UnitTitle("PerceptionMeter")]
    public class PerceptionMeter : MonoBehaviour
    {
        [SerializeField] private float _limitTime;
        [SerializeField] private float _decreaseDuration;
        [SerializeField] private float _waitDuration;
        [SerializeField] private Image _fillGage;

        private List<PerceptionParameterItem> _parameters = new(2);

        public float Value { get; private set; }

        private float _waitTimer = 0f;

        private AotList _contracts;

        private int _rank;

        public const int MAX_RANK = 3;
        public int Rank
        {
            get => _rank;
            set
            {
                _rank = value;

                if (Rank < 0)
                {
                    Rank = 0;
                }
                else if (Rank > MAX_RANK)
                {
                    Rank = MAX_RANK;
                }

                OnRankUp(_rank);
            }
        }

        public AotList Contracts
        {
            get
            {
                if (_contracts == null) _contracts = new AotList();

                _contracts.Clear();
                _contracts.AddRange(_parameters.Select(x => x.Target));

                return _contracts;
            }
        }

        private Action _state;

        public void MeterReset(bool includeRank)
        {
            if (includeRank)
            {
                Rank = 0;
            }
            
            Value = 0f;
            _waitTimer = 0f;
            _state = NoDetectingGage;
            _fillGage.fillAmount = 0f;
        }

        private void Awake()
        {
            _fillGage.fillAmount = 0f;
            Rank = 0;

            _state = NoDetectingGage;
        }

        public void Schedule(CollisionInteraction interaction, float speedFactor)
        {
            var item = new PerceptionParameterItem(interaction, speedFactor);
            _parameters.Add(item);
        }

        public void UnSchedule(CollisionInteraction interaction)
        {
            var index = _parameters.FindIndex(x => x.Target == interaction);
            if (index == -1) return;

            _parameters.RemoveAt(index);
        }

        private float _totalGage = 0f;

        private void Update()
        {
            _state?.Invoke();
            Value = _totalGage / _limitTime;

            if (Rank == 0 && _parameters.Any())
            {
                Rank = 1;
            }

            if (Value >= 1f)
            {
                if (Rank < MAX_RANK)
                {
                    _totalGage = Value = 0f;
                }
                
                Rank += 1;
            }   
            _fillGage.fillAmount = Value;
        }

        public void RankDown()
        {
            Rank -= 1;
            Value = 1f;
            _totalGage = _limitTime;
        }

        private void OnRankUp(int rank)
        {
            if (rank == 1)
            {
                _fillGage.color = Color.yellow;
            }
            if (rank == 2)
            {
                _fillGage.color = Color.red;
            }
            if (rank == 3)
            {
                _fillGage.fillAmount = 1f;
                _fillGage.color = Color.magenta;
            }
        }

        private void DetectingGage()
        {
            if (_parameters.Count == 0)
            {
                _state = WaitingDecrease;
                return;
            }

            foreach (var item in _parameters)
            {
                _totalGage += item.StepTime;
            }

            if (_totalGage >= _limitTime)
            {
                _totalGage = _limitTime;
            }
        }

        private void NoDetectingGage()
        {
            if (_parameters.Count > 0)
            {
                _state = DetectingGage;
                return;
            }

            _totalGage -= Time.deltaTime / _decreaseDuration;
            
            if (_totalGage < 0f)
            {
                _totalGage = 0f;
            }
        }

        private void WaitingDecrease()
        {
            if (_parameters.Count > 0)
            {
                _state = DetectingGage;
                _waitTimer = 0f;
                return;
            }

            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _waitDuration)
            {
                _waitTimer = 0f;
                _state = NoDetectingGage;
            }
        }
    }

    public class PerceptionParameterItem
    {
        public CollisionInteraction Target { get; private set; }
        public float StepTime => _speedFactor * Time.deltaTime;

        private readonly float _speedFactor;

        public PerceptionParameterItem(CollisionInteraction target, float speedFactor)
        {
            Target = target;
            _speedFactor = speedFactor;
        }
    }
}