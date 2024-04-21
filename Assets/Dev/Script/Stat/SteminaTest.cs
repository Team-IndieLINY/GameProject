using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;

public class SteminaTest : MonoBehaviour
{
    private StatController _statController;

    private void Awake()
    {
        _statController = GetComponent<StatController>();
    }

    private void Update()
    {
        _statController.Decrease(1);
    }
}
