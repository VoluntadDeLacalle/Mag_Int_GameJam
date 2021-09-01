using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            GameObject currentAmmo = ObjectPooler.GetPooler(ammoPrefabKey).GetPooledObject();
            if (currentAmmo == null)
            {
                return;
            }

            Trajectory currentTrajectory = currentAmmo.GetComponent<Trajectory>();
            if (currentTrajectory == null)
            {
                Debug.LogError("Selected Ammo does not have a trajectory script attached!");
                return;
            }

            bool gunEffectorCheck = false;
            bool catapultEffectorCheck = false;
            GunEffector currentGun = null;
            CatapultEffector currentCat = null;

            for (int i = 0; i < currentChassis.chassisComponentTransforms.Count; i++)
            {
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

            if (gunEffectorCheck && catapultEffectorCheck)
            {
                int tempNumbGen = Random.Range(0, 2);
                if (tempNumbGen == 0)
                {
                    currentTrajectory.trajectoryType = Trajectory.TrajectoryType.straight;
                    currentAmmo.transform.rotation.SetLookRotation(currentGun.transform.forward, currentGun.transform.up);
                }
                else
                {
                    currentTrajectory.trajectoryType = Trajectory.TrajectoryType.arc;
                    currentAmmo.transform.rotation.SetLookRotation(currentCat.transform.forward, currentCat.transform.up);
                }
            }
            else if (gunEffectorCheck && !catapultEffectorCheck)
            {
                currentTrajectory.trajectoryType = Trajectory.TrajectoryType.straight;
                currentAmmo.transform.rotation.SetLookRotation(currentGun.transform.forward, currentGun.transform.up);
            }
            else if (!gunEffectorCheck && catapultEffectorCheck)
            {
                currentTrajectory.trajectoryType = Trajectory.TrajectoryType.arc;
                currentAmmo.transform.rotation.SetLookRotation(currentCat.transform.forward, currentCat.transform.up);
            }
            else
            {
                currentTrajectory.trajectoryType = Trajectory.TrajectoryType.none;
                currentAmmo.transform.rotation.SetLookRotation(transform.forward, transform.up);
            }

            currentAmmo.transform.position = transform.position;
            currentAmmo.SetActive(true);
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
