using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripItem : Item
{
    new void Update()
    {
        base.Update();

        if (itemType != TypeTag.grip)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not Grip!");
            return;
        }
    }
}
