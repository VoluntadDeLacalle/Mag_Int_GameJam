using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public TMPro.TextMeshProUGUI itemHighlight;

    private List<Item> itemsInRange = new List<Item>();

    private void OnTriggerEnter(Collider other)
    {
        Item tempItem = null; 
        tempItem = other.GetComponent<Item>();
        if (tempItem != null)
        {
            if (tempItem.isEquipped != true && tempItem.GetComponentInParent<Player>() == null)
            {
                if (!itemsInRange.Contains(other.GetComponent<Item>()))
                {
                    itemsInRange.Add(other.GetComponent<Item>());
                }
            }
            else if (tempItem.isEquipped && tempItem.itemType == Item.TypeTag.grip && tempItem.GetComponentInParent<Player>() == null)
            {
                if (!itemsInRange.Contains(other.GetComponent<Item>()))
                {
                    itemsInRange.Add(other.GetComponent<Item>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Item tempItem = null;
        tempItem = other.GetComponent<Item>();
        if (tempItem != null)
        {
            if (tempItem.itemType == Item.TypeTag.grip && tempItem.isEquipped)
            {
                if (itemsInRange.Contains(other.GetComponent<Item>()))
                {
                    itemsInRange.Remove(other.GetComponent<Item>());
                }
            }
            else
            {
                if (itemsInRange.Contains(other.GetComponent<Item>()))
                {
                    itemsInRange.Remove(other.GetComponent<Item>());
                }
            }
        }
    }

    private void Update()
    {
        if (itemHighlight == null)
        {
            return;
        }

        if (itemsInRange.Count > 0 && itemHighlight.text == "")
        {
            itemHighlight.text = "Press 'E' to pick up item!";
        }
        else if (itemsInRange.Count == 0 && itemHighlight.text != "")
        {
            itemHighlight.text = "";
        }

        if (itemsInRange.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Time.timeScale != 0.0f || Player.Instance.vThirdPersonInput.CanMove())
                {
                    PickupItem();
                }
            }
        }
    }

    public void PickupItem()
    {
        int randNumb = Random.Range(0, itemsInRange.Count);
        Item tempItem = itemsInRange[randNumb];
        if (tempItem.isEquipped == true && tempItem.itemType == Item.TypeTag.grip)
        {
            Inventory.Instance.AddToInventory(tempItem.GetComponentInChildren<ChassisItem>());
        }
        else
        {
            Inventory.Instance.AddToInventory(tempItem);

        }
        itemsInRange.RemoveAt(randNumb);
    }
}
