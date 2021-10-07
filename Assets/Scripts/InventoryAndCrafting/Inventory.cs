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
    public int amountOfScrap = 0;
    public GameObject inventoryItemPanel;
    public GameObject inventoryItemBox;
    [Range(3,6)]
    public int maxNumbofColumns = 0;
    public FlexibleGridLayout inventoryPanelGridLayout;

    [Header("UI Variables")]
    public GameObject inventoryPanel;
    public TMPro.TextMeshProUGUI scrapText;
    public Image selectedInspectorImage;
    public TMPro.TextMeshProUGUI selectedItemTitle;
    public TMPro.TextMeshProUGUI selectedItemDescription;
    public TMPro.TextMeshProUGUI selectedItemAttachedComponents;
    public GameObject dropItemButton;
    public GameObject equipItemButton;
    public GameObject unequipItemButton;
    public GameObject restoreItemButton;
    private bool isActive = false;
    //private int dropIndex = -1;
    //private int equipIndex = -1;

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
            InitInventory();
        }
        else if (!inventoryPanel.activeSelf && isActive)
        {
            isActive = false;
            ResetSelectedInfo();
            DeactivateCurrentInventoryView();
        }
    }

    void InitInventory()
    {
        DisplayScrapAmount();
        UpdateInventoryView();
    }

    void DisplayScrapAmount()
    {
        scrapText.text = $"Scrap: {amountOfScrap.ToString("D4")}";
    }

    void ResetSelectedInfo()
    {
        selectedInspectorImage.sprite = null;
        selectedInspectorImage.color = new Color(0, 0, 0, 0);
        selectedItemTitle.text = "";
        selectedItemDescription.text = "";
        selectedItemAttachedComponents.text = "";

        dropItemButton.SetActive(false);
        dropItemButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        equipItemButton.SetActive(false);
        equipItemButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        unequipItemButton.SetActive(false);
        unequipItemButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        restoreItemButton.SetActive(false);
        restoreItemButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
    }

    public void AddToInventory(Item newItem)
    {
        inventory.Add(newItem);

        if (newItem.itemType == Item.TypeTag.chassis && newItem.chassisGripTransform.IsGripTransformOccupied())
        {
            Item tempItem = newItem.chassisGripTransform.GetGripTransformItem();
            tempItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            tempItem.gameObject.GetComponent<Collider>().enabled = false;
            tempItem.gameObject.SetActive(false);
            tempItem.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            newItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            newItem.gameObject.GetComponent<Collider>().enabled = false;
            newItem.gameObject.SetActive(false);
            newItem.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        newItem.isObtained = true;

        Player.Instance.backpackFill.IncreaseBackpack(20);
    }

    public void LoadAddToInventory(Item newItem)
    {
        inventory.Add(newItem);

        if (newItem.itemType == Item.TypeTag.chassis && newItem.chassisGripTransform.IsGripTransformOccupied())
        {
            Item tempItem = newItem.chassisGripTransform.GetGripTransformItem();
            tempItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            tempItem.gameObject.GetComponent<Collider>().enabled = false;
            tempItem.gameObject.SetActive(false);
            tempItem.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            newItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            newItem.gameObject.GetComponent<Collider>().enabled = false;
            newItem.gameObject.SetActive(false);
            newItem.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void AddScrap(Item newItem)
    {
        ScrapItem currentScrap = newItem.gameObject.GetComponent<ScrapItem>();
        int scrapAmount = currentScrap.scrapAmount;

        if (amountOfScrap + scrapAmount > 9999)
        {
            amountOfScrap = 9999;
        }
        else
        {
            amountOfScrap += scrapAmount;
        }

        DisplayScrapAmount();


        //Possibly change this?
        Destroy(newItem.gameObject);
    }

    public void RemoveScrap(int itemToRestoreIndex, int amountToRemove)
    {
        if (amountOfScrap - amountToRemove < 0)
        {
            return;
        }
        else
        {
            amountOfScrap -= amountToRemove;
            DisplayScrapAmount();

            inventory[itemToRestoreIndex].isRestored = true;
            ChangeInventoryInformation(itemToRestoreIndex);
        }
    }

    public void DropFromInventory(int dropIndex)
    {
        if (dropIndex == -1)
        {
            return;
        }

        int tempEquippedIndex = -1;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemType == Item.TypeTag.chassis && inventory[i].isEquipped)
            {
                tempEquippedIndex = i;
                break;
            }
        }

        if (playerItemHandler.attachedItem != null && dropIndex == tempEquippedIndex)
        {
            UnequipItem(tempEquippedIndex);
        }

        if (inventory[dropIndex].itemType == Item.TypeTag.chassis && inventory[dropIndex].chassisGripTransform.IsGripTransformOccupied())
        {
            GameObject tempDropObj = inventory[dropIndex].chassisGripTransform.GetGripTransformItem().gameObject;
            tempDropObj.transform.position = dropTransform.position;
            tempDropObj.GetComponent<Rigidbody>().isKinematic = false;
            tempDropObj.GetComponent<Collider>().enabled = true;
            tempDropObj.SetActive(true);
        }
        else
        {
            inventory[dropIndex].gameObject.transform.position = dropTransform.position;
            inventory[dropIndex].gameObject.GetComponent<Rigidbody>().isKinematic = false;
            inventory[dropIndex].gameObject.GetComponent<Collider>().enabled = true;
            inventory[dropIndex].gameObject.SetActive(true);
        }

        inventory[dropIndex].isObtained = false;
        inventory.RemoveAt(dropIndex);
        if (dropIndex == inventory.Count)
        {
            if (inventory.Count == 0)
            {
                ResetSelectedInfo();
                dropIndex = -1;
            }
            else
            {
                ChangeInventoryInformation(dropIndex - 1);
            }
        }
        else
        {
            if (inventory.Count == 0)
            {
                ResetSelectedInfo();
                dropIndex = -1;
            }
            else
            {
                ChangeInventoryInformation(dropIndex);
            }
        }

        Player.Instance.backpackFill.DecreaseBackpack(20);
        if (inventory.Count == 0)
        {
            Player.Instance.backpackFill.DecreaseBackpack(300);
        }

        UpdateInventoryView();
    }

    public void ChangeInventoryInformation(int inventoryIndex)
    {
        ResetSelectedInfo();

        selectedInspectorImage.sprite = inventory[inventoryIndex].inventorySprite;
        selectedInspectorImage.color = new Color(1, 1, 1, 1);

        string itemType = (inventory[inventoryIndex].itemType).ToString();
        itemType = char.ToUpper(itemType[0]) + itemType.Substring(1);
        selectedItemTitle.text = $"{inventory[inventoryIndex].itemName} ({itemType})";

        if (inventory[inventoryIndex].isRestored)
        {
            if (inventory[inventoryIndex].itemType == Item.TypeTag.chassis)
            {
                if (inventory[inventoryIndex].isEquipped)
                {
                    equipItemButton.SetActive(false);
                    unequipItemButton.SetActive(true);
                    unequipItemButton.GetComponent<Button>().onClick.AddListener(delegate { UnequipItem(inventoryIndex); });
                }
                else
                {
                    unequipItemButton.SetActive(false);
                    equipItemButton.SetActive(true);
                    equipItemButton.GetComponent<Button>().onClick.AddListener(delegate { EquipItem(inventoryIndex); });
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
        }
        else
        {
            restoreItemButton.SetActive(true);
            restoreItemButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { RemoveScrap(inventoryIndex, inventory[inventoryIndex].restorationScrapAmount); });
            restoreItemButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Restore for {inventory[inventoryIndex].restorationScrapAmount} scrap";
        }
            
        selectedItemDescription.text = inventory[inventoryIndex].description;
        dropItemButton.SetActive(true);
        dropItemButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { DropFromInventory(inventoryIndex); });
    }

    public void EquipItem(int equipIndex)
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
        inventory[equipIndex].OnEquip();
        
        equipItemButton.SetActive(false);
        unequipItemButton.SetActive(true);
    }

    public void UnequipItem(int unequipIndex)
    {
        if (unequipIndex == -1)
        {
            return;
        }

        if (inventory[unequipIndex].chassisGripTransform.IsGripTransformOccupied())
        {
            playerItemHandler.UnequipItem(inventory[unequipIndex].chassisGripTransform.GetGripTransformItem());
        }
        else
        {
            playerItemHandler.UnequipItem(unequipIndex);
        }

        GrabberEffector grabberEffector = null;
        for (int i = 0; i < inventory[unequipIndex].chassisComponentTransforms.Count; i++)
        {
            if (inventory[unequipIndex].chassisComponentTransforms[i].IsComponentTransformOccupied())
            {
                grabberEffector = inventory[unequipIndex].chassisComponentTransforms[i].GetComponentTransformItem().gameObject.GetComponent<GrabberEffector>();

                if (grabberEffector != null)
                {
                    break;
                }
            }
        }

        if (grabberEffector != null)
        {
            if (grabberEffector.currentAttachedObj != null)
            {
                grabberEffector.DropCurrentObj();
            }
        }

        inventory[unequipIndex].isEquipped = false;
        inventory[unequipIndex].OnUnequip();

        unequipItemButton.SetActive(false);
        equipItemButton.SetActive(true);
    }

    void UpdateInventoryView()
    {
        DeactivateCurrentInventoryView();
        for (int i = 0; i < inventory.Count; i++)
        {
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
