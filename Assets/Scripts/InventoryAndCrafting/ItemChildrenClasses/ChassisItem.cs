using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChassisItem : Item
{
    List<bool> currentSlotsFilled = new List<bool>();

    private void Awake()
    {
        if (currentSlotsFilled.Count == 0)
        {
            for (int i = 0; i < chassisComponentTransforms.Count; i++)
            {
                currentSlotsFilled.Add(false);
            }
        }
    }

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
                //if(chassisComponentTransforms[i].GetComponentTransformItem().itemType == TypeTag.modifier)
                //{
                //    continue;
                //}

                chassisComponentTransforms[i].GetComponentTransformItem().Activate();
            }
        }

        for (int i = 0; i < chassisComponentTransforms.Count; i++)
        {
            if (chassisComponentTransforms[i].IsComponentTransformOccupied())
            {
                if (!chassisComponentTransforms[i].GetComponentTransformItem().isEquipped)
                {
                    chassisComponentTransforms[i].GetComponentTransformItem().isEquipped = true;
                }

                //for (int j = 0; j < chassisComponentTransforms.Count; j++)
                //{
                //    if(chassisComponentTransforms[j].GetComponentTransformItem().itemType == TypeTag.modifier)
                //    {
                //        chassisComponentTransforms[j].GetComponentTransformItem().Activate();
                //    }
                //} //Attempt at only have the modifier run once. Oh well, keep it in case you need it. B)

                //Debug.Log("Running");
            }
        }
    }
}
