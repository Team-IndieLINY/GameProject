using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    [CreateAssetMenu(menuName = "IndieLINY/PlayerData/AudioSpeaker", fileName = "AudioSpeakerData")]
    public class PlayerAudioSpeakerData : ScriptableObject
    {
        [Header("웅크리기 소리 반지름 (meter)")]
        [SerializeField] private float _crouchRadius;
        [Header("달리기 소리 반지름 (meter)")]
        [SerializeField] private float _sprintRadius;
        [Header("걷기 소리 반지름 (meter)")]
        [SerializeField] private float _walkRadius;
        
        [Header("웅크리기 소리 반지름으로 전환되는 시간 (sec)")]
        [SerializeField] private float _crouchRadiusSpeed;
        [Header("달리기 소리 반지름으로 전환되는 시간 (sec)")]
        [SerializeField] private float _sprintRadiusSpeed;
        [Header("걷기 소리 반지름으로 전환되는 시간 (sec)")]
        [SerializeField] private float _walkRadiusSpeed;
        [Header("가만히 있을 때 소리 반지름으로 전환되는 시간 (sec)")]
        [SerializeField] private float _idleRadiusSpeed;

        public float CrouchRadius => _crouchRadius;

        public float SprintRadius => _sprintRadius;

        public float WalkRadius => _walkRadius;

        public float CrouchRadiusSpeed => _crouchRadiusSpeed;

        public float SprintRadiusSpeed => _sprintRadiusSpeed;

        public float WalkRadiusSpeed => _walkRadiusSpeed;

        public float IdleRadiusSpeed => _idleRadiusSpeed;
        
        
    }
}