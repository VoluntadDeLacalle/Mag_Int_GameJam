using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PatrolPoint
{
    public Transform patrolTransform;
    public float restTime = 0;
}

public class EnemyBehavior : MonoBehaviour
{
    private Enemy enemy;

    [Header("Basic Variables")]
    public float baseSpeed = 3.5f;
    public float currentSpeed;

    [Header("Patrol Variables")]
    public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    public float patrolSpeed = 0.5f;
    private int currentPatrolPointIndex = 0;
    private bool shouldRest = false;
    private bool hasRested = false;
    private float restTimer = 0;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        currentSpeed = baseSpeed;
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

    private void PatrolRest(float currentRestTime)
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

        enemy.nav.SetDestination(patrolPoints[currentPatrolPointIndex].patrolTransform.position);
    }

    public void Patrol()
    {
        if (shouldRest || patrolPoints.Count == 0)
        {
            return;
        }

        if (enemy.nav.remainingDistance > enemy.nav.stoppingDistance)
        {
            enemy.thirdPersonCharacter.Move(enemy.nav.desiredVelocity, false, false);
        }
        else
        {
            enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);

            if (patrolPoints[currentPatrolPointIndex].restTime != 0 && !hasRested)
            {
                shouldRest = true;
                PatrolRest(patrolPoints[currentPatrolPointIndex].restTime);
            }
            else
            {
                if (currentPatrolPointIndex + 1 == patrolPoints.Count)
                {
                    currentPatrolPointIndex = 0;
                }
                else
                {
                    currentPatrolPointIndex++;
                }

                enemy.nav.SetDestination(patrolPoints[currentPatrolPointIndex].patrolTransform.position);
                hasRested = false;
            }
        }
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

        if (enemy.nav.speed != currentSpeed)
        {
            enemy.nav.speed = currentSpeed;
        }
    }
}
