using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landmine : Explosion
{
    //If anyting touches this object the landmind is set to explode
    private void OnCollisionEnter(Collision collision)
    {
        if(!hasExploded)
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.enabled = true;

            Explode();
        }
    }

    //If a landmine explodes other landmines in a nearby radius will also explode
    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded)
        {
            DoDelayExplosion(explosionDelay);
            if (other.CompareTag(bombTag))
            {
                Explode();
            }
        }
    }
}
