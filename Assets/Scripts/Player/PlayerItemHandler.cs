using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    public vThirdPersonController playerController;
    public Pickup itemDetection;
    private Item equippedItem;
    private GameObject equippedGO;

    public Transform leftHandAttachmentBone;

    public GameObject attachedItem = null;

    public Item GetEquippedItem()
    {
        return equippedGO.GetComponent<Item>();
    }

    void AttachItem(Vector3 newPos, Vector3 newRot)
    {
        attachedItem = equippedGO;
        attachedItem.transform.parent = leftHandAttachmentBone;
        attachedItem.transform.localPosition = newPos;
        attachedItem.transform.localRotation = Quaternion.Euler(newRot);
    }

    public void EquipItem(int inventoryIndex)
    {
        if (equippedGO != null)
        {
            //for (int i = 0; i < Inventory.Instance.inventory.Count; i++)
            //{
            //    if (Inventory.Instance.inventory[i].isEquipped && Inventory.Instance.inventory[i].gameObject == equippedGO)
            //    {
            //        UnequipItem(i);
            //        break;
            //    }
            //}

            UnequipItem(equippedItem);
        }

        equippedItem = Inventory.Instance.inventory[inventoryIndex];
        equippedGO = equippedItem.gameObject;
        equippedGO.SetActive(true);

        if (equippedItem.itemType != Item.TypeTag.grip)
        {
            equippedItem.isEquipped = true;
        }

        AttachItem(Inventory.Instance.inventory[inventoryIndex].localHandPos,
                   Inventory.Instance.inventory[inventoryIndex].localHandRot);
    }

    public void EquipItem(Item itemToEquip)
    {
        if (equippedGO != null)
        {
            UnequipItem(equippedItem);
        }

        equippedItem = itemToEquip;
        equippedGO = equippedItem.gameObject;
        equippedGO.SetActive(true);

        if (itemToEquip.itemType != Item.TypeTag.grip)
        {
            equippedItem.isEquipped = true;
        }
        
        AttachItem(itemToEquip.localHandPos, itemToEquip.localHandRot);
    }

    public void UnequipItem(int inventoryIndex)
    {
        if (Inventory.Instance.inventory[inventoryIndex].itemType != Item.TypeTag.grip)
        {
            Inventory.Instance.inventory[inventoryIndex].isEquipped = false;
        }
        else
        {
            Inventory.Instance.inventory[inventoryIndex].gameObject.GetComponentInChildren<ChassisItem>().isEquipped = false;
        }
        
        Inventory.Instance.inventory[inventoryIndex].gameObject.transform.parent = null; 
        Inventory.Instance.inventory[inventoryIndex].gameObject.SetActive(false);
        Inventory.Instance.inventory[inventoryIndex].gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        equippedItem = null;
        equippedGO = null;
        attachedItem = null;
    }

    public void UnequipItem(Item itemToUnequip)
    {
        if (itemToUnequip.itemType != Item.TypeTag.grip)
        {
            itemToUnequip.isEquipped = false;
        }
        else
        {
            itemToUnequip.GetComponentInChildren<ChassisItem>().isEquipped = false;
        }

        itemToUnequip.gameObject.transform.parent = null;
        itemToUnequip.gameObject.SetActive(false);

        equippedItem = null;
        equippedGO = null;
        attachedItem = null;
    }
}
