using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : landmine
{
    public float timer = 2;
    private float countdown;

    public bool triggerPulled = false;

    void Start()
    {
        countdown = timer;
    }

    //Grenade countdown to explode
    void Update()
    {
        if(triggerPulled)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !hasExploded)
            {
                Explode();
                DoDelayExplosion(explosionDelay);
            }
        }
    }
}
