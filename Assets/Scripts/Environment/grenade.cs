using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : Explosion
{
    [Header("Grenade Variables")]
    public bool triggerPulled = false;
    public float timer = 2;

    [SerializeField]
    private float countdown;

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
