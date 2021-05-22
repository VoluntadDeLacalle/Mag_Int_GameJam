using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class landmine : Explosion
{
    public string groundTag = "ground";

    //If anything collides this object that is not the ground, the object is set to explode
    private void OnCollisionEnter(Collision collision)
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
            bool isPlayer = other.CompareTag("Player");

            DoDelayExplosion(explosionDelay);
            if (other.CompareTag(bombTag) || isPlayer)
            {
                Explode();

                if (isPlayer)
                {
                    other.GetComponent<PlayerController>().OnTouchMine();
                }
            }
        }
    }
}
