using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI
{
    [CreateAssetMenu(menuName = "IndieLINY/AI/EnemyAIMovement", fileName = "EnemyAIMovement")]
    public class EnemyAIMovementData : ScriptableObject
    {
        [Header("기본 상태의 이동 속도 (unit/s)")]
        [SerializeField] private float _NormalMovementSpeed;
        [Header("타겟 추적 상태의 이동 속도 (unit/s)")]
        [SerializeField] private float _TraceMovementSpeed;
        [Header("Rank4 수색 상태의 이동 속도 (unit/s)")]
        [SerializeField] private float _TimeOfRank4MovementSpeed;
        [Header("Rank4 수색 상태 종료까지 걸리는 시간 (sec)")]
        [SerializeField] private float _TimeOfRank4SearchEnd;

        public float NormalMovementSpeed => _NormalMovementSpeed;

        public float TimeOfRank4SearchEnd => _TimeOfRank4SearchEnd;

        public float TraceMovementSpeed => _TraceMovementSpeed;

        public float TimeOfRank4MovementSpeed => _TimeOfRank4MovementSpeed;
    }

}