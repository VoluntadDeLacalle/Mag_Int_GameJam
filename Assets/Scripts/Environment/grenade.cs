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

    private Color originalColor;
    private bool hasPlayed = false;
    private Renderer rend;
    private SphereCollider sphereCollider;
    private Trajectory trajectory;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rend = GetComponent<Renderer>();
        trajectory = GetComponent<Trajectory>();

        originalColor = rend.material.color;
    }

    void OnEnable()
    {
        countdown = timer;
        ActivateGrenade();
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
                sphereCollider.enabled = true;
                Explode();
            }
        }
    }

    public void ActivateGrenade()
    {
        triggerPulled = true;
    }

    private void OnDisable()
    {
        countdown = timer;
        sphereCollider.enabled = false;
        triggerPulled = false;
        hasPlayed = false;

        ResetExplosive();

        trajectory.trajectoryType = Trajectory.TrajectoryType.none;
    }
}
