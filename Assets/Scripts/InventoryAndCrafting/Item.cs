using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChassisComponentTransform
{
    public Transform componentTransform;
    public Item.TypeTag componentType = Item.TypeTag.activeComponent;
    public bool isOccupied = false;
}

public class Item : MonoBehaviour
{
    public enum TypeTag
    {
        chassis,
        activeComponent,
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
    public List<ChassisComponentTransform> chassisComponentTransforms;
    
    public Item(Item oldItem)
    {
        this.itemType = oldItem.itemType;
        this.itemName = oldItem.itemName;
        this.description = oldItem.description;
        this.inventorySprite = oldItem.inventorySprite;

        if (this.itemType == TypeTag.chassis)
        {
            this.chassisComponentTransforms = new List<ChassisComponentTransform>(oldItem.chassisComponentTransforms);
        }
    }

    private void OnDrawGizmos()
    {
        if (itemType == TypeTag.chassis)
        {
            foreach(ChassisComponentTransform chassisComponent in chassisComponentTransforms)
            {
                switch (chassisComponent.componentType)
                {
                    case TypeTag.grip:
                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                        Gizmos.DrawSphere(chassisComponent.componentTransform.position, 0.5f);
                        break;
                    case TypeTag.activeComponent:
                        Gizmos.color = new Color(0, 0, 1, 0.5f);
                        Gizmos.DrawSphere(chassisComponent.componentTransform.position, 0.5f);
                        break;
                    case TypeTag.chassis:
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(chassisComponent.componentTransform.position, 0.5f);
                        break;
                }
            }
        }
    }

    public virtual void Activate() { }
}
