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

    GameObject attachedItem = null;

    void AttachItem()
    {
        attachedItem = equippedGO;
        attachedItem.transform.parent = rightHandAttachmentBone;
        attachedItem.transform.localPosition = new Vector3(0.069f, -0.048f, -0.028f);
        attachedItem.transform.Rotate(new Vector3(90.0f, 0, 0));
        attachedItem.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void EquipItem(int inventoryIndex)
    {
        Debug.Log("Player handler equip has worked");
        equippedItem = itemDetection.inventoryRef.inventory[inventoryIndex];
        equippedGO = equippedItem.gameObject;
        equippedGO.SetActive(true);

        equippedItem.isEquipped = true;

        AttachItem();
    }
}
