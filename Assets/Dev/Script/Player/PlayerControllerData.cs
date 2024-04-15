using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY
{
    [CreateAssetMenu(menuName = "IndieLINY/PlayerData/ControllerData", fileName = "ControllerData")]
    public class PlayerControllerData : ScriptableObject
    {
        [SerializeField] private float _worldInteractionRadius;
        [SerializeField] private float _crouchSpeed;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _walkSpeed;

        [SerializeField] private float _decreaseEndurancePerSec;

        public float DecreaseEndurancePerSec => _decreaseEndurancePerSec;

        public float WorldInteractionRadius => _worldInteractionRadius;

        public float CrouchSpeed => _crouchSpeed;

        public float SprintSpeed => _sprintSpeed;

        public float WalkSpeed => _walkSpeed;
    }

}