using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private bool isActivated;
    
    public bool startAtTop = false;
    public Transform topTransform;
    public Transform bottomTransform;
    [Range(1,10)]
    public float speed = 3;

    public float floorWaitingTimer = 5f;
    private float maxFloorWaitingTimer = 0;
    
    private bool shouldWait = true;
    private bool atBottomFloor;

    public void ShouldActivate(bool shouldActivate)
    {
        isActivated = shouldActivate;
    }

    private void Awake()
    {
        if (startAtTop)
        {
            topTransform.position = transform.position;
        }
        else
        {
            bottomTransform.position = transform.position;
        }

        atBottomFloor = !startAtTop;
        maxFloorWaitingTimer = floorWaitingTimer;
    }

    private void Update()
    {
        if (!isActivated)
        {
            return;
        }

        if (shouldWait)
        {

            floorWaitingTimer -= Time.deltaTime;
            if (floorWaitingTimer <= 0)
            {
                shouldWait = false;
                floorWaitingTimer = maxFloorWaitingTimer;
            }

            return;
        }

        if (atBottomFloor)
        {
            transform.parent.transform.position = transform.parent.transform.position + Vector3.up * speed * Time.deltaTime;

            if (transform.position.y >= topTransform.position.y)
            {
                shouldWait = true;
                atBottomFloor = false;
            }
        }
        else
        {
            transform.parent.transform.position = transform.parent.transform.position + (-Vector3.up) * speed * Time.deltaTime;

            if (transform.position.y <= bottomTransform.position.y)
            {
                shouldWait = true;
                atBottomFloor = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.parent = this.transform.parent;
    }

    private void OnTriggerExit(Collider other)
    {
        Player playerCheck = other.gameObject.GetComponent<Player>();
        if (playerCheck != null)
        {
            Player.Instance.transform.parent = Player.Instance.rootObj.transform;
        }
    }
}
