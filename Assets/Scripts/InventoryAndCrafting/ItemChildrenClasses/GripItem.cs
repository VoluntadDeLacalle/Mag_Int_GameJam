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

    }

    public override void OnUnequip()
    {
        Player.Instance.anim.SetInteger("GripEnum", -1);
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
