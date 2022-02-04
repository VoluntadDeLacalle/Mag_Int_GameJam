using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalComponentEffectorActions : EffectorActions
{
    public Electrical electricalComponent;

    public override void PowerEffectorAction()
    {
        if (!electricalComponent.IsPowered())
        {
            electricalComponent.SetIsPowered(true);
        }
    }

    public override void EMPEffectorAction()
    {
        if (electricalComponent.IsPowered())
        {
            electricalComponent.SetIsPowered(false);
        }
    }
}
