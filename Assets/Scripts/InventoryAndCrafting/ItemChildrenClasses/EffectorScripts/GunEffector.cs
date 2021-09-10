using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffector : Item
{
    public Launcher gunLauncher;
    
    private Item currentChassis = null;

    public override void Activate()
    {
        if (currentChassis == null && isEquipped)
        {
            FindCurrentChassis();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            List<Item> currentAttachedAmmo = new List<Item>();
            for (int i = 0; i < currentChassis.chassisComponentTransforms.Count; i++)
            {
                if (currentChassis.chassisComponentTransforms[i].IsComponentTransformOccupied())
                {
                    if (currentChassis.chassisComponentTransforms[i].GetComponentTransformItem().gameObject == this.gameObject)
                    {
                        continue;
                    }
                    else if (currentChassis.chassisComponentTransforms[i].GetComponentTransformItem().itemType == Item.TypeTag.ammo)
                    {
                        currentAttachedAmmo.Add(currentChassis.chassisComponentTransforms[i].GetComponentTransformItem());
                    }
                }
            }

            for (int i = 0; i < currentAttachedAmmo.Count; i++)
            {
                gunLauncher.Shoot(currentAttachedAmmo[i].gameObject.GetComponent<AmmoItem>().ammoPrefabKey);
            }
        }
    }

    void FindCurrentChassis()
    {
        Inventory inventoryRef = Inventory.Instance;

        for (int i = 0; i < inventoryRef.inventory.Count; i++)
        {
            if (inventoryRef.inventory[i].itemType == TypeTag.chassis && inventoryRef.inventory[i].isEquipped)
            {
                currentChassis = inventoryRef.inventory[i];
                return;
            }
        }
    }

    new void Update()
    {
        base.Update();

        if (itemType != TypeTag.effector)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not Effector!");
            return;
        }

        if (currentChassis != null)
        {
            if (!currentChassis.isEquipped)
            {
                currentChassis = null;
            }
        }
    }
}