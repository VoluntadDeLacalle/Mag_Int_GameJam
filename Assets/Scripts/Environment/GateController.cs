using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public Animator gateAnimator;
    public string openParameter;

    public void OpenGate()
    {
        Debug.Log("Ran");
        gateAnimator.SetBool(openParameter, true);
    }

    public void CloseGate()
    {
        Debug.Log("closed");
        gateAnimator.SetBool(openParameter, false);
    }
}
