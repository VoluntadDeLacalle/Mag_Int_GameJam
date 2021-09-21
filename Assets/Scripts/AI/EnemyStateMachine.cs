using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private Enemy enemy;

    public enum StateType
    {
        Patrol,
        Shoot,
        Chase,
        LostPlayer
    }

    public StateType state = StateType.Patrol;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        OnStateEnter();
    }

    void Update()
    {
        ///ADD CODE FOR CHECKING IF ENEMY IS ALIVE!!!
        if (Player.Instance == null)
        {
            return;
        }

        if (Player.Instance.IsAlive())
        {
            switch (state)
            {
                case StateType.Patrol:

                    enemy.enemyBehavior.Patrol();
                    break;
                case StateType.Shoot:
                    
                    break;
                case StateType.Chase:
                    //enemy.Chasing();

                    break;
                case StateType.LostPlayer:

                    //enemy.playerSearch();
                    break;
            }
        }
    }

    public void switchState(StateType newState)
    {
        if (state == newState)
        {
            return;
        }

        state = newState;
        OnStateEnter();
    }

    public void OnStateEnter()
    {
        switch (state)
        {
            case StateType.Patrol:
                enemy.enemyBehavior.StartPatrolling();

                break;
            case StateType.Shoot:

                break;
            case StateType.Chase:

                break;
            case StateType.LostPlayer:

                break;
        }
    }
}
