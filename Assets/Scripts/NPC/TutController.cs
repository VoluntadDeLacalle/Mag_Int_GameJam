using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody primaryRigidbody;
    public Collider primaryCollider;

    public Ragdoll ragdoll;

    public GameObject grabberObj;
    public float grabberRange = 0.2f;
    private Vector3 hitOffset;
    private float hitDistance;
    private Quaternion differenceRotation;
    public GameObject grabbedObj;

    private bool isRagdolled = false;
    private bool grabbed = false;

    [Button("Toggle Ragdoll", "TestRagdoll")]
    [SerializeField] private bool _ragdollBtn;

    [Button("Try Grab", "TestGrab")]
    [SerializeField] private bool _grabBtn;

    private void Awake()
    {
        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = grabberObj.transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f));

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(grabberObj.transform.position, grabberRange);
    }

    public void ToggleRagdoll(bool shouldToggle)
    {
        //if (shouldToggle)
        //{
        //    primaryRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        //    primaryRigidbody.isKinematic = shouldToggle;
        //}
        //else
        //{
        //    primaryRigidbody.isKinematic = shouldToggle;
        //    primaryRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}

        primaryCollider.enabled = !shouldToggle;
        anim.enabled = !shouldToggle;

        ragdoll.ToggleRagdoll(shouldToggle);
    }

    public void TestRagdoll()
    {
        isRagdolled = !isRagdolled;
        ToggleRagdoll(isRagdolled);
        TestGrab();
    }

    public void TestGrab()
    {
        RaycastHit hitInfo;
        Collider[] collidersInRange = Physics.OverlapSphere(grabberObj.transform.position, grabberRange);
        float min = 50;
        if (collidersInRange.Length > 0) 
        {
            GameObject tempGrabbedObj = collidersInRange[0].gameObject;

            for (int i = 0; i < collidersInRange.Length; i++)
            {
                Rigidbody rbCheck = collidersInRange[i].gameObject.GetComponent<Rigidbody>();
                if (rbCheck == null)
                {
                    continue;
                }

                if (Vector3.Distance(collidersInRange[i].gameObject.transform.position, grabberObj.transform.position) < min)
                {
                    min = Vector3.Distance(collidersInRange[i].gameObject.transform.position, grabberObj.transform.position);
                    tempGrabbedObj = collidersInRange[i].gameObject;
                    Debug.Log(collidersInRange[i].gameObject.name);
                }
            }

            Rigidbody secondaryRBCheck = tempGrabbedObj.GetComponent<Rigidbody>();
            if (secondaryRBCheck == null)
            {
                Debug.Log("No Grabbables in range");
                return;
            }

            Debug.Log("popped");
            grabbedObj = tempGrabbedObj;
            FixedJoint grabbedJoint = grabbedObj.AddComponent<FixedJoint>();
            grabbedJoint.connectedBody = grabberObj.GetComponent<Rigidbody>();

            grabbed = true;
        }
    }

    private void FixedUpdate()
    {
        if (isRagdolled && grabbed)
        {
            

            //grabbedObj.transform.position = grabberObj.transform.position + hitOffset;
            //grabbedObj.transform.RotateAround(grabberObj.transform.position, grabberObj.transform.right, 1 * Time.deltaTime);
            //grabbedObj.transform.rotation = Quaternion.Euler((grabberObj.transform.rotation * differenceRotation).eulerAngles);
        }
    }
}
