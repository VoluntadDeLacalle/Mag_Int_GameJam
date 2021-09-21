using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
    [Header("Movement Component Variables")]
    public NavMeshAgent nav;
    public ThirdPersonCharacter thirdPersonCharacter;

    [Header("Behavior Component Variables")]
    public EnemyStateMachine enemyStateMachine;
    public EnemyBehavior enemyBehavior;

    void Start()
    {
        nav.updateRotation = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                nav.SetDestination(hit.point);
            }
        }

        if (nav.remainingDistance > nav.stoppingDistance)
        {
            thirdPersonCharacter.Move(nav.desiredVelocity, false, false);
        }
        else
        {
            thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }
}
