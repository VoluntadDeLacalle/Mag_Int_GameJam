using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] inventory = new Item[20];
    public GameObject[] inventoryImages = new GameObject[20];
    public GameObject inventoryPanel;
    private bool isActive = false;
    public PlayerItemHandler playerItemHandler;
    public Transform dropTransform;

    [Header("Visual Variables")]
    public Sprite nullPlaceholderSprite;
    public UnityEngine.UI.Image selectedInspectorImage;
    public TMPro.TextMeshProUGUI selectedItemTitle;
    public TMPro.TextMeshProUGUI selectedItemDescription;
    public GameObject dropItemButton;
    public GameObject equipItemButton;
    public GameObject unequipItemButton;
    private int dropIndex = -1;
    private int equipIndex = -1;

    private void Update()
    {
        if (inventoryPanel.activeSelf && !isActive)
        {
            isActive = true;

            selectedInspectorImage.sprite = nullPlaceholderSprite;
            selectedItemTitle.text = "";
            selectedItemDescription.text = "";
            dropItemButton.SetActive(false);
            equipItemButton.SetActive(false);
            unequipItemButton.SetActive(false);
        }
        else if (!inventoryPanel.activeSelf && isActive)
        {
            isActive = false;
        }
    }

    public void AddToInventory(Item newItem)
    {
        for (int i = 0; i < 20; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = newItem;
                inventoryImages[i].GetComponent<UnityEngine.UI.Image>().sprite = newItem.inventorySprite;

                newItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                newItem.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void DropFromInventory()
    {
        if (dropIndex == -1)
        {
            return;
        }

        if (playerItemHandler.attachedItem != null)
        {
            UnequipItem();
        }

        inventory[dropIndex].gameObject.transform.position = dropTransform.position;
        inventory[dropIndex].gameObject.GetComponent<Rigidbody>().isKinematic = false;
        inventory[dropIndex].gameObject.SetActive(true);

        inventory[dropIndex] = null;
        inventoryImages[dropIndex].GetComponent<UnityEngine.UI.Image>().sprite = nullPlaceholderSprite;

        selectedInspectorImage.sprite = nullPlaceholderSprite;
        selectedItemTitle.text = "";
        selectedItemDescription.text = "";
        dropItemButton.SetActive(false);
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(false);
        dropIndex = -1;

        //spawn item in world
    }

    public void ChangeInventoryInformation(GameObject clickedObject)
    {
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(false);
        equipIndex = -1;

        int imageIndex = -1;
        for (int i = 0; i < inventoryImages.Length; i++)
        {
            if (clickedObject == inventoryImages[i] && clickedObject.GetComponent<UnityEngine.UI.Image>().sprite != nullPlaceholderSprite)
            {
                imageIndex = i;
                break;
            }
        }

        if (imageIndex == -1)
        {
            selectedInspectorImage.sprite = nullPlaceholderSprite;
            selectedItemTitle.text = "";
            selectedItemDescription.text = "";
            dropItemButton.SetActive(false);
        }
        else
        {
            selectedInspectorImage.sprite = inventory[imageIndex].inventorySprite;

            switch (inventory[imageIndex].itemType)
            {
                case Item.TypeTag.chassis:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Chassis)";
                    equipIndex = imageIndex;

                    if (inventory[imageIndex].isEquipped)
                    {
                        equipItemButton.SetActive(false);
                        unequipItemButton.SetActive(true);
                    }
                    else
                    {
                        unequipItemButton.SetActive(false);
                        equipItemButton.SetActive(true);
                    }
                    break;
                case Item.TypeTag.grip:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Grip)";
                    break;
                case Item.TypeTag.activeComponent:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Component)";
                    break;
            }
            
            selectedItemDescription.text = inventory[imageIndex].description;
            dropItemButton.SetActive(true);
            dropIndex = imageIndex;
        }
    }

    public void EquipItem()
    {
        if (equipIndex == -1)
        {
            return;
        }

        playerItemHandler.EquipItem(equipIndex);
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(true);
    }

    public void UnequipItem()
    {
        if (equipIndex == -1)
        {
            return;
        }

        playerItemHandler.UnequipItem(equipIndex);
        unequipItemButton.SetActive(false);
        equipItemButton.SetActive(true);
    }
}
