using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public Animator gateAnimator;

    public void OpenGate()
    {
        Debug.Log("Ran");
        gateAnimator.SetBool("IsOpened", true);
    }

    public void CloseGate()
    {
        Debug.Log("closed");
        gateAnimator.SetBool("IsOpened", false);
    }
}
