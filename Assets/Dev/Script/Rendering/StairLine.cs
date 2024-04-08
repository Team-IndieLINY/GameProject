using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairLine : MonoBehaviour
{
    [SerializeField] private Collider2D[] _enterColliders;
    [SerializeField] private Collider2D[] _exitColliders;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedObject obj))
        {
            foreach (var col in _enterColliders)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), col, false);
            }
            foreach (var col in _exitColliders)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), col, true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out OrderedObject obj))
        {
            foreach (var col in _enterColliders)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), col, true);
            }
            foreach (var col in _exitColliders)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), col, false);
            }
        }
    }
}
