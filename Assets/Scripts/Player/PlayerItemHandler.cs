using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    public PlayerController playerController;
    public Pickup itemDetection;
    private Item equippedItem;
    private GameObject equippedGO;

    public Transform rightHandAttachmentBone;

    public GameObject attachedItem = null;

    void AttachItem()
    {
        attachedItem = equippedGO;
        attachedItem.transform.parent = rightHandAttachmentBone;
        attachedItem.transform.localPosition = new Vector3(0.069f, -0.048f, -0.028f);
        //attachedItem.transform.Rotate(new Vector3(90.0f, 0, 0));
    }

    public void EquipItem(int inventoryIndex)
    {
        equippedItem = itemDetection.inventoryRef.inventory[inventoryIndex];
        equippedGO = equippedItem.gameObject;
        equippedGO.SetActive(true);

        equippedItem.isEquipped = true;

        AttachItem();
    }

    public void UnequipItem(int inventoryIndex)
    {
        itemDetection.inventoryRef.inventory[inventoryIndex].isEquipped = false;
        itemDetection.inventoryRef.inventory[inventoryIndex].gameObject.transform.parent = null;
        itemDetection.inventoryRef.inventory[inventoryIndex].gameObject.SetActive(false);
        
        equippedItem = null;
        equippedGO = null;
        attachedItem = null;
    }
}
