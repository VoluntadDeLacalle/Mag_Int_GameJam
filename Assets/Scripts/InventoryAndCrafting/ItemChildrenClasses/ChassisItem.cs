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
            return;
        }

        if (chassisEffectorTransforms.Count < 1 || isEquipped == false || Time.timeScale == 0.0f)
        {
            return;
        }

        for (int i = 0; i < chassisEffectorTransforms.Count; i++)
        {
            if (chassisEffectorTransforms[i].isOccupied)
            {
                chassisEffectorTransforms[i].currentEffector.Activate();
            }
        }
    }
}
