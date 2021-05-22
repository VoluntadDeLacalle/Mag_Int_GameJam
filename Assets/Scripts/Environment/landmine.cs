using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landmine : Explosion
{
    public string groundTag = "ground";

    //If anyting touches this object that is not the ground, the object is set to explode
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(groundTag))
        {
            if (!hasExploded)
            {
                SphereCollider sphereCollider = GetComponent<SphereCollider>();
                sphereCollider.enabled = true;

                Explode();
            }
        }
    }

    //If a landmine explodes, other bombs in a nearby radius will also explode
    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded)
        {
            DoDelayExplosion(explosionDelay);
            if (other.CompareTag(bombTag) || other.CompareTag("Player"))
            {
                Explode();
            }
        }
    }
}
