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

    [Button("Toggle Ragdoll", "TestRagdoll")]
    [SerializeField] private bool _ragdollBtn;

    [Button("Try Grab", "TestGrab")]
    [SerializeField] private bool _grabBtn;

    private void Awake()
    {
        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
    }

    public void ToggleRagdoll(bool shouldToggle)
    {
        primaryCollider.enabled = !shouldToggle;
        anim.enabled = !shouldToggle;

        ragdoll.ToggleRagdoll(shouldToggle);
    }
}
