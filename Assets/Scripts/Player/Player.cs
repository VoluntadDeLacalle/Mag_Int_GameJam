using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [Header("Third Person Component Variables")]
    public vThirdPersonController vThirdPersonController;
    public vThirdPersonInput vThirdPersonInput;
    public vThirdPersonCamera vThirdPersonCamera;

    [Header("Other Component Variables")]
    public Animator anim;
    public Rigidbody primaryRigidbody;
    public Collider primaryCollider;
    public Health health;
    public Pickup pickup;
    public PlayerItemHandler itemHandler;

    [Header("Juice Variables")]
    public Ragdoll ragdoll;
    public Transform deathCameraTarget;
    public BackpackFill backpackFill;
    public MagicaCloth.MagicaPhysicsManager clothPhysicsManager;
    public MagicaCloth.MagicaBoneSpring backpackBoneSpring;

    [Header("Personal Player Variables")]
    public Transform origin = null;

    private bool isAlive = true;

    private float originalCameraHeight;

    new void Awake()
    {
        base.Awake();

        health.OnHealthDepleated.AddListener(Die);
        health.OnHealthRestored.AddListener(Revived);
        health.OnHealthRestored.AddListener(ResetVariables);

        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
        originalCameraHeight = vThirdPersonCamera.height;

        if (origin == null)
        {
            GameObject originPoint = new GameObject("OriginPoint");
            originPoint.transform.parent = transform.root;

            originPoint.transform.position = transform.position;
            originPoint.transform.rotation = transform.rotation;
            origin = originPoint.transform;
        }
    }

    void ToggleRagdoll(bool shouldToggle)
    {
        if (shouldToggle)
        {
            primaryRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            primaryRigidbody.isKinematic = shouldToggle;
        }
        else
        {
            primaryRigidbody.isKinematic = shouldToggle;
            primaryRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        primaryCollider.enabled = !shouldToggle;
        anim.enabled = !shouldToggle;
        clothPhysicsManager.enabled = !shouldToggle;
        backpackBoneSpring.enabled = !shouldToggle;

        ragdoll.ToggleRagdoll(shouldToggle);
    }

    public void Explode(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (Vector3.Distance(transform.position, explosionPosition) > explosionRadius)
        {
            return;
        }

        health.OnHealthDepleated.AddListener(delegate { ragdoll.ExplodeRagdoll(explosionForce, explosionPosition, explosionRadius); });
        health.TakeDamage(100);

        if (itemHandler.GetEquippedItem())
        {
            health.OnHealthDepleated.AddListener(delegate { itemHandler.GetEquippedItem().gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPosition, explosionRadius); });
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void SetNewSpawnPoint(Transform spawnPoint)
    {
        origin = spawnPoint;
    }

    public void Respawn(Transform spawnPoint = null)
    {
        if (spawnPoint != null)
        {
            origin = spawnPoint;
        }

        health.FullHeal();
    }

    public void CantUseChassis()
    {
        anim.SetBool("IsActivated", false);
        anim.SetBool("IsStrafing", false);
    }

    private void ResetVariables()
    {
        anim.SetBool("IsActivated", false);
        anim.SetInteger("GripEnum", -1);

        if (itemHandler.GetEquippedItem() != null)
        {
            itemHandler.GetEquippedItem().gameObject.transform.parent = itemHandler.leftHandAttachmentBone;
            itemHandler.GetEquippedItem().gameObject.transform.localPosition = itemHandler.GetEquippedItem().localHandPos;
            itemHandler.GetEquippedItem().gameObject.transform.localRotation = Quaternion.Euler(itemHandler.GetEquippedItem().localHandRot);
            itemHandler.GetEquippedItem().gameObject.GetComponent<Collider>().enabled = false;
            itemHandler.GetEquippedItem().gameObject.GetComponent<Rigidbody>().isKinematic = true;

            itemHandler.UnequipItem(itemHandler.GetEquippedItem());
        }
    }

    private void Revived()
    {
        isAlive = true;

        vThirdPersonCamera.height = originalCameraHeight;
        vThirdPersonCamera.SetTarget(gameObject.transform);
        ToggleRagdoll(false);

        transform.position = origin.position;
        primaryRigidbody.MovePosition(origin.position);
        transform.rotation = origin.rotation;
    }

    private void Die()
    {
        isAlive = false;

        vThirdPersonCamera.SetTarget(deathCameraTarget);
        if (vThirdPersonController.isGrounded)
        {
            vThirdPersonCamera.height = deathCameraTarget.position.y;
        }
        else
        {
            vThirdPersonCamera.height = deathCameraTarget.position.y - transform.position.y;
        }

        if (itemHandler.GetEquippedItem() != null)
        {
            itemHandler.GetEquippedItem().gameObject.transform.parent = null;
            itemHandler.GetEquippedItem().gameObject.GetComponent<Collider>().enabled = true;
            itemHandler.GetEquippedItem().gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        ToggleRagdoll(true);

        health.OnHealthDepleated.RemoveAllListeners();
        health.OnHealthDepleated.AddListener(Die);
    }
}
