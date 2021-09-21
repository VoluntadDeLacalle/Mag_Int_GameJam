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
    public bool shouldAct = true;

    [Header("Patrol Variables")]
    public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    public float patrolSpeed = 0.5f;
    private int currentPatrolPointIndex = 0;
    private bool shouldRest = false;
    private bool hasRested = false;
    private float restTimer = 0;

    [Header("Attack Variables")]
    public float innerAttackRadius = 0;
    public float outerAttackRadius = 0;
    public float attackTimer = 1.2f;
    private bool isAttacking = false;
    private float maxAttackTimer = 0;

    [Header("Chase Variables")]
    public float chaseRadius = 0;
    public float chaseSpeed = 1.2f;
    public float updatePositionDist = 0.2f;
    private Vector3 lastKnownPosition;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        currentSpeed = baseSpeed;
        maxAttackTimer = attackTimer;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerAttackRadius);

        if (outerAttackRadius <= innerAttackRadius)
        {
            outerAttackRadius = innerAttackRadius + 1;
        }

        Gizmos.color = new Color(1, 0.5f, 0, 1);
        Gizmos.DrawWireSphere(transform.position, outerAttackRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
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

    /// <summary>
    /// Start of Attack Functions
    /// </summary>

    public void StartAttack()
    {
        isAttacking = true;
        attackTimer = maxAttackTimer;

        enemy.nav.SetDestination(transform.position);

    }

    public void Attack()
    {
        Vector3 playerPos = Player.Instance.transform.position;

        if (!isAttacking)
        {
            if (!enemy.enemyFOV.FindPlayer())
            {
                enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
                isAttacking = false;
                return;
            }

            if (Vector3.Distance(playerPos, transform.position) > innerAttackRadius)
            {
                enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.Chase);
                isAttacking = false;
                return;
            }

            isAttacking = true;
        }

        enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);
    }

    /// <summary>
    /// Start of Chase functions
    /// </summary>

    public void SetLastKnowPosition(Vector3 lastKnowPos)
    {
        lastKnownPosition = lastKnowPos;
        enemy.nav.SetDestination(lastKnownPosition);

        currentSpeed = chaseSpeed;
    }

    public void Chase()
    {
        if (!enemy.enemyFOV.FindPlayer())
        {
            enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }
        Vector3 playerPos = Player.Instance.transform.position;

        if (Vector3.Distance(playerPos, transform.position) < innerAttackRadius)
        {
            enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.Attack);
            return;
        }

        if (Vector3.Distance(playerPos, lastKnownPosition) > updatePositionDist)
        {
            lastKnownPosition = playerPos;
            enemy.nav.SetDestination(lastKnownPosition);
        }

        if (enemy.nav.remainingDistance > enemy.nav.stoppingDistance)
        {
            enemy.thirdPersonCharacter.Move(enemy.nav.desiredVelocity, false, false);
        }
        else
        {
            enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }

    /// <summary>
    /// Start of Lost Player Functions
    /// </summary>

    public void LostPlayer()
    {
        if (enemy.nav.remainingDistance > enemy.nav.stoppingDistance)
        {
            enemy.thirdPersonCharacter.Move(enemy.nav.desiredVelocity, false, false);
        }
        else
        {
            enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);
            enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.Patrol);
        }
    }

    /// <summary>
    /// Update
    /// </summary>

    private void Update()
    {
        if (!Player.Instance.IsAlive())
        {
            return;
        }

        if (!enemy.IsAlive())
        {
            if (enemy.nav.enabled)
            {
                if (!enemy.nav.isStopped || enemy.nav.hasPath)
                {
                    enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);
                    enemy.nav.isStopped = true;
                }
                return;
            }

            return;
        }

        //Update nav speed.
        if (enemy.nav.speed != currentSpeed)
        {
            enemy.nav.speed = currentSpeed;
        }

        //Checks if enemy is resting on current patrol position.
        if (shouldRest)
        {
            enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);

            restTimer -= Time.deltaTime;
            if (restTimer <= 0)
            {
                shouldRest = false;
                hasRested = true;
            }
        }

        //Charges up an attack and releases it once the timer hits zero.
        if (isAttacking)
        {
            enemy.thirdPersonCharacter.Move(Vector3.zero, false, false);

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Debug.Log("Attack!");
                enemy.anim.SetBool("IsAttacking", true);

                Vector3 playerPos = Player.Instance.transform.position;
                if (Vector3.Distance(playerPos, transform.position) < outerAttackRadius)
                {
                    Player.Instance.Explode(1000, transform.position, outerAttackRadius);
                    shouldAct = false;
                }

                isAttacking = false;
                attackTimer = maxAttackTimer;
            }
        }
    }
}
