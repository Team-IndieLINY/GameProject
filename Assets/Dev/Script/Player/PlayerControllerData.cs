using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    [CreateAssetMenu(menuName = "IndieLINY/PlayerData/ControllerData", fileName = "ControllerData")]
    public class PlayerControllerData : ScriptableObject
    {
        [Header("월드의 오브젝트와 상호작용할 수 있는 범위(meter)")]
        [SerializeField] private float _worldInteractionRadius;
        [Header("웅크리기 이동 시간 (unit/s)")]
        [SerializeField] private float _crouchSpeed;
        [Header("웅크리기 이동 시간 (unit/s)")]
        [SerializeField] private float _sprintSpeed;
        [Header("웅크리기 이동 시간 (unit/s)")]
        [SerializeField] private float _walkSpeed;

        [Header("지구력이 0이 되는데 걸리는 시간 (sec)")]
        [SerializeField] private float _decreaseEndurancePerSec;
        [Header("지구력이 최대가 되는데 걸리는 시간 (sec)")]
        [SerializeField] private float _increaseEndurancePerSec;

        public float DecreaseEndurancePerSec => _decreaseEndurancePerSec;

        public float IncreaseEndurancePerSec => _increaseEndurancePerSec;

        public float WorldInteractionRadius => _worldInteractionRadius;

        public float CrouchSpeed => _crouchSpeed;

        public float SprintSpeed => _sprintSpeed;

        public float WalkSpeed => _walkSpeed;
    }

}