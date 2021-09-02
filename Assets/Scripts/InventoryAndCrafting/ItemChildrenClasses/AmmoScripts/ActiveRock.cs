using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRock : MonoBehaviour
{
    public float activeTimer = 0;
    private float maxActiveTimer;

    public Trajectory trajectory;

    private void Awake()
    {
        maxActiveTimer = activeTimer;
    }

    private void Update()
    {
        activeTimer -= Time.deltaTime;
        if (activeTimer <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        activeTimer = maxActiveTimer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Rock has collided with {collision.gameObject.name}");
    }
}
