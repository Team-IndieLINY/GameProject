using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapShowcaseDummy : MonoBehaviour
{
    public Transform A, B;

    public Transform current;

    private void Awake()
    {
        current = B;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            transform.SetParent(current = current == A ? B : A);
            transform.position = current.position;
            var p = transform.position;
            p.z = -10f;
            transform.position = p;
        }
    }
}