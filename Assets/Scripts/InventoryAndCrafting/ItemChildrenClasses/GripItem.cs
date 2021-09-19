using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripItem : Item
{
    public enum GripType
    {
        None,
        Simple,
        Pole,
        Umbrella
    };

    [Header("Grip Variables")]
    public GripType gripType = GripType.None;

    public override void Activate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Player.Instance.anim.SetBool("IsActivated", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Player.Instance.anim.SetBool("IsActivated", false);
        }
    }

    public override void OnEquip()
    {
        Player.Instance.anim.SetLayerWeight((int)gripType + 1, 1);
    }

    public override void OnUnequip()
    {
        Player.Instance.anim.SetLayerWeight((int)gripType + 1, 0);
        Player.Instance.anim.SetBool("IsActivated", false);
    }

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
