using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Variables")]
    public float radius = 3;
    public float force = 500;
    public float explosionDelay = 0.1f;
    public bool hasExploded = false;
    public string bombTag = "bomb";
    public AudioSource explosionSFX;

    private float destroyDelay = 1f;

    ObjectPooler.Key explosionParticleKey = ObjectPooler.Key.ExplosionParticle;

    //Searches for nearby object in a defined radius and applies a force to those objects
    protected void Explode()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        explosionSFX.Play();
        hasExploded = true;
        GameObject spawnedParticle = ObjectPooler.GetPooler(explosionParticleKey).GetPooledObject();
        spawnedParticle.transform.position = transform.position;
        spawnedParticle.transform.rotation = transform.rotation;
        spawnedParticle.SetActive(true);

        DisableAfterTime(spawnedParticle, 1);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        //gameObject.SetActive(false);
        DisableAfterTime(gameObject, destroyDelay);
    }

    protected void ResetExplosive()
    {
        GetComponent<MeshRenderer>().enabled = true;
        hasExploded = false;
    }

    void DisableAfterTime(GameObject objectToDisable, float time = 0)
    {
        StartCoroutine(DisableEnum(time, objectToDisable));
    }

    IEnumerator DisableEnum(float disableTime, GameObject objectToDisable)
    {
        yield return new WaitForSeconds(disableTime);

        objectToDisable.SetActive(false);
    }

    //Delays the activation of the sphere collider that will detonate any explosives nearby
    protected void DoDelayExplosion(float delayTime)
    {
        StartCoroutine(DelayExplosion(explosionDelay));
    }

    IEnumerator DelayExplosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = true;
    }

    //If an explosive explodes, other bombs in a nearby radius will also explode
    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded && other.CompareTag(bombTag))
        {
            DoDelayExplosion(explosionDelay);
            if (other.CompareTag(bombTag))
            {
                Explode();
            }
        }
    }
}
