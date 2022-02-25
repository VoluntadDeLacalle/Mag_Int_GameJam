using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineNEW : Explosive
{
    [Header("Landmine Variables")]
    public Transform colliderTransform;
    public Vector3 boxCenter;
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
        Gizmos.matrix = colliderTransform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, boxBounds * 2);
    }

    void CheckCollisions()
    {
        Collider[] collidersInBox = Physics.OverlapBox(colliderTransform.position, boxBounds, transform.rotation);

        if (collidersInBox.Length > 0)
        {
            for (int i = 0; i < collidersInBox.Length; i++)
            {
                if (collidersInBox[i].gameObject != gameObject)
                {
                    Debug.Log(collidersInBox[i].gameObject.name + ", " + gameObject.name);
                    ActivateExplosion();
                    return;
                }
            }
        }
    }

    public void ActivateExplosion()
    {
        hasExploded = true;
        Explode();
    }

    private void Update()
    {
        if (colliderTransform.position != transform.position + boxCenter)
        {
            colliderTransform.position = transform.position + boxCenter;
        }
    }

    private void FixedUpdate()
    {
        if (!hasExploded)
        {
            CheckCollisions();
        }
    }
}
