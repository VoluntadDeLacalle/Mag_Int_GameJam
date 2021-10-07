using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChassisComponentTransform
{
    public Transform componentTransform;
    private bool isOccupied = false;
    private Item currentComponent = null;

    public void CopyChassisComponentTransform(ChassisComponentTransform transformToCopy)
    {
        isOccupied = transformToCopy.IsComponentTransformOccupied();
        currentComponent = transformToCopy.GetComponentTransformItem();
    }

    public void AddNewComponentTransform(Item newComponent)
    {
        if (newComponent.itemType == Item.TypeTag.chassis || newComponent.itemType == Item.TypeTag.grip)
        {
            Debug.LogError("Illegal item type being added as Component Transform!");
            return;
        }

        isOccupied = true;
        currentComponent = newComponent;
        currentComponent.isEquipped = true;
        currentComponent.OnEquip();
    }

    public Item GetComponentTransformItem()
    {
        return currentComponent;
    }

    public bool IsComponentTransformOccupied()
    {
        return isOccupied;
    }

    public void ResetComponentTransform()
    {
        if (currentComponent == null)
        {
            return;
        }

        currentComponent.isEquipped = false;
        currentComponent.OnUnequip();

        isOccupied = false;
        currentComponent = null;
    }
}

[System.Serializable]
public class ChassisGripTransform
{
    public Transform gripTransform;
    private bool isOccupied = false;
    private Item currentGrip = null;

    public void CopyChassisGripTransform(ChassisGripTransform transformToCopy)
    {
        isOccupied = transformToCopy.IsGripTransformOccupied();
        currentGrip = transformToCopy.GetGripTransformItem();
    }

    public void AddNewGripTransform(Item newGrip)
    {
        isOccupied = true;
        currentGrip = newGrip;
        currentGrip.isEquipped = true;
        currentGrip.OnEquip();
    }

    public Item GetGripTransformItem()
    {
        return currentGrip;
    }

    public bool IsGripTransformOccupied()
    {
        return isOccupied;
    }

    public void ResetGripTransform()
    {
        currentGrip.isEquipped = false;
        currentGrip.OnUnequip();

        isOccupied = false;
        currentGrip = null;
    }
}

[Serializable]
[RequireComponent(typeof(SaveableEntity))]
public class Item : MonoBehaviour, ISaveable
{
    private bool hasBeenCopied = false;

    public enum TypeTag
    {
        chassis,
        effector,
        grip,
        ammo,
        modifier,
        scrap,
        external
    };
    public TypeTag itemType;

    public string itemName;
    [TextArea]
    public string description;
    public bool isObtained = false;
    public bool isRestored = false;
    public int restorationScrapAmount = 0;
    public bool isEquipped = false;
    public Vector3 localHandPos = Vector3.zero;
    public Vector3 localHandRot = Vector3.zero;
    public Sprite inventorySprite;
    public List<ChassisComponentTransform> chassisComponentTransforms = new List<ChassisComponentTransform>();
    public ChassisGripTransform chassisGripTransform;

    public object CaptureState()
    {
        if (itemType == TypeTag.chassis)
        {
            return new SaveData
            {
                isActive = this.gameObject.activeInHierarchy,
                currentPosition = transform,
                isObtained = isObtained,
                isRestored = isRestored,
                isEquipped = isEquipped,
                chassisComponentTransforms = chassisComponentTransforms,
                chassisGripTransform = chassisGripTransform
            };
        }
        else
        {
            return new SaveData
            {
                isActive = this.gameObject.activeInHierarchy,
                currentPosition = transform,
                isObtained = isObtained,
                isRestored = isRestored,
                isEquipped = isEquipped
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        this.gameObject.SetActive(saveData.isActive);
        transform.position = saveData.currentPosition.position;
        transform.rotation = saveData.currentPosition.rotation;
        isObtained = saveData.isObtained;
        isRestored = saveData.isRestored;
        isEquipped = saveData.isEquipped;

        if (itemType == TypeTag.chassis)
        {
            for (int i = 0; i < chassisComponentTransforms.Count; i++)
            {
                if (saveData.chassisComponentTransforms[i].IsComponentTransformOccupied())
                {
                    chassisComponentTransforms[i].ResetComponentTransform();
                    chassisComponentTransforms[i].AddNewComponentTransform(saveData.chassisComponentTransforms[i].GetComponentTransformItem());
                }
            }

            if (saveData.chassisGripTransform.IsGripTransformOccupied())
            {
                chassisGripTransform.ResetGripTransform();
                chassisGripTransform.AddNewGripTransform(saveData.chassisGripTransform.GetGripTransformItem());
            }
        }

        //Add item to inventory and update non-equipped chassis' and other items
        if ((isObtained && !isEquipped))
        {
            if (!Inventory.Instance.inventory.Contains(this))
            {
                Inventory.Instance.LoadAddToInventory(this);
            }

            if (itemType == TypeTag.chassis)
            {
                for (int i = 0; i < chassisComponentTransforms.Count; i++)
                {
                    if (chassisComponentTransforms[i].IsComponentTransformOccupied())
                    {
                        Item currentComponent = chassisComponentTransforms[i].GetComponentTransformItem();
                        GameObject currentComponentGO = currentComponent.gameObject;

                        currentComponentGO.transform.parent = gameObject.transform;
                        currentComponentGO.gameObject.transform.position = chassisComponentTransforms[i].componentTransform.position;
                        currentComponentGO.gameObject.transform.rotation = chassisComponentTransforms[i].componentTransform.rotation;
                        currentComponent.isEquipped = true;
                        currentComponent.OnEquip();

                        currentComponentGO.gameObject.SetActive(false);
                    }
                }

                if (chassisGripTransform.IsGripTransformOccupied())
                {
                    Item currentGrip = chassisGripTransform.GetGripTransformItem();
                    GameObject currentGripGO = currentGrip.gameObject;

                    gameObject.transform.parent = currentGripGO.transform;
                    gameObject.transform.position = currentGripGO.transform.position;
                    gameObject.transform.localRotation = Quaternion.Euler(0, -currentGrip.localHandRot.y, 0);
                    currentGrip.isEquipped = true;
                    currentGrip.OnEquip();

                    gameObject.SetActive(false);
                }
            }
        }

        //Updates the current equipped item.
        if (isEquipped)
        {
            if (itemType == TypeTag.chassis)
            {
                if (!Inventory.Instance.inventory.Contains(this))
                {
                    Inventory.Instance.LoadAddToInventory(this);
                }

                for (int i = 0; i < chassisComponentTransforms.Count; i++)
                {
                    if (chassisComponentTransforms[i].IsComponentTransformOccupied())
                    {
                        Item currentComponent = chassisComponentTransforms[i].GetComponentTransformItem();
                        GameObject currentComponentGO = currentComponent.gameObject;

                        currentComponentGO.transform.parent = gameObject.transform;
                        currentComponentGO.gameObject.transform.position = chassisComponentTransforms[i].componentTransform.position;
                        currentComponentGO.gameObject.transform.rotation = chassisComponentTransforms[i].componentTransform.rotation;
                        currentComponent.isEquipped = true;
                        currentComponent.OnEquip();

                        if (Inventory.Instance.inventory.Contains(currentComponent))
                        {
                            Inventory.Instance.inventory.Remove(currentComponent);
                        }

                        currentComponentGO.gameObject.SetActive(true);
                    }
                }

                if (chassisGripTransform.IsGripTransformOccupied())
                {
                    Item currentGrip = chassisGripTransform.GetGripTransformItem();
                    GameObject currentGripGO = currentGrip.gameObject;
                    Inventory.Instance.playerItemHandler.EquipItem(currentGrip);

                    gameObject.transform.parent = currentGripGO.transform;
                    gameObject.transform.position = currentGripGO.transform.position;
                    gameObject.transform.localRotation = Quaternion.Euler(0, -currentGrip.localHandRot.y, 0);
                    currentGrip.isEquipped = true;
                    currentGrip.OnEquip();

                    if (Inventory.Instance.inventory.Contains(currentGrip))
                    {
                        Inventory.Instance.inventory.Remove(currentGrip);
                    }

                    gameObject.SetActive(true);
                }
                else
                {
                    Inventory.Instance.playerItemHandler.EquipItem(this);
                }
            }
        }

        AddItemToVisualDictionary();
    }

    [Serializable]
    private struct SaveData
    {
        public bool isActive;
        public Transform currentPosition;
        public bool isObtained;
        public bool isRestored;
        public bool isEquipped;
        public List<ChassisComponentTransform> chassisComponentTransforms;
        public ChassisGripTransform chassisGripTransform;
    }

    private void OnEnable()
    {
        if (!ItemPooler.Instance.itemDictionary.ContainsKey(itemName))
        {
            ItemPooler.Instance.itemDictionary.Add(itemName, this);
        }
    }

    private void OnDrawGizmos()
    {
        if (itemType == TypeTag.chassis)
        {
            if (chassisGripTransform.gripTransform != null)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawSphere(chassisGripTransform.gripTransform.position, 0.05f);
            }
            
            for (int i = 0; i < chassisComponentTransforms.Count; i++)
            {
                if (chassisComponentTransforms[i].componentTransform != null)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.5f);
                    Gizmos.DrawSphere(chassisComponentTransforms[i].componentTransform.position, 0.05f);
                }
            }
        }
    }

    protected void LateUpdate()
    {
        AddItemToVisualDictionary();
    }

    void AddItemToVisualDictionary()
    {
        if (Inventory.Instance != null && !hasBeenCopied)
        {
            if (Inventory.Instance.visualItemDictionary.ContainsKey(this.gameObject))
            {
                return;
            }

            GameObject tempGameObj = Instantiate(this.gameObject);
            string tempName = tempGameObj.name;
            if (tempName.Contains("(Clone)"))
            {
                tempName = tempName.Substring(0, Mathf.Abs(tempName.IndexOf("(Clone)")));
            }
            tempGameObj.name = $"{tempName}_Unwrapped";

            if (tempGameObj.GetComponent<Item>() != null)
            {
                if (tempGameObj.GetComponent<Item>().itemType == TypeTag.chassis)
                {
                    tempGameObj.AddComponent<ChassisVisualItem>();
                    tempGameObj.GetComponent<ChassisVisualItem>().AddVisualTransforms(tempGameObj.GetComponent<Item>().chassisComponentTransforms, tempGameObj.GetComponent<Item>().chassisGripTransform);
                }
            }

            foreach (var component in tempGameObj.GetComponents<Component>())
            {
                if (component == tempGameObj.GetComponent<Transform>() || component == tempGameObj.GetComponent<MeshFilter>() ||
                    component == tempGameObj.GetComponent<MeshRenderer>() || component == tempGameObj.GetComponent<VisualItem>())
                {
                    continue;
                }

                Destroy(component);
            }

            tempGameObj.layer = LayerMask.NameToLayer("ItemRenderer");

            foreach (Transform child in tempGameObj.GetComponentsInChildren<Transform>())
            {
                MeshRenderer tempMR = null;
                tempMR = child.GetComponent<MeshRenderer>();

                if (tempMR == null)
                {
                    continue;
                }
                else
                {
                    child.gameObject.layer = LayerMask.NameToLayer("ItemRenderer");
                }
            }

            Inventory.Instance.visualItemDictionary.Add(this.gameObject, tempGameObj);
            tempGameObj.transform.SetParent(Inventory.Instance.visualItemParent.transform);
            tempGameObj.SetActive(false);
            hasBeenCopied = true;
        }
    }

    public void LoadItem(Item itemToBeLoaded)
    {
        isObtained = itemToBeLoaded.isObtained;
        isRestored = itemToBeLoaded.isRestored;
        isEquipped = itemToBeLoaded.isEquipped;
        
        if (itemType == TypeTag.chassis && itemToBeLoaded.itemType == TypeTag.chassis)
        {
            for (int i = 0; i < chassisComponentTransforms.Count; i++)
            {
                chassisComponentTransforms[i].CopyChassisComponentTransform(itemToBeLoaded.chassisComponentTransforms[i]);
            }
            
            chassisGripTransform.CopyChassisGripTransform(itemToBeLoaded.chassisGripTransform);
        }
    }

    public virtual void OnEquip() { }
    public virtual void OnUnequip() { }
    public virtual void Activate() { }
    public virtual void ModifyComponent(ModifierItem.ModifierType modifierType) { }
    public virtual void UnmodifyComponent(ModifierItem.ModifierType modifierType) { }
}
