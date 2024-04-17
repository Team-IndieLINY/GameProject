using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemyController : MonoBehaviour
{
    public NavMeshAgent Agent;
    public SightPerception SightPerception;
    public Renderer _viewVisualizer;

    private void Update()
    {
        var dir = SightPerception.Direction;
    }
}