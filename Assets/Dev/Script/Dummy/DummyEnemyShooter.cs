using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyShooter : MonoBehaviour
{
    public GameObject Prefabs;
    public float Speed;

    public void Shoot(Vector2 direction)
    {
        var obj = Instantiate(Prefabs);
        obj.SetActive(true);

        obj.transform.right = direction;
        obj.AddComponent<DummyEnemyBullet>();
    }
    public void ShootToPoint(Vector2 Point)
    {
        var obj = Instantiate(Prefabs);
        obj.SetActive(true);

        obj.transform.right = Point - (Vector2)transform.position;
        obj.AddComponent<DummyEnemyBullet>().Speed = Speed;
        obj.transform.position = transform.position;
    }
}

public class DummyEnemyBullet : MonoBehaviour
{
    public float Speed;

    private void Awake()
    {
        StartCoroutine(CoUpdate());
    }

    public void Update()
    {
        transform.position += transform.right * Speed * Time.deltaTime;
    }

    private IEnumerator CoUpdate()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}