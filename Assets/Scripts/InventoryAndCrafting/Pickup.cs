using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Inventory inventoryRef;
    public TMPro.TextMeshProUGUI itemHighlight;

    private List<Item> itemsInRange = new List<Item>();

    private void OnTriggerEnter(Collider other)
    {
        Item tempItem = null; 
        tempItem = other.GetComponent<Item>();
        if (tempItem != null)
        {
            if (!itemsInRange.Contains(other.GetComponent<Item>()))
            {
                itemsInRange.Add(other.GetComponent<Item>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Item tempItem = null;
        tempItem = other.GetComponent<Item>();
        if (tempItem != null)
        {
            if (itemsInRange.Contains(other.GetComponent<Item>()))
            {
                itemsInRange.Remove(other.GetComponent<Item>());
            }
        }
    }

    private void Update()
    {
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
                PickupItem();
            }
        }
    }

    public void PickupItem()
    {
        int randNumb = Random.Range(0, itemsInRange.Count);
        inventoryRef.AddToInventory(itemsInRange[randNumb]);
        itemsInRange.RemoveAt(randNumb);
    }
}
