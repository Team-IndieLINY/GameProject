using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI;
using IndieLINY.Event;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public Rigidbody2D Rigid;

    public float CrouchRadius;
    public float SprintRadius;
    public float WalkRadius;

    public float CrouchRadiusSpeed;
    public float SprintRadiusSpeed;
    public float WalkRadiusSpeed;
    public float IdleRadiusSpeed;


    public float CrouchSpeed;
    public float SprintSpeed;
    public float WalkSpeed;

    public AuditorySpeaker Speaker;

    private void Awake()
    {
        var com = GetComponent<CollisionInteraction>();
        
        com.SetContractInfo(ActorContractInfo.Create(transform, ()=>gameObject), this);
    }

    private void Update()
    {
        var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float curSpeed = 0f;
        float curRadius = 0f;
        float curRadiusSpeed = 0f;

        if (Input.GetKey(KeyCode.LeftControl) && dir.sqrMagnitude >= 0.001f)
        {
            curSpeed = CrouchSpeed;
            curRadius = CrouchRadius;
            curRadiusSpeed = CrouchRadiusSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && dir.sqrMagnitude >= 0.001f)
        {
            curSpeed = SprintSpeed;
            curRadius = SprintRadius;
            curRadiusSpeed = SprintRadiusSpeed;
        }
        else if(dir.sqrMagnitude >= 0.001f)
        {
            curSpeed = WalkSpeed;
            curRadius = WalkRadius;
            curRadiusSpeed = WalkRadiusSpeed;
        }
        else
        {
            curSpeed = 0f;
            curRadius = 0f;
            curRadiusSpeed = IdleRadiusSpeed;
        }

        dir.Normalize();
        Rigid.velocity = dir * curSpeed;
        Speaker.PlayWithNoStop( curRadiusSpeed, curRadius);
    }
}
