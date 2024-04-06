using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.Event;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    public float Speed;
    public Rigidbody2D Rigid;

    private void Awake()
    {
        var com = GetComponent<CollisionInteraction>();
        
        com.SetContractInfo(ActorContractInfo.Create(transform, ()=>gameObject), this);
    }

    private void Update()
    {
        Rigid.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * Speed;
    }
}
