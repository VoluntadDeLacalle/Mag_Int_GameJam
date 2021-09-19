using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [Header("Component Variables")]
    public Animator anim;
    public vThirdPersonInput vThirdPersonInput;
    public vThirdPersonCamera vThirdPersonCamera;
    public Rigidbody primaryRigidbody;
    public Collider primaryCollider;
    public Health health;

    [Header("Juice Variables")]
    public Ragdoll ragdoll;
    public Transform deathCameraTarget;
    public BackpackFill backpackFill;
    public MagicaCloth.MagicaPhysicsManager clothPhysicsManager;
    public MagicaCloth.MagicaBoneSpring backpackBoneSpring;

    [Header("Personal Player Variables")]

    private bool isAlive = true;
    public Transform origin = null;
    private List<Rigidbody> ragdollRigidbodies = new List<Rigidbody>();
    private List<Collider> ragdollColliders = new List<Collider>();

    private float originalCameraHeight;

    new void Awake()
    {
        base.Awake();

        health.OnHealthDepleated.AddListener(Die);
        health.OnHealthRestored.AddListener(Revived);

        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
        originalCameraHeight = vThirdPersonCamera.height;

        if (origin == null)
        {
            origin = transform;
        }
    }

    void ToggleRagdoll(bool shouldToggle)
    {
        primaryRigidbody.isKinematic = shouldToggle;
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

    private void Revived()
    {
        isAlive = true;
        vThirdPersonCamera.height = originalCameraHeight;
        vThirdPersonCamera.SetTarget(gameObject.transform);
        ToggleRagdoll(false);

        primaryRigidbody.MovePosition(origin.position);
        transform.rotation = origin.rotation;
    }

    private void Die()
    {
        isAlive = false;
        vThirdPersonCamera.height = deathCameraTarget.position.y;
        vThirdPersonCamera.SetTarget(deathCameraTarget);
        ToggleRagdoll(true);

        health.OnHealthDepleated.RemoveAllListeners();
        health.OnHealthDepleated.AddListener(Die);
    }

    private void Update()
    {
        Debug.Log(transform.position);
    }
}
