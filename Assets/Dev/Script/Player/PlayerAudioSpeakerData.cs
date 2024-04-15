using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    [CreateAssetMenu(menuName = "IndieLINY/PlayerData/AudioSpeaker", fileName = "AudioSpeakerData")]
    public class PlayerAudioSpeakerData : ScriptableObject
    {
        [Header("소리 반지름")]
        [SerializeField] private float _crouchRadius;
        [SerializeField] private float _sprintRadius;
        [SerializeField] private float _walkRadius;
        
        [Header("각 소리 반지름으로 전환되는 속도")]
        [SerializeField] private float _crouchRadiusSpeed;
        [SerializeField] private float _sprintRadiusSpeed;
        [SerializeField] private float _walkRadiusSpeed;
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