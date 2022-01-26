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

    [Header("Juice Variables")]
    [Range(1000, 2500)]
    public float playerScoopingForce = 1500;

    [Header("Debugging")]
    public bool showActRadius = true;
    public bool showChaseRadius = true;
    public bool showFOV = true;

    private bool isAlive = true;
    private bool isDead = false;
    [HideInInspector] public bool shouldScoop = true;

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

    public void ScoopPlayer()
    {
        Player.Instance.ragdoll.ExplodeRagdoll(playerScoopingForce, Player.Instance.transform.position, 2f);
    }

    void Update()
    {
        if (junkerScoop.IsPlayerInRange())
        {
            if (stateMachine.GetCurrentState() != JunkerStateMachine.StateType.Act)
            {
                stateMachine.switchState(JunkerStateMachine.StateType.Act);
                Player.Instance.KnockOut();
            }
        }
    }
}
