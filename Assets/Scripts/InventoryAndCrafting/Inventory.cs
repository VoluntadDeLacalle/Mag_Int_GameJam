using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : SingletonMonoBehaviour<Inventory>
{
    [Header("Containers and External Variables")]
    public List<Item> inventory = new List<Item>();
    public Dictionary<GameObject, GameObject> visualItemDictionary = new Dictionary<GameObject, GameObject>();
    public GameObject visualItemParent;
    public PlayerItemHandler playerItemHandler;
    public Transform dropTransform;

    [Header("Inventory Variables")]
    public GameObject inventoryItemPanel;
    public GameObject inventoryItemBox;
    [Range(3,6)]
    public int maxNumbofColumns = 0;
    public FlexibleGridLayout inventoryPanelGridLayout;

    [Header("UI Variables")]
    public GameObject inventoryPanel;
    public Image selectedInspectorImage;
    public TMPro.TextMeshProUGUI selectedItemTitle;
    public TMPro.TextMeshProUGUI selectedItemDescription;
    public TMPro.TextMeshProUGUI selectedItemAttachedComponents;
    public GameObject dropItemButton;
    public GameObject equipItemButton;
    public GameObject unequipItemButton;
    private bool isActive = false;
    private int dropIndex = -1;
    private int equipIndex = -1;

    private ObjectPooler.Key inventoryItemUIKey = ObjectPooler.Key.InventoryItemUIButtons;

    private new void Awake()
    {
        base.Awake();
        inventoryPanelGridLayout.columns = maxNumbofColumns;
    }

    private void Update()
    {
        if (inventoryPanel.activeSelf && !isActive)
        {
            isActive = true;
            UpdateInventoryView();

            dropIndex = -1;
        }
        else if (!inventoryPanel.activeSelf && isActive)
        {
            isActive = false;
            ResetSelectedInfo();
            DeactivateCurrentInventoryView();
        }
    }

    void ResetSelectedInfo()
    {
        selectedInspectorImage.sprite = null;
        selectedInspectorImage.color = new Color(0, 0, 0, 0);
        selectedItemTitle.text = "";
        selectedItemDescription.text = "";
        selectedItemAttachedComponents.text = "";
        dropItemButton.SetActive(false);
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(false);
    }

    public void AddToInventory(Item newItem)
    {
        inventory.Add(newItem);

        newItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        newItem.gameObject.GetComponent<Collider>().enabled = false;
        newItem.gameObject.SetActive(false);
        newItem.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void DropFromInventory()
    {
        if (dropIndex == -1)
        {
            return;
        }

        if (playerItemHandler.attachedItem != null && dropIndex == equipIndex)
        {
            UnequipItem();
        }

        inventory[dropIndex].gameObject.transform.position = dropTransform.position;
        inventory[dropIndex].gameObject.GetComponent<Rigidbody>().isKinematic = false;
        inventory[dropIndex].gameObject.GetComponent<Collider>().enabled = true;
        inventory[dropIndex].gameObject.SetActive(true);

        inventory.RemoveAt(dropIndex);
        ResetSelectedInfo();
        dropIndex = -1;

        UpdateInventoryView();
    }

    public void ChangeInventoryInformation(int inventoryIndex)
    {
        selectedItemAttachedComponents.text = "";
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(false);
        equipIndex = -1;

        selectedInspectorImage.sprite = inventory[inventoryIndex].inventorySprite;
        selectedInspectorImage.color = new Color(1, 1, 1, 1);

        string itemType = (inventory[inventoryIndex].itemType).ToString();
        itemType = char.ToUpper(itemType[0]) + itemType.Substring(1);
        selectedItemTitle.text = $"{inventory[inventoryIndex].itemName} ({itemType})";

        if (inventory[inventoryIndex].itemType == Item.TypeTag.chassis)
        {
            equipIndex = inventoryIndex;

            if (inventory[inventoryIndex].isEquipped)
            {
                equipItemButton.SetActive(false);
                unequipItemButton.SetActive(true);
            }
            else
            {
                unequipItemButton.SetActive(false);
                equipItemButton.SetActive(true);
            }

            selectedItemAttachedComponents.text = "<b>Effectors</b>\n------------";

            for (int i = 0; i < inventory[inventoryIndex].chassisComponentTransforms.Count; i++)
            {
                if (inventory[inventoryIndex].chassisComponentTransforms[i].IsComponentTransformOccupied())
                {
                    selectedItemAttachedComponents.text += $"\n{inventory[inventoryIndex].chassisComponentTransforms[i].GetComponentTransformItem().itemName}";
                }
            }
            selectedItemAttachedComponents.text += "\n\n<b>Grip</b>\n------------";

            if (inventory[inventoryIndex].chassisGripTransform.IsGripTransformOccupied())
            {
                selectedItemAttachedComponents.text += $"\n{inventory[inventoryIndex].chassisGripTransform.GetGripTransformItem().itemName}";
            }
        }
            
        selectedItemDescription.text = inventory[inventoryIndex].description;
        dropItemButton.SetActive(true);
        dropIndex = inventoryIndex;
    }

    public void EquipItem()
    {
        if (equipIndex == -1)
        {
            return;
        }

        if (inventory[equipIndex].chassisGripTransform.IsGripTransformOccupied())
        {
            playerItemHandler.EquipItem(inventory[equipIndex].chassisGripTransform.GetGripTransformItem());
        }
        else
        {
            playerItemHandler.EquipItem(equipIndex);
        }
        inventory[equipIndex].isEquipped = true;
        
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(true);
    }

    public void UnequipItem()
    {
        if (equipIndex == -1)
        {
            return;
        }

        if (inventory[equipIndex].chassisGripTransform.IsGripTransformOccupied())
        {
            playerItemHandler.UnequipItem(inventory[equipIndex].chassisGripTransform.GetGripTransformItem());
        }
        else
        {
            playerItemHandler.UnequipItem(equipIndex);
        }
        inventory[equipIndex].isEquipped = false;

        unequipItemButton.SetActive(false);
        equipItemButton.SetActive(true);
    }

    void UpdateInventoryView()
    {
        DeactivateCurrentInventoryView();
        for (int i = 0; i < inventory.Count; i++)
        {
            //if (inventory[i].itemType != Item.TypeTag.chassis && inventory[i].isEquipped)
            //{
            //    continue;
            //}

            GameObject currentItemBox = ObjectPooler.GetPooler(inventoryItemUIKey).GetPooledObject();
            currentItemBox.transform.SetParent(inventoryItemPanel.transform, false);
            int currentIndex = i;
            
            currentItemBox.GetComponentInChildren<Image>().sprite = inventory[i].inventorySprite;
            currentItemBox.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChangeInventoryInformation(currentIndex); });
            currentItemBox.SetActive(true);
        }
        inventoryPanelGridLayout.cellSize.y = inventoryPanelGridLayout.cellSize.x;
    }

    void DeactivateCurrentInventoryView()
    {
        foreach(Transform currentTrans in inventoryItemPanel.GetComponentsInChildren<Transform>())
        {
            if(currentTrans == inventoryItemPanel.GetComponent<Transform>())
            {
                continue;
            }

            currentTrans.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            currentTrans.gameObject.GetComponentInChildren<Image>().sprite = null;
            currentTrans.gameObject.SetActive(false);
            currentTrans.SetParent(ObjectPooler.GetPooler(inventoryItemUIKey).gameObject.transform, false);
        }
    }
}
