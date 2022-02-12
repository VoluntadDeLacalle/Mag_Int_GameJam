using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineNEW : Explosive
{
    [Header("Landmine Variables")]
    public Transform colliderTransform;
    public Vector3 boxBounds;

    public bool turnOnDetectorMat = false;
    public Material detectorMat;

    private void Awake()
    {
        if (turnOnDetectorMat)
        {
            gameObject.GetComponent<Renderer>().material = detectorMat;
        }
    }

    new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(colliderTransform.position, boxBounds * 2);
    }

    void CheckCollisions()
    {
        Collider[] collidersInBox = Physics.OverlapBox(colliderTransform.position, boxBounds);

        if (collidersInBox.Length > 0)
        {
            ActivateExplosion();
        }
    }

    public void ActivateExplosion()
    {
        hasExploded = true;
        Explode();
    }

    private void FixedUpdate()
    {
        if (!hasExploded)
        {
            CheckCollisions();
        }
    }
}
