using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleAnimator : SingletonMonoBehaviour<PlayerIdleAnimator>
{
    public Animator animator;
    public Dictionary<string, KeyValuePair<Vector3, Quaternion>> idleTransforms = new Dictionary<string, KeyValuePair<Vector3, Quaternion>>();

    new void Awake()
    {
        base.Awake();

        animator.speed = 0;
    }

    public void SwitchPlayerAnimLayer(int index)
    {
        animator.SetFloat("Blend", index);
    }

    public void UpdateIdleTransforms()
    {
        idleTransforms.Clear();

        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 1; i < childTransforms.Length; i++)
        {
            idleTransforms.Add(childTransforms[i].gameObject.name, new KeyValuePair<Vector3, Quaternion>(childTransforms[i].localPosition, childTransforms[i].localRotation));
        }
    }
}
