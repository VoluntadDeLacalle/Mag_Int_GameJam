using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float timer = 2;
    private float countdown;
    private bool hasExploded = false;

    [SerializeField] GameObject explodeParticle;

    // Start is called before the first frame update
    void Start()
    {
        countdown = timer;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0 && !hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explodeParticle, transform.position, transform.rotation);
        hasExploded = true;
    }
}
