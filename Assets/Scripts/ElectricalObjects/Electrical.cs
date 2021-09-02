using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrical : MonoBehaviour
{
    protected bool isPowered = false;

    public void SetPower(bool shouldPower)
    {
        isPowered = shouldPower;
    }

    public bool IsPowered()
    {
        return isPowered;
    }
}
