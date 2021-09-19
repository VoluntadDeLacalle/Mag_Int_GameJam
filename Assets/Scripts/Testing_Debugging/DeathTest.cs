using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTest : MonoBehaviour
{
    [Button("Kill Player", "KillPlayer")] [SerializeField]
    public bool _killBtn;

    [Button("Revive Player", "RevivePlayer")]
    [SerializeField]
    public bool _reviveBtn;


    public void KillPlayer()
    {
        Player.Instance.health.TakeDamage(100);
    }

    public void RevivePlayer()
    {
        Player.Instance.health.FullHeal();
    }
}
