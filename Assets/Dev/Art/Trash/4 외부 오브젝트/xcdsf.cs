using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xcdsf : MonoBehaviour
{
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        dir = dir.normalized;

        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }
}
