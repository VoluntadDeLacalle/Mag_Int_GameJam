using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : Explosion
{
    public float timer = 2;
    private float countdown;

    void Start()
    {
        countdown = timer;
    }

    //Grenade countdown to explode
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0 && !hasExploded)
        {
            Explode();
            DoDelayExplosion(explosionDelay);
        }
    }
}
