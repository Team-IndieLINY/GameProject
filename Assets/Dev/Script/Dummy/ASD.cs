using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASD : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            SceneManager.LoadScene("BaseCamp");
        }
    }
}
