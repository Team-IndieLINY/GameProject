using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;

public class SteminaTest : MonoBehaviour
{
    private SteminaController _steminaController;

    private void Awake()
    {
        _steminaController = GetComponent<SteminaController>();
    }

    private void Update()
    {
        _steminaController.Decrease(ESteminaType.Endurance, 0.001f);
    }
}
