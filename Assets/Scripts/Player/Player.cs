using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public BackpackFill backpackFill;
    public vThirdPersonInput vThirdPersonInput;
    public Health health;

    new void Awake()
    {
        base.Awake();

        health.OnHealthDepleated.AddListener(Die);
    }

    private void Die()
    {
        Debug.Log("Fuckin, dead B");
    }
}
