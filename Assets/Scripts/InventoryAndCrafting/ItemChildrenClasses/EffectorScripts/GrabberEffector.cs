using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberEffector : Item
{
    [Header("Grabber Variables")]
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
            if (Input.GetMouseButtonDown(0) && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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
        if (collidersInRange.Length > 0)
        {
            float min = Mathf.Infinity;
            GameObject tempGrabbedObj = collidersInRange[0].gameObject;
            Collider closestCollider = collidersInRange[0];

            for (int i = 0; i < collidersInRange.Length; i++)
            {
                if (collidersInRange[i].gameObject == this.gameObject || collidersInRange[i].gameObject == Player.Instance.gameObject)
                {
                    continue;
                }

                Rigidbody rbCheck = collidersInRange[i].gameObject.GetComponent<Rigidbody>();
                if (rbCheck == null)
                {
                    continue;
                }

                if (Vector3.Distance(collidersInRange[i].gameObject.transform.position, grabTransform.position) < min)
                {
                    min = Vector3.Distance(collidersInRange[i].gameObject.transform.position, grabTransform.position);
                    tempGrabbedObj = collidersInRange[i].gameObject;
                    closestCollider = collidersInRange[i];
                }
            }

            Rigidbody secondaryRBCheck = tempGrabbedObj.GetComponent<Rigidbody>();
            if (secondaryRBCheck == null)
            {
                Debug.Log("No Grabbables in range");
                return;
            }

            Elevator elevator = secondaryRBCheck.gameObject.GetComponent<Elevator>();
            if (elevator != null)
            {
                return;
            }

            JunkerBot junkerInRange = secondaryRBCheck.gameObject.GetComponent<JunkerBot>();
            if (junkerInRange != null)
            {
                junkerInRange.stateMachine.switchState(JunkerStateMachine.StateType.Disabled);
                junkerInRange.GrabToggle(true);
                junkerInRange.junkerScoop.scoopCollider.enabled = false;

            }

            Grab(tempGrabbedObj, closestCollider, secondaryRBCheck);
            return;
        }
    }

    void Grab(GameObject nObj, Collider nCollider, Rigidbody nRB)
    {
        Quaternion targetRot = Quaternion.identity * Quaternion.Inverse(nObj.transform.rotation);
        Quaternion sourceRot = Quaternion.identity * Quaternion.Inverse(grabTransform.rotation);

        grabTransform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles);

        currentAttachedObj = nObj;
        UnityEngine.Animations.ParentConstraint grabbedConstraint = nObj.AddComponent<UnityEngine.Animations.ParentConstraint>();
        UnityEngine.Animations.ConstraintSource grabberSource = new UnityEngine.Animations.ConstraintSource();
        grabberSource.sourceTransform = grabTransform;
        grabberSource.weight = 1;

        int sourceIndex = grabbedConstraint.AddSource(grabberSource);
        grabbedConstraint.SetTranslationOffset(sourceIndex, nObj.transform.position - grabTransform.position);
        grabbedConstraint.SetRotationOffset(sourceIndex, nObj.transform.rotation.eulerAngles);
        grabbedConstraint.constraintActive = true;

        nCollider.isTrigger = true;
        Physics.IgnoreCollision(nCollider, Player.Instance.GetComponent<Collider>(), true);
        nObj.AddComponent<GrabberCollisionCheck>();
        nObj.GetComponent<GrabberCollisionCheck>().grabberEffector = this;

        nRB.isKinematic = true;
        nRB.velocity = Vector3.zero;
        nRB.angularVelocity = Vector3.zero;
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
            Destroy(currentAttachedObj.GetComponent<UnityEngine.Animations.ParentConstraint>());

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


        Destroy(currentAttachedObj.GetComponent<UnityEngine.Animations.ParentConstraint>());

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
