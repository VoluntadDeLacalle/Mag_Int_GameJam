using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float timer = 2;
    public float radius = 3;
    public float force = 500;
    private float countdown;
    private bool hasExploded = false;

    [SerializeField] GameObject explodeParticle;

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
        }
    }

    //Searches for nearby object in a defined radius and applies a force to those objects
    private void Explode()
    {
        GameObject spawnedParticle = Instantiate(explodeParticle, transform.position, transform.rotation);
        Destroy(spawnedParticle, 2);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        hasExploded = true;
        Destroy(gameObject);
    }
}
