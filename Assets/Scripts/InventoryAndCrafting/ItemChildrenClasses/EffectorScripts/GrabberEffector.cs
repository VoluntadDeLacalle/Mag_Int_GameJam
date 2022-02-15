using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberEffector : Item
{
    public Transform grabTransform;
    public float grabRadius = 0;

    public GameObject currentAttachedObj = null;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabTransform.position, grabRadius);
    }

    public override void Activate()
    {
        if (currentAttachedObj == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryGrab();

                if (QuestManager.Instance.IsCurrentQuestActive())
                {
                    Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                    if (currentObjective != null)
                    {
                        currentObjective.ActivateItem(itemName);
                    }
                }
            }
        }
        else
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetMouseButtonDown(0))
            {
                DropCurrentObj();
            }
        }
    }

    void TryGrab()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(grabTransform.position, grabRadius);

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            if (collidersInRange[i].gameObject == this.gameObject || collidersInRange[i].gameObject == Player.Instance.gameObject)
            {
                continue;
            }

            MeshCollider meshCheck = collidersInRange[i].gameObject.GetComponent<MeshCollider>();
            if (meshCheck != null)
            {
                if(meshCheck == collidersInRange[i])
                {
                    continue;
                }
            }

            Rigidbody tempRB = null;
            tempRB = collidersInRange[i].gameObject.GetComponentInChildren<Rigidbody>();

            if(tempRB != null)
            {
                Elevator elevator = collidersInRange[i].gameObject.GetComponent<Elevator>();
                if (elevator != null)
                {
                    continue;
                }

                JunkerBot junkerInRange = collidersInRange[i].gameObject.GetComponent<JunkerBot>();
                if (junkerInRange != null)
                {
                    junkerInRange.stateMachine.switchState(JunkerStateMachine.StateType.Disabled);
                    junkerInRange.GrabToggle(true);
                    junkerInRange.junkerScoop.scoopCollider.enabled = false;

                    Grab(junkerInRange.gameObject, collidersInRange[i], tempRB);
                    return;
                }

                Grab(tempRB.gameObject, collidersInRange[i], tempRB);
                return;
            }
        }
    }

    void Grab(GameObject nObj, Collider nCollider, Rigidbody nRB)
    {
        currentAttachedObj = nObj;
        nCollider.isTrigger = true;
        Physics.IgnoreCollision(nCollider, Player.Instance.GetComponent<Collider>(), true);
        nObj.AddComponent<GrabberCollisionCheck>();
        nObj.GetComponent<GrabberCollisionCheck>().grabberEffector = this;

        nRB.isKinematic = true;
        nRB.velocity = Vector3.zero;
        nRB.angularVelocity = Vector3.zero;

        nObj.transform.parent = this.gameObject.transform;
    }

    public GameObject DropCurrentObj()
    {
        if (currentAttachedObj == null)
        {
            return null;
        }

        JunkerBot tempJunker = currentAttachedObj.GetComponentInChildren<JunkerBot>();
        if (tempJunker != null)
        {
            currentAttachedObj.transform.parent = null;
            tempJunker.gameObject.transform.parent = tempJunker.rootObject.transform;
            tempJunker.primaryCollider.isTrigger = false;
            Physics.IgnoreCollision(tempJunker.primaryCollider, Player.Instance.primaryCollider, false);
            Destroy(tempJunker.GetComponent<GrabberCollisionCheck>());

            tempJunker.junkerScoop.scoopCollider.enabled = true;

            tempJunker.primaryRigidbody.isKinematic = false;
            tempJunker.primaryRigidbody.velocity = Vector3.zero;
            tempJunker.primaryRigidbody.angularVelocity = Vector3.zero;

            tempJunker.GrabToggle(false);
            currentAttachedObj = null;
            return tempJunker.gameObject;
        }

        currentAttachedObj.transform.parent = null;
        currentAttachedObj.GetComponent<Collider>().isTrigger = false;
        Physics.IgnoreCollision(currentAttachedObj.GetComponent<Collider>(), Player.Instance.primaryCollider, false);
        Destroy(currentAttachedObj.GetComponent<GrabberCollisionCheck>());

        currentAttachedObj.GetComponent<Rigidbody>().isKinematic = false;
        currentAttachedObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        currentAttachedObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GameObject tempGO = currentAttachedObj;
        currentAttachedObj = null;
        return tempGO;
    }

    public override void OnUnequip()
    {
        if (currentAttachedObj != null)
        {
            DropCurrentObj();
        }
    }

    void Update()
    {
        if (itemType != TypeTag.effector)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not effector!");
        }
    }
}
