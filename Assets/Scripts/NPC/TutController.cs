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

    private bool isRagdolled = false;

    [Button("Toggle Ragdoll", "TestRagdoll")]
    [SerializeField] private bool _ragdollBtn;

    private void Awake()
    {
        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
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
    }
}
