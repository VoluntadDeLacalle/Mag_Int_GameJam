using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChassisEffectorTransform
{
    public Transform componentTransform;
    public bool isOccupied = false;
    public Item currentEffector = null;

    public void AddNewEffectorTransform(Item newEffector)
    {
        isOccupied = true;
        currentEffector = newEffector;
    }

    public void ResetEffectorTransform()
    {
        isOccupied = false;
        currentEffector = null;
    }
}

[System.Serializable]
public class ChassisGripTransform
{
    public Transform componentTransform;
    public bool isOccupied = false;
    public Item currentGrip = null;

    public void AddNewGripTransform(Item newGrip)
    {
        isOccupied = true;
        currentGrip = newGrip;
    }

    public void ResetGripTransform()
    {
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
        grip
    };
    public TypeTag itemType;
    
    public string itemName;
    [TextArea]
    public string description;
    public bool isEquipped = false;
    public Vector3 localHandPos = Vector3.zero;
    public Vector3 localHandRot = Vector3.zero;
    public Sprite inventorySprite;
    public List<ChassisEffectorTransform> chassisEffectorTransforms = new List<ChassisEffectorTransform>();
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
            
            for (int i = 0; i < chassisEffectorTransforms.Count; i++)
            {
                if (chassisEffectorTransforms[i].componentTransform != null)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.5f);
                    Gizmos.DrawSphere(chassisEffectorTransforms[i].componentTransform.position, 0.05f);
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

            foreach (var component in tempGameObj.GetComponents<Component>())
            {
                if (component == tempGameObj.GetComponent<Transform>() || component == tempGameObj.GetComponent<MeshFilter>() ||
                    component == tempGameObj.GetComponent<MeshRenderer>())
                {
                    continue;
                }

                Destroy(component);
            }

            tempGameObj.layer = LayerMask.NameToLayer("ItemRenderer");

            Inventory.Instance.visualItemDictionary.Add(this.gameObject, tempGameObj);
            tempGameObj.SetActive(false);
            hasBeenCopied = true;
        }
    }

    public virtual void Activate() { }
}
