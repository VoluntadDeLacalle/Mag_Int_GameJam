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
            if (!Player.Instance.anim.GetBool("IsActivated"))
            {
                Player.Instance.anim.SetLayerWeight((int)gripType + 1, 1);
            }

            Player.Instance.anim.SetBool("IsActivated", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (Player.Instance.anim.GetBool("IsActivated"))
            {
                Player.Instance.anim.SetLayerWeight((int)gripType + 1, 0.5f);
            }

            Player.Instance.anim.SetBool("IsActivated", false);
        }
    }

    public override void OnEquip()
    {
        ChassisItem chassisItem = null;
        chassisItem = GetComponentInChildren<ChassisItem>();
        if (isEquipped == false || chassisItem == null)
        {
            if (!chassisItem.isEquipped)
            {
                return;
            }
        }

        Player.Instance.anim.SetLayerWeight((int)gripType + 1, 0.5f);
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
