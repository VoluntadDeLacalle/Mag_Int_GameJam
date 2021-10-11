using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AmmoItem : Item
{
    public ObjectPooler.Key ammoPrefabKey;

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
            bool gunEffectorCheck = false;
            bool catapultEffectorCheck = false;
            GunEffector currentGun = null;
            CatapultEffector currentCat = null;

            for (int i = 0; i < currentChassis.chassisComponentTransforms.Count; i++)
            {
                if (!currentChassis.chassisComponentTransforms[i].IsComponentTransformOccupied())
                {
                    continue;
                }

                GunEffector tempGun = null;
                CatapultEffector tempCat = null;

                tempGun = currentChassis.chassisComponentTransforms[i].GetComponentTransformItem().GetComponent<GunEffector>();
                tempCat = currentChassis.chassisComponentTransforms[i].GetComponentTransformItem().GetComponent<CatapultEffector>();

                if (tempGun != null)
                {
                    gunEffectorCheck = true;
                    currentGun = tempGun;
                    continue;
                }

                if (tempCat != null)
                {
                    catapultEffectorCheck = true;
                    currentCat = tempCat;
                    continue;
                }
            }

            if (!gunEffectorCheck && !catapultEffectorCheck)
            {
                GameObject currentAmmo = ObjectPooler.GetPooler(ammoPrefabKey).GetPooledObject();
                if (currentAmmo == null)
                {
                    return;
                }

                currentAmmo.transform.position = transform.position;
                currentAmmo.SetActive(true);
            }
        }
    }

    void FindCurrentChassis()
    {
        currentChassis = Inventory.Instance.currentEquippedGO.GetComponent<Item>();
    }

    void Update()
    {
        if (itemType != TypeTag.ammo)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not Ammo!");
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
