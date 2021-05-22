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
    public int weight;
    public UnityEngine.UI.Image inventorySprite;  
    public List<ChassisComponentTransform> chassisComponentTransforms;

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
