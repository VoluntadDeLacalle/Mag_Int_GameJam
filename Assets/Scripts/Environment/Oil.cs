using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Player.Instance.IsAlive())
        {
            Player playerCheck = other.gameObject.GetComponent<Player>();
            if (playerCheck != null)
            {
                playerCheck.FallDeath();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Player.Instance.IsAlive())
        {
            Player playerCheck = other.gameObject.GetComponent<Player>();
            if (playerCheck != null)
            {
                playerCheck.FallDeath();
            }
        }
    }
}
