using Invector.vCharacterController;
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

    public override void OnEquip()
    {
        if (isEquipped == false)
        {
            return;
        }

        Player.Instance.anim.SetBool("IsHoldingChassis", true);
    }

    public override void OnUnequip()
    {
        if (isEquipped == true)
        {
            return;
        }

        Player.Instance.anim.SetBool("IsHoldingChassis", false);
        Player.Instance.anim.SetBool("IsActivated", false);
    }

    void LateUpdate()
    {
        if (itemType != TypeTag.chassis)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not Chassis!");
            return;
        }

        if (chassisComponentTransforms.Count < 1 || isEquipped == false || Time.timeScale == 0.0f || !Player.Instance.IsAlive())
        {
            return;
        }

        for (int i = 0; i < chassisComponentTransforms.Count; i++)
        {
            if (!Player.Instance.vThirdPersonInput.CanMove())
            {
                break;
            }

            if (chassisComponentTransforms[i].IsComponentTransformOccupied())
            {
                chassisComponentTransforms[i].GetComponentTransformItem().Activate();
            }
        }

        if (chassisGripTransform.IsGripTransformOccupied())
        {
            if (Player.Instance.anim.GetLayerWeight((int)chassisGripTransform.GetGripTransformItem().gameObject.GetComponent<GripItem>().gripType) == 0)
            {
                Player.Instance.anim.SetLayerWeight((int)chassisGripTransform.GetGripTransformItem().gameObject.GetComponent<GripItem>().gripType, 0.5f);
            }

            chassisGripTransform.GetGripTransformItem().Activate();
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
