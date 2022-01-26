using Invector.vCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>, ISaveable
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

    [Header("Death Variables")]
    public PanelComponentFade panelFade;
    public float deathTime = 3;

    [Header("Audio Variables")]
    public string leftStepSFX = string.Empty;
    public string rightStepSFX = string.Empty;
    public string deathSFX = string.Empty;

    [Header("Ragdoll Variables")]
    public Ragdoll ragdoll;
    public Transform deathCameraTarget;
    public MagicaCloth.MagicaPhysicsManager clothPhysicsManager;
    public MagicaCloth.MagicaBoneSpring backpackBoneSpring;

    [Header("Juice Variables")]
    public BackpackFill backpackFill;
    public Emoter playerEmoter;

    [Header("Personal Player Variables")]
    public Transform origin = null;

    private bool isAlive = true;

    private float originalCameraHeight;

    public object CaptureState()
    {
        return new SaveData
        {
            backpackFillSizeWeights = backpackFill.GetBlendWeights()
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        backpackFill.SetBlendWeights(saveData.backpackFillSizeWeights);
    }

    [Serializable]
    private struct SaveData
    {
        public List<float> backpackFillSizeWeights;
    }

    new void Awake()
    {
        base.Awake();

        health.OnHealthDepleated.AddListener(Die);
        health.OnHealthRestored.AddListener(Revived);
        health.OnHealthRestored.AddListener(ResetVariables);

        ragdoll.GetAllRagdolls(primaryRigidbody, primaryCollider);
        originalCameraHeight = vThirdPersonCamera.height;

        playerEmoter.InitEmoter(anim, true);

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

    public void KnockOut()
    {
        if (!ragdoll.IsRagdolled())
        {
            vThirdPersonCamera.SetTarget(deathCameraTarget);
            if (vThirdPersonController.isGrounded)
            {
                vThirdPersonCamera.height = deathCameraTarget.localPosition.y;
            }
            else
            {
                vThirdPersonCamera.height = deathCameraTarget.localPosition.y;
            }

            vThirdPersonInput.ShouldMove(false);
            
            ToggleRagdoll(true);
            StartCoroutine(RegainConsciousnessTime());
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void SetNewSpawnPoint(Transform spawnPoint)
    {
        origin.position = spawnPoint.position;
        origin.rotation = spawnPoint.rotation;
    }

    public void Spawn(Transform spawnPoint = null)
    {
        if (spawnPoint != null)
        {
            origin.position = spawnPoint.position;
            origin.rotation = spawnPoint.rotation;
        }

        Revived();
    }

    public void Respawn(Transform spawnPoint = null)
    {
        if (spawnPoint != null)
        {
            origin.position = spawnPoint.position;
            origin.rotation = spawnPoint.rotation;
        }

        health.FullHeal();
    }

    public void RegainConsciousness()
    {
        vThirdPersonCamera.height = originalCameraHeight;
        vThirdPersonCamera.SetTarget(gameObject.transform);
        ToggleRagdoll(false);

        transform.position = ragdoll.ragdollColliders[0].transform.position;
        primaryRigidbody.MovePosition(ragdoll.ragdollColliders[0].transform.position);

        vThirdPersonCamera.transform.LookAt(deathCameraTarget);
        vThirdPersonCamera.SetTarget(gameObject.transform);

        vThirdPersonInput.ShouldMove(true);
    }

    public void CantUseChassis()
    {
        anim.SetBool("IsActivated", false);
        anim.SetBool("IsStrafing", false);
    }

    public void PlayerStopMove()
    {
        anim.ResetTrigger("PickupTrigger");
        vThirdPersonInput.ShouldMove(false);
    }

    public void PlayerStartMove()
    {
        vThirdPersonInput.ShouldMove(true);
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

        vThirdPersonCamera.transform.LookAt(deathCameraTarget);
        vThirdPersonCamera.SetTarget(gameObject.transform);
    }

    private void Die()
    {
        isAlive = false;

        if (deathSFX != string.Empty)
        {
            //AudioManager.Get().Play(deathSFX);
        }

        vThirdPersonCamera.SetTarget(deathCameraTarget);
        if (vThirdPersonController.isGrounded)
        {
            vThirdPersonCamera.height = deathCameraTarget.localPosition.y;
        }
        else
        {
            vThirdPersonCamera.height = deathCameraTarget.localPosition.y;
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

        StartCoroutine(StartFade(RespawnTime()));
    }

    IEnumerator StartFade(IEnumerator RespawnType)
    {
        yield return new WaitForSeconds(deathTime);

        panelFade.FadeOutAndIn();
        StartCoroutine(RespawnType);
    }

    IEnumerator RegainConsciousnessTime()
    {
        yield return new WaitForSeconds(deathTime * 1.5f);

        RegainConsciousness();
    }

    IEnumerator RespawnTime()
    {
        yield return new WaitForSeconds(panelFade.duration);

        Respawn();
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (QuestManager.Instance.IsCurrentQuestActive())
        {
            Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
            if (currentObjective != null)
            {
                currentObjective.CheckLocation(transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && vThirdPersonController.inputMagnitude < 0.1f && vThirdPersonInput.CanMove())
        {
            playerEmoter.PlayEmote("EmoteTrigger");
        }
    }
}
