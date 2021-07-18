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
    
    //public Item(Item oldItem)
    //{
    //    this.itemType = oldItem.itemType;
    //    this.itemName = oldItem.itemName;
    //    this.description = oldItem.description;
    //    this.inventorySprite = oldItem.inventorySprite;
    //    this.isEquipped = oldItem.isEquipped;

    //    if (this.itemType == TypeTag.chassis)
    //    {
    //        this.localHandPos = oldItem.localHandPos;
    //        this.localHandRot = oldItem.localHandRot;
    //        this.chassisGripTransform = oldItem.chassisGripTransform;
    //        this.chassisEffectorTransforms = new List<ChassisEffectorTransform>(oldItem.chassisEffectorTransforms);
    //    }
    //}

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

    public virtual void Activate() { }
}
