using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 3;
    public float force = 500;
    public bool hasExploded = false;
    public string bombTag = "bomb";

    private float destroyDelay = 0.2f;

    [SerializeField] GameObject explodeParticle;

    //Searches for nearby object in a defined radius and applies a force to those objects
    public void Explode()
    {
        hasExploded = true;
        GameObject spawnedParticle = Instantiate(explodeParticle, transform.position, transform.rotation);
        Destroy(spawnedParticle, 1);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        Destroy(gameObject, destroyDelay);
    }
}
