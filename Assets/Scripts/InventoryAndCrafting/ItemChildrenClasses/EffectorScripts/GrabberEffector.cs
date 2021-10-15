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

            Rigidbody tempRB = null;
            tempRB = collidersInRange[i].gameObject.GetComponentInChildren<Rigidbody>();

            if(tempRB != null)
            {
                if (!tempRB.isKinematic)
                {
                    Grab(tempRB.gameObject, collidersInRange[i], tempRB);
                    return;
                }
            }
        }
    }

    void Grab(GameObject nObj, Collider nCollider, Rigidbody nRB)
    {
        currentAttachedObj = nObj;
        nCollider.enabled = false;

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

        currentAttachedObj.transform.parent = null;
        currentAttachedObj.GetComponent<Collider>().enabled = true;

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
