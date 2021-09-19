using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [Header("Component Variables")]
    public Animator anim;
    public vThirdPersonInput vThirdPersonInput;
    public Rigidbody primaryRigidbody;
    public Collider primaryCollider;
    public Health health;
    public BackpackFill backpackFill;

    [Header("Juice Variables")]
    public MagicaCloth.MagicaPhysicsManager clothPhysicsManager;
    public MagicaCloth.MagicaBoneSpring backpackBoneSpring;

    [Header("Personal Player Variables")]

    private bool isAlive = true;
    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();

    new void Awake()
    {
        base.Awake();

        health.OnHealthDepleated.AddListener(Die);
        health.OnHealthRestored.AddListener(Revived);
        GetAllRagdolls();
    }

    void GetAllRagdolls()
    {
        foreach (Rigidbody rb in gameObject.GetComponentsInChildren<Rigidbody>())
        {
            if (rb == primaryRigidbody)
            {
                continue;
            }

            rb.isKinematic = true;
            ragdollRigidbodies.Add(rb);
        }

        foreach (Collider collider in gameObject.GetComponentsInChildren<Collider>())
        {
            if(collider == primaryCollider)
            {
                continue;
            }

            collider.enabled = false;
            ragdollColliders.Add(collider);
        }
    }

    void ToggleRagdoll(bool shouldToggle)
    {
        primaryRigidbody.isKinematic = shouldToggle;
        primaryCollider.enabled = !shouldToggle;
        anim.enabled = !shouldToggle;
        clothPhysicsManager.enabled = !shouldToggle;
        backpackBoneSpring.enabled = !shouldToggle;

        for (int i = 0; i < ragdollColliders.Count; i++)
        {
            ragdollColliders[i].enabled = shouldToggle;
        }

        for(int i = 0; i < ragdollRigidbodies.Count; i++)
        {
            ragdollRigidbodies[i].isKinematic = !shouldToggle;
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    private void Revived()
    {
        isAlive = true;
        ToggleRagdoll(false);
    }

    private void Die()
    {
        Debug.Log("Fuckin, dead B");
        isAlive = false;
        ToggleRagdoll(true);
    }
}
