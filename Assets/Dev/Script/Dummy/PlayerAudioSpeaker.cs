using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI;
using IndieLINY.Event;
using UnityEngine;

namespace IndieLINY
{
    public class PlayerAudioSpeaker : AuditorySpeaker
    {
        [SerializeField] private PlayerAudioSpeakerData _data;
        public PlayerAudioSpeakerData SpeakerData => _data;
        private void Update()
        {
            var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            float curRadius = 0f;
            float curRadiusSpeed = 0f;

            if (Input.GetKey(KeyCode.LeftControl) && dir.sqrMagnitude >= 0.001f)
            {
                curRadius = SpeakerData.CrouchRadius;
                curRadiusSpeed = SpeakerData.CrouchRadiusSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftShift) && dir.sqrMagnitude >= 0.001f)
            {
                curRadius = SpeakerData.SprintRadius;
                curRadiusSpeed = SpeakerData.SprintRadiusSpeed;
            }
            else if (dir.sqrMagnitude >= 0.001f)
            {
                curRadius = SpeakerData.WalkRadius;
                curRadiusSpeed = SpeakerData.WalkRadiusSpeed;
            }
            else
            {
                curRadius = 0f;
                curRadiusSpeed = SpeakerData.IdleRadiusSpeed;
            }

            dir.Normalize();
            this.PlayWithNoStop(curRadiusSpeed, curRadius);
        }
    }
}