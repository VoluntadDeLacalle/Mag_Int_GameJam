using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] inventory = new Item[20];
    public GameObject[] inventoryImages = new GameObject[20];
    public int totalAllowedWeight = 0;

    public Sprite nullPlaceholderSprite;
    public UnityEngine.UI.Image selectedInspectorImage;
    public TMPro.TextMeshProUGUI selectedItemTitle;
    public TMPro.TextMeshProUGUI selectedItemDescription;
    public TMPro.TextMeshProUGUI selectedItemWeight;
    public TMPro.TextMeshProUGUI totalWeightText;
    public GameObject dropItemButton;
    private int dropIndex = -1;
    private int totalWeight = 0;

    public Item testItem;

    [ButtonAttribute("Add To Inventory", "AddToInventory")] [SerializeField]
    private bool _btnInventory;

    private void Awake()
    {
        totalWeightText.text = $"Total Weight: {totalWeight}kg/{totalAllowedWeight}kg";
    }

    private void OnEnable()
    {
        selectedInspectorImage.sprite = nullPlaceholderSprite;
        selectedItemTitle.text = "";
        selectedItemDescription.text = "";
        selectedItemWeight.text = "";
        dropItemButton.SetActive(false);
    }

    public void AddToInventory()
    {
        if (totalWeight + testItem.weight > totalAllowedWeight)
        {
            Debug.Log("TOO MUCH WEIGHT");
            return;
        }

        for (int i = 0; i < 20; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = testItem;
                inventoryImages[i].GetComponent<UnityEngine.UI.Image>().sprite = testItem.inventorySprite;
                totalWeight += testItem.weight;
                totalWeightText.text = $"Total Weight: {totalWeight}kg/{totalAllowedWeight}kg";
                return;
            }
        }
    }

    public void DropFromInventory()
    {
        totalWeight -= inventory[dropIndex].weight;
        totalWeightText.text = $"Total Weight: {totalWeight}kg/{totalAllowedWeight}kg";
        inventory[dropIndex] = null;
        inventoryImages[dropIndex].GetComponent<UnityEngine.UI.Image>().sprite = nullPlaceholderSprite;

        selectedInspectorImage.sprite = nullPlaceholderSprite;
        selectedItemTitle.text = "";
        selectedItemDescription.text = "";
        selectedItemWeight.text = "";
        dropItemButton.SetActive(false);

        //spawn item in world

    }

    public void ChangeInventoryInformation(GameObject clickedObject)
    {
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
            selectedItemWeight.text = "";
            dropItemButton.SetActive(false);
        }
        else
        {
            selectedInspectorImage.sprite = inventory[imageIndex].inventorySprite;

            switch (inventory[imageIndex].itemType)
            {
                case Item.TypeTag.chassis:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Chassis)";
                    break;
                case Item.TypeTag.grip:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Grip)";
                    break;
                case Item.TypeTag.activeComponent:
                    selectedItemTitle.text = $"{inventory[imageIndex].itemName} (Component)";
                    break;
            }
            
            selectedItemDescription.text = inventory[imageIndex].description;
            selectedItemWeight.text = $"{inventory[imageIndex].weight}kg";
            dropItemButton.SetActive(true);
            dropIndex = imageIndex;
        }
    }
}
