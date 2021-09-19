using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemViewer : MonoBehaviour
{
    public Transform handAttachment;
    public Animator UIPlayerAnim;
    public float lerpSpeed = 1;

    private bool shouldLerpUp = false;
    private bool shouldLerpDown = false;
    private float lowActivationLerp = 0;
    private float maxActivationLerp = 1;
    private int currentAnimationlayer = -1;
    private int previousAnimationLayer = -1;

    public void SwitchPlayerAnimLayer(int index)
    {
        currentAnimationlayer = index;
        if (currentAnimationlayer == 0)
        {
            if (previousAnimationLayer == -1)
            {
                return;
            }

            shouldLerpDown = true;
            shouldLerpUp = false;
            lowActivationLerp = maxActivationLerp;
        }
        else
        {
            if (previousAnimationLayer == currentAnimationlayer)
            {
                return;
            }

            shouldLerpUp = true;
            shouldLerpDown = false;
            lowActivationLerp = 0;
        }

        for (int i = 0; i < UIPlayerAnim.layerCount; i++)
        {
            if (i == currentAnimationlayer || i == previousAnimationLayer)
            {
                continue;
            }

            UIPlayerAnim.SetLayerWeight(i, 0);
        }
    }

    private void LateUpdate()
    {
        if (shouldLerpUp)
        {
            if (lowActivationLerp < maxActivationLerp - 0.05f)
            {
                lowActivationLerp = Mathf.Lerp(lowActivationLerp, maxActivationLerp, lerpSpeed * Time.fixedDeltaTime);
                UIPlayerAnim.SetLayerWeight(currentAnimationlayer, lowActivationLerp);
                return;
            }

            lowActivationLerp = maxActivationLerp;
            UIPlayerAnim.SetLayerWeight(currentAnimationlayer, lowActivationLerp);

            shouldLerpUp = false;
            previousAnimationLayer = currentAnimationlayer;
            currentAnimationlayer = -1;

        }
        else if (shouldLerpDown)
        {
            if (lowActivationLerp > 0.05f)
            {
                lowActivationLerp = Mathf.Lerp(lowActivationLerp, 0, lerpSpeed * Time.fixedDeltaTime);
                UIPlayerAnim.SetLayerWeight(previousAnimationLayer, lowActivationLerp);
                return;
            }

            lowActivationLerp = 0;
            UIPlayerAnim.SetLayerWeight(previousAnimationLayer, lowActivationLerp);

            shouldLerpDown = false;
            currentAnimationlayer = -1;
            previousAnimationLayer = -1;
        }

    }
}
