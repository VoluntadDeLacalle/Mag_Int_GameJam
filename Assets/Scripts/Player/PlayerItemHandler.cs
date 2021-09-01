using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    public PlayerController playerController;
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
        equippedItem = itemDetection.inventoryRef.inventory[inventoryIndex];
        equippedGO = equippedItem.gameObject;
        equippedGO.SetActive(true);

        equippedItem.isEquipped = true;

        AttachItem(itemDetection.inventoryRef.inventory[inventoryIndex].localHandPos,
                   itemDetection.inventoryRef.inventory[inventoryIndex].localHandRot);
    }

    public void UnequipItem(int inventoryIndex)
    {
        itemDetection.inventoryRef.inventory[inventoryIndex].isEquipped = false;
        itemDetection.inventoryRef.inventory[inventoryIndex].gameObject.transform.parent = null;
        itemDetection.inventoryRef.inventory[inventoryIndex].gameObject.SetActive(false);
        itemDetection.inventoryRef.inventory[inventoryIndex].gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        equippedItem = null;
        equippedGO = null;
        attachedItem = null;
    }
}
