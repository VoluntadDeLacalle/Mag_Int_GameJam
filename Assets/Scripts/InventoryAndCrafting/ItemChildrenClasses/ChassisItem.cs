using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChassisItem : Item
{
    new void Update()
    {
        base.Update();
        if (itemType != TypeTag.chassis)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not Chassis!");
            return;
        }

        if (chassisComponentTransforms.Count < 1 || isEquipped == false || Time.timeScale == 0.0f)
        {
            return;
        }

        for (int i = 0; i < chassisComponentTransforms.Count; i++)
        {
            if (chassisComponentTransforms[i].IsComponentTransformOccupied())
            {
                chassisComponentTransforms[i].GetComponentTransformItem().Activate();
            }
        }
    }
}
