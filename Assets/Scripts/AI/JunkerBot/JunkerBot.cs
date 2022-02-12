using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JunkerBot : MonoBehaviour
{
    [Header("Movement Component Variables")]
    public NavMeshAgent nav;

    [Header("Behavior Component Variables")]
    public JunkerStateMachine stateMachine;
    public JunkerBehavior behavior;
    public JunkerFOV junkerFOV;

    [Header("Other Component Variables")]
    public LayerMask junkerMask;
    public Animator anim;
    public JunkerScoop junkerScoop;
    public Rigidbody primaryRigidbody;
    public Collider primaryCollider;
    public Health health;

    [Header("Disabled Variables")]
    public GameObject rootObject;
    public float disabledTimer = 5f;
    private float maxDisabledTimer = 0f;

    [Header("Juice Variables")]
    [Range(1000, 2500)]
    public float playerScoopingForce = 1500;
    public float deathTimer = 2f;

    [Header("Debugging")]
    public bool showActRadius = true;
    public bool showChaseRadius = true;
    public bool showFOV = true;

    private bool isAlive = true;
    private bool isDead = false;
    private bool isDisabled = false;
    private bool isGrabbed = false;
    [HideInInspector] public bool shouldScoop = true;

    private void Awake()
    {
        maxDisabledTimer = disabledTimer;

        health.OnHealthDepleated.AddListener(Die);
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public void StopAction()
    {
        anim.ResetTrigger("StartAction");
        nav.isStopped = false;

        shouldScoop = false;
        stateMachine.switchState(JunkerStateMachine.StateType.Patrol);

        junkerScoop.SetPlayerInRange(false);
    }

    public void ResetDisabledTimer()
    {
        disabledTimer = maxDisabledTimer;
    }

    public void GrabToggle(bool gotGrabbed)
    {
        isGrabbed = gotGrabbed;
    }

    public void ScoopPlayer()
    {
        Player.Instance.ragdoll.ExplodeRagdoll(playerScoopingForce, Player.Instance.transform.position, 2f);
    }

    public void ToggleActive(bool isActive)
    {
        primaryRigidbody.isKinematic = isActive;

        if (isActive)
        {
            nav.enabled = isActive;
            nav.isStopped = !isActive;
        }
        else
        {
            nav.isStopped = !isActive;
            nav.enabled = isActive;
        }

        isDisabled = !isActive;
        anim.SetBool("IsDisabled", !isActive);

        if (isActive)
        {
            gameObject.layer = transform.GetChild(0).gameObject.layer;
        }
        else
        {
            gameObject.layer = 0;
        }
    }

    public void KillJunker()
    {
        health.TakeDamage(100);
    }

    private void Die()
    {
        isAlive = false;
        isDead = true;

        health.OnHealthDepleated.RemoveAllListeners();
        health.OnHealthDepleated.AddListener(Die);
    }

    void Update()
    {
        if (isDead)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0)
            {
                primaryCollider.enabled = false;
                gameObject.SetActive(false);
            }
        }

        if (junkerScoop.IsPlayerInRange())
        {
            if (stateMachine.GetCurrentState() != JunkerStateMachine.StateType.Act && !isDisabled)
            {
                stateMachine.switchState(JunkerStateMachine.StateType.Act);
                Player.Instance.KnockOut();
            }
        }

        if (isDisabled && !isGrabbed)
        {

            disabledTimer -= Time.deltaTime;
            if (disabledTimer <= 0)
            {
                ResetDisabledTimer();
                ToggleActive(true);

                if (stateMachine.GetCurrentState() == JunkerStateMachine.StateType.Disabled)
                {
                    stateMachine.switchState(JunkerStateMachine.StateType.Patrol);
                }
            }
        }
    }
}
