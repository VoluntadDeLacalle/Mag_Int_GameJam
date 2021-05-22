using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landmine : Explosion
{
    public float explosionDelay = 0.1f;

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
        if(!hasExploded)
        {
            DoDelayExplosion(explosionDelay);
            if (other.CompareTag(bombTag))
            {
                Explode();
            }
        }
    }


    //Delays the activation of the sphere collider that will detonate any landmines nearby
    void DoDelayExplosion(float delayTime)
    {
        StartCoroutine(DelayExplosion(explosionDelay));
    }

    IEnumerator DelayExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        //Do the action after the delay time has finished.
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = true;
    }
}
