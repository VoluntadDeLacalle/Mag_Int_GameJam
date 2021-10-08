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
        if (currentGrip == null)
        {
            return;
        }

        currentGrip.isEquipped = false;
        currentGrip.OnUnequip();

        isOccupied = false;
        currentGrip = null;
    }
}

[Serializable]
public struct ChassisDataModel
{
    public string itemName;
    public Transform itemTransform;
    public bool isObtained;
    public bool isRestored;
    public bool isEquipped;
    public List<ItemDataModel?> componentItemModels;
    public ItemDataModel? gripItemModel;
}

[Serializable]
public struct ItemDataModel
{
    public string itemName;
    public Transform itemTransform;
    public bool isObtained;
    public bool isRestored;
    public bool isEquipped;
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
            List<ItemDataModel?> tempComponentModels = new List<ItemDataModel?>();
            for (int i = 0; i < chassisComponentTransforms.Count; i++)
            {
                ItemDataModel? currentItemDataModel;
                if (chassisComponentTransforms[i].IsComponentTransformOccupied())
                {
                    Item currentComponent = chassisComponentTransforms[i].GetComponentTransformItem();
                    currentItemDataModel = new ItemDataModel 
                    {
                        itemName = currentComponent.itemName,
                        itemTransform = currentComponent.gameObject.transform,
                        isObtained = currentComponent.isObtained,
                        isRestored = currentComponent.isRestored,
                        isEquipped = currentComponent.isEquipped
                    };

                    tempComponentModels.Add(currentItemDataModel);
                }
                else
                {
                    currentItemDataModel = null;
                    tempComponentModels.Add(currentItemDataModel);
                }
            }

            ItemDataModel? tempGripModel;
            if (chassisGripTransform.IsGripTransformOccupied())
            {
                Item currentGripItem = chassisGripTransform.GetGripTransformItem();
                tempGripModel = new ItemDataModel
                {
                    itemName = currentGripItem.itemName,
                    itemTransform = currentGripItem.gameObject.transform,
                    isObtained = currentGripItem.isObtained,
                    isRestored = currentGripItem.isRestored,
                    isEquipped = currentGripItem.isEquipped
                };
            }
            else
            {
                tempGripModel = null;
            }


            return new ChassisDataModel
            {
                itemName = itemName,
                itemTransform = transform,
                isObtained = isObtained,
                isRestored = isRestored,
                isEquipped = isEquipped,
                componentItemModels = new List<ItemDataModel?>(tempComponentModels),
                gripItemModel = tempGripModel
            };
        }
        else
        {
            return new ItemDataModel
            {
                itemName = itemName,
                itemTransform = transform,
                isObtained = isObtained,
                isRestored = isRestored,
                isEquipped = isEquipped,
            };
        }
    }

    public void RestoreState(object state)
    {
        if (itemType != TypeTag.chassis)
        {
            var savedItemDataModel = (ItemDataModel)state;

            if (savedItemDataModel.isObtained || savedItemDataModel.isEquipped)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                transform.position = savedItemDataModel.itemTransform.position;
                transform.rotation = savedItemDataModel.itemTransform.rotation;
                isObtained = savedItemDataModel.isObtained;
                isRestored = savedItemDataModel.isRestored;
                isEquipped = savedItemDataModel.isEquipped;
            }
        }
        else
        {
            var savedChassisDataModel = (ChassisDataModel)state;

            if (savedChassisDataModel.isObtained)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                transform.position = savedChassisDataModel.itemTransform.position;
                transform.rotation = savedChassisDataModel.itemTransform.rotation;
                isObtained = savedChassisDataModel.isObtained;
                isRestored = savedChassisDataModel.isRestored;
                isEquipped = savedChassisDataModel.isEquipped;

                for (int i = 0; i < savedChassisDataModel.componentItemModels.Count; i++)
                {
                    chassisComponentTransforms[i].ResetComponentTransform();
                    if (savedChassisDataModel.componentItemModels[i].HasValue)
                    {
                        GameObject currentComponentGameObject = ItemPooler.Instance.InstantiateItemByName(savedChassisDataModel.componentItemModels[i].Value.itemName);

                        currentComponentGameObject.transform.parent = gameObject.transform;
                        currentComponentGameObject.gameObject.transform.position = chassisComponentTransforms[i].componentTransform.position;
                        currentComponentGameObject.gameObject.transform.rotation = chassisComponentTransforms[i].componentTransform.rotation;
                        currentComponentGameObject.gameObject.SetActive(true);

                        chassisComponentTransforms[i].AddNewComponentTransform(currentComponentGameObject.GetComponent<Item>());
                    }
                }

                chassisGripTransform.ResetGripTransform();
                if (savedChassisDataModel.gripItemModel.HasValue)
                {
                    GameObject currentGripGameObject = ItemPooler.Instance.InstantiateItemByName(savedChassisDataModel.gripItemModel.Value.itemName);

                    gameObject.transform.parent = currentGripGameObject.transform;
                    gameObject.transform.position = currentGripGameObject.transform.position;
                    gameObject.transform.localRotation = Quaternion.Euler(0, -currentGripGameObject.GetComponent<Item>().localHandRot.y, 0);
                    gameObject.SetActive(true);

                    currentGripGameObject.transform.position = savedChassisDataModel.itemTransform.position;
                    currentGripGameObject.transform.rotation = savedChassisDataModel.itemTransform.rotation;

                    chassisGripTransform.AddNewGripTransform(currentGripGameObject.GetComponent<Item>());
                }
            }
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
