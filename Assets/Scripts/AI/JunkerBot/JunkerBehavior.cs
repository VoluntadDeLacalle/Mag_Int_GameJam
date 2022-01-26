using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkerBehavior : MonoBehaviour
{
    public JunkerBot junker;

    [Header("Basic Variables")]
    public float baseSpeed = 3.5f;
    public float currentSpeed;
    [HideInInspector] public bool shouldAct = true;

    [Header("Patrol Variables")]
    public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    public float patrolSpeed = 0.5f;
    private int currentPatrolPointIndex = 0;
    public bool shouldRest = false;
    public bool hasRested = false;
    private float restTimer = 0;

    [Header("Attack Variables")]
    public float innerActRadius = 0;
    public float outerActRadius = 0;
    private bool isActing = false;

    [Header("Chase Variables")]
    public float chaseRadius = 0;
    public float chaseSpeed = 1.2f;
    public float updatePositionDist = 0.2f;
    private Vector3 lastKnownPosition;

    private void Awake()
    {
        junker = GetComponent<JunkerBot>();
    }

    void OnDrawGizmosSelected()
    {
        if (junker != null && junker.showActRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, innerActRadius);

            if (outerActRadius <= innerActRadius)
            {
                outerActRadius = innerActRadius + 1;
            }

            Gizmos.color = new Color(1, 0.5f, 0, 1);
            Gizmos.DrawWireSphere(transform.position, outerActRadius);
        }

        if (junker != null && junker.showChaseRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }

    private int FindClosestPatrolPoint()
    {
        if (patrolPoints.Count == 0)
        {
            return -1;
        }

        float minDistance = Vector3.Distance(transform.position, patrolPoints[0].patrolTransform.position);
        int minIndex = 0;

        if (patrolPoints.Count == 1)
        {
            return minIndex;
        }

        for (int i = 1; i < patrolPoints.Count; i++)
        {
            float currentDistance = Vector3.Distance(transform.position, patrolPoints[i].patrolTransform.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                minIndex = i;
            }
        }

        return minIndex;
    }

    /// <summary>
    /// Start of Patrol functions
    /// </summary>

    private void Rest(float currentRestTime)
    {
        restTimer = currentRestTime;
    }

    public void StartPatrolling()
    {
        currentPatrolPointIndex = FindClosestPatrolPoint();
        currentSpeed = patrolSpeed;

        if (currentPatrolPointIndex == -1)
        {
            return;
        }

        junker.nav.SetDestination(patrolPoints[currentPatrolPointIndex].patrolTransform.position);

        shouldRest = false;
        hasRested = false;
    }

    public void Patrol()
    {
        if (shouldRest || patrolPoints.Count == 0)
        {
            return;
        }

        if (junker.nav.remainingDistance < junker.nav.stoppingDistance)
        {
            Debug.Log("Test");
            if (patrolPoints[currentPatrolPointIndex].restTime != 0 && !hasRested && !shouldRest)
            {
                shouldRest = true;
                Rest(patrolPoints[currentPatrolPointIndex].restTime);
            }
            else if ((hasRested && !shouldRest) || patrolPoints[currentPatrolPointIndex].restTime == 0)
            {
                if (currentPatrolPointIndex + 1 == patrolPoints.Count)
                {
                    currentPatrolPointIndex = 0;
                }
                else
                {
                    currentPatrolPointIndex++;
                }

                junker.nav.SetDestination(patrolPoints[currentPatrolPointIndex].patrolTransform.position);
                hasRested = false;

                if (!junker.shouldScoop)
                {
                    junker.shouldScoop = true;
                }
            }
        }
    }

    public void SetLastKnowPosition(Vector3 lastKnowPos)
    {
        lastKnownPosition = lastKnowPos;
        junker.nav.SetDestination(lastKnownPosition);

        currentSpeed = chaseSpeed;

        shouldRest = false;
        hasRested = false;
    }

    public void Chase()
    {
        if (!junker.junkerFOV.FindPlayer())
        {
            junker.stateMachine.switchState(JunkerStateMachine.StateType.Patrol);
            return;
        }
        Vector3 playerPos = Player.Instance.transform.position;

        if (Vector3.Distance(playerPos, lastKnownPosition) > updatePositionDist)
        {
            lastKnownPosition = playerPos;
            junker.nav.SetDestination(lastKnownPosition);
        }
    }

    public void PerformAction()
    {
        junker.anim.SetTrigger("StartAction");
        junker.nav.SetDestination(transform.position);
        junker.nav.isStopped = true;
    }

    private void Update()
    {
        if (shouldRest)
        {

            restTimer -= Time.deltaTime;
            if (restTimer <= 0)
            {
                shouldRest = false;
                hasRested = true;
            }
        }
    }
}
