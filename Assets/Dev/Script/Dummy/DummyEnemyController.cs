using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemyController : MonoBehaviour
{
    public NavMeshAgent Agent;
    public SightPerception SightPerception;
    
    private Vector2 _dir;
    
    private void Update()
    {
        if (Mathf.Approximately(Agent.desiredVelocity.magnitude, 0f))
        {
            return;
        }

        _dir = Agent.desiredVelocity;
        SightPerception.transform.right = _dir;


    }
}
