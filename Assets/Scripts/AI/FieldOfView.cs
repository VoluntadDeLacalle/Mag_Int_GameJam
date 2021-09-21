using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Enemy enemy;

    [Header("DetectionVariables")]
    public float detectionRadius;
    public LayerMask enemyMask;

    [Header("Field of view Variables")]
    [Range(0, 360)] public float viewAngle;
    public List<Transform> visibleTargets = new List<Transform>();

    [Header("Chase Variable")]
    public float chaseRadius;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectionRadius);

        Gizmos.color = Color.red;
        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }

    public void FindPlayer()
    {
        visibleTargets.Clear();

        List<Transform> targets = Player.Instance.pickup.raycastOrigins;
        for (int i = 0; i < targets.Count; i++)
        {
            float distToTarget = Vector3.Distance(transform.position, targets[i].transform.position);

            if (distToTarget > detectionRadius)
            {
                continue;
            }

            Vector3 dirToTarget = (targets[i].position - transform.position).normalized;
            if (IsTargetInFOV(dirToTarget))
            {
                LayerMask invertedEnemyMask = ~enemyMask;
                RaycastHit hitInfo;

                if ((Physics.Raycast(transform.position, dirToTarget, out hitInfo, distToTarget, invertedEnemyMask)))
                {
                    if (hitInfo.collider != Player.Instance.primaryCollider)
                    {
                        Debug.Log(hitInfo.collider.gameObject.name);
                        continue;
                    }

                    //enemy.enemyStateMachine.switchState(EnemyStateMachine.StateType.Shoot);
                    visibleTargets.Add(targets[i]);
                    break;
                }
            }
        }
    }

    public bool IsTargetInFOV(Vector3 dirToTarget)
    {
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
