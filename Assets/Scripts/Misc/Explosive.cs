using System.Collections;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [Header("Explosion Variables")]
    public float explosionRadius = 3;
    public float explosionForce = 500;
    public float explosionDelay = 0.1f;
    public bool hasExploded = false;
    public string bombTag = "bomb";
    public AudioSource explosionSFX;

    private float destroyDelay = 1f;

    ObjectPooler.Key explosionParticleKey = ObjectPooler.Key.ExplosionParticle;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    //Searches for nearby object in a defined radius and applies a force to those objects
    protected void Explode()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        PlayJuice();

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject == Player.Instance.primaryCollider)
            {
                Player.Instance.Explode(explosionForce, transform.position, explosionRadius);
                continue;
            }

            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        DisableAfterTime(gameObject, destroyDelay);
    }

    private void PlayJuice()
    {
        explosionSFX.Play();
        hasExploded = true;
        GameObject spawnedParticle = ObjectPooler.GetPooler(explosionParticleKey).GetPooledObject();
        spawnedParticle.transform.position = transform.position;
        spawnedParticle.transform.rotation = transform.rotation;
        spawnedParticle.SetActive(true);

        DisableAfterTime(spawnedParticle, 1);
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

        ActivateNearbyExplosives();
    }

    //If an explosive explodes, other bombs in a nearby radius will also explode
    private void ActivateNearbyExplosives()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            if (!hasExploded && collidersInRange[i].gameObject.CompareTag(bombTag))
            {
                DoDelayExplosion(explosionDelay);
                
                if (collidersInRange[i].gameObject.CompareTag(bombTag))
                {
                    Explode();
                }
            }
        }
    }
}
