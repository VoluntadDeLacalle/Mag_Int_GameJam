using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : Explosion
{
    public float timer = 2;
    public float explosionDelay = 0.1f;
    private float countdown;

    void Start()
    {
        countdown = timer;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0 && !hasExploded)
        {
            Explode();
            DoDelayExplosion(explosionDelay);
        }
    }

    //Delays the activation of the sphere collider that will detonate any landmines nearby
    void DoDelayExplosion(float delayTime)
    {
        StartCoroutine(DelayExplosion(explosionDelay));
    }

    IEnumerator DelayExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = true;
    }
}
