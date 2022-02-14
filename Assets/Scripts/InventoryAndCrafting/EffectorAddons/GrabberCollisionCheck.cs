using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberCollisionCheck : MonoBehaviour
{
    public GrabberEffector grabberEffector;
    private float pickupBuffer = 15;

    private void OnTriggerStay(Collider other)
    {
        if (pickupBuffer > 0)
        {
            pickupBuffer--;
            return;
        }

        grabberEffector.DropCurrentObj();
    }

    private void Update()
    {
        pickupBuffer -= 1;

        if (pickupBuffer < 0 && grabberEffector == null)
        {
            Destroy(this);
        }
    }
}
