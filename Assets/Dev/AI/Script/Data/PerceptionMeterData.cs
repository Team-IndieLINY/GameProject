using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/AI/PerceptionMeter", fileName = "PerceptionMeter")]
    public class PerceptionMeterData : ScriptableObject
    {
        [Header("Rank1 상태로 전이되기까지 걸리는 시간 (sec)")]
        [SerializeField] private float _rank1FindingTime;
        [Header("Rank2 상태로 전이되기까지 걸리는 시간 (sec)")]
        [SerializeField] private float _rank2FindingTime;
        [Header("Rank3 상태로 전이되기까지 걸리는 시간 (sec)")]
        [SerializeField] private float _rank3FindingTime;
        
        
        [Header("이전 상태(rank)로 전이되는 시간(가득 차있을 때 기준) (sec)")]
        [SerializeField] private float _decreaseDuration;
        [Header("감지 계수가 줄어들기까지 대기하는 시간 (sec)")]
        [SerializeField] private float _waitDuration;

        public float Rank1FindingTime => _rank1FindingTime;

        public float Rank2FindingTime => _rank2FindingTime;

        public float Rank3FindingTime => _rank3FindingTime;

        public float DecreaseDuration => _decreaseDuration;

        public float WaitDuration => _waitDuration;
    }

}