using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackFill : MonoBehaviour
{
    public SkinnedMeshRenderer backpackRef;

    public void IncreaseBackpack(int amount)
    {
        if (backpackRef.GetBlendShapeWeight(0) - amount >= 0)
        {
            backpackRef.SetBlendShapeWeight(0, backpackRef.GetBlendShapeWeight(0) - amount);
            return;
        }
        else
        {
            if (backpackRef.GetBlendShapeWeight(0) != 0)
            {
                backpackRef.SetBlendShapeWeight(0, 0);
                return;
            }

            if (backpackRef.GetBlendShapeWeight(1) + amount <= 100)
            {
                backpackRef.SetBlendShapeWeight(1, backpackRef.GetBlendShapeWeight(1) + amount);
                return;
            }
            else
            {
                if(backpackRef.GetBlendShapeWeight(1) != 100)
                {
                    backpackRef.SetBlendShapeWeight(1, 100);
                    return;
                }
            }
        }
    }
    
    public void DecreaseBackpack(int amount)
    {
        if (backpackRef.GetBlendShapeWeight(1) - amount >= 0)
        {
            backpackRef.SetBlendShapeWeight(1, backpackRef.GetBlendShapeWeight(1) - amount);
            return;
        }
        else
        {
            if (backpackRef.GetBlendShapeWeight(1) != 0)
            {
                backpackRef.SetBlendShapeWeight(1, 0);
                return;
            }

            if (backpackRef.GetBlendShapeWeight(0) + amount <= 100)
            {
                backpackRef.SetBlendShapeWeight(0, backpackRef.GetBlendShapeWeight(0) + amount);
                return;
            }
            else
            {
                if (backpackRef.GetBlendShapeWeight(0) != 100)
                {
                    backpackRef.SetBlendShapeWeight(0, 100);
                    return;
                }
            }
        }
    }
}
