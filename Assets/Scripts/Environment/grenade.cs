using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : Explosion
{
    [Header("Grenade Variables")]
    public bool triggerPulled = false;
    public float timer = 2;
    public AudioSource triggerPullSFX;
    Color activatedcolor = Color.red;

    [SerializeField]
    private float countdown;

    private bool hasPlayed = false;
    private Renderer rend;

    void Start()
    {
        countdown = timer;
        rend = GetComponent<Renderer>();
    }

    //Grenade countdown to explode
    void Update()
    {
        if(triggerPulled)
        {
            //hack for soundbite of grenade. Fix later with an audiomanager
            if(!hasPlayed)
            {
                triggerPullSFX.Play();
                rend.material.color = activatedcolor;
            }


            countdown -= Time.deltaTime;
            if (countdown <= 0 && !hasExploded)
            {
                SphereCollider sphereCollider = GetComponent<SphereCollider>();
                sphereCollider.enabled = true;
                Explode();
            }
        }
    }

    public void activateGrenade()
    {
        triggerPulled = true;
    }
}
