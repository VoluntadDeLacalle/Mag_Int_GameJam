using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmineEffectorActions : EffectorActions
{
    public LandmineNEW landmine;

    public override void PowerEffectorAction()
    {
        landmine.ActivateExplosion();
    }
}
