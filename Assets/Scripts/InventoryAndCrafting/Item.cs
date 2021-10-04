using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChassisComponentTransform
{
    public Transform componentTransform;
    private bool isOccupied = false;
    private Item currentComponent = null;

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
        currentComponent.isEquipped = false;
        currentComponent.OnUnequip();

        isOccupied = false;
        currentComponent = null;
    }
}

[System.Serializable]
public class ChassisGripTransform
{
    public Transform componentTransform;
    private bool isOccupied = false;
    private Item currentGrip = null;

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

public class Item : MonoBehaviour
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

    private void OnDrawGizmos()
    {
        if (itemType == TypeTag.chassis)
        {
            if (chassisGripTransform.componentTransform != null)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawSphere(chassisGripTransform.componentTransform.position, 0.05f);
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

    protected void Update()
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

            foreach(Transform child in tempGameObj.GetComponentsInChildren<Transform>())
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

    public virtual void OnEquip() { }
    public virtual void OnUnequip() { }
    public virtual void Activate() { }
    public virtual void ModifyComponent(ModifierItem.ModifierType modifierType) { }
    public virtual void UnmodifyComponent(ModifierItem.ModifierType modifierType) { }
}
