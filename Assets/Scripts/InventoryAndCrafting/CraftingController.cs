using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour
{
    [Header("Crafting UI Variables")]
    public GameObject craftingPanel;
    public GameObject modItemPrefab;
    public GameObject primaryCraftingList;
    public GameObject craftingModButtonPrefab;
    public GameObject resetCraftingButtonPrefab;
    public GameObject warningPanel;

    private bool isActive = false;
    private int currentChassisIndex = -1;
    private bool warningCalled = false;


    [Header("Item Viewer Variables")]
    public Camera itemViewerCamera;
    public GameObject itemViewerTransform;

    private List<Item> chassisList = new List<Item>();

    private List<Item> effectorList = new List<Item>();

    private List<Item> gripList = new List<Item>();

    private void Start()
    {
        OnDisableCraftingPanel();
    }

    void OnEnableCraftingPanel()
    {
        for (int i = 0; i < Inventory.Instance.inventory.Count; i++)
        {
            if (Inventory.Instance.inventory[i] == null)
            {
                continue;
            }

            switch (Inventory.Instance.inventory[i].itemType)
            {
                case Item.TypeTag.chassis:
                    foreach (Item childrenItem in Inventory.Instance.inventory[i].gameObject.GetComponentsInChildren<Item>())
                    {
                        if (childrenItem.gameObject == Inventory.Instance.inventory[i].gameObject)
                        {
                            continue;
                        }

                        switch (childrenItem.itemType)
                        {
                            case Item.TypeTag.effector:
                                Item tempChildEffectorItem = childrenItem;
                                effectorList.Add(tempChildEffectorItem);
                                break;
                            case Item.TypeTag.grip:
                                Item tempChildGripItem = childrenItem;
                                gripList.Add(tempChildGripItem);
                                break;
                        }
                    }

                    Item tempChassisItem = Inventory.Instance.inventory[i];
                    chassisList.Add(tempChassisItem);
                    break;
                case Item.TypeTag.effector:
                    Item tempEffectorItem = Inventory.Instance.inventory[i];
                    effectorList.Add(tempEffectorItem);
                    break;
                case Item.TypeTag.grip:
                    Item tempGripItem = Inventory.Instance.inventory[i];
                    gripList.Add(tempGripItem);
                    break;
            }
        }
        ChooseNewChassis(-1);
    }

    void OnDisableCraftingPanel()
    {
        chassisList.Clear();
        effectorList.Clear();
        gripList.Clear();

        currentChassisIndex = -1;
    }

    void DestroyPrimaryCraftingList()
    {
        foreach(Transform currentCraftingModButton in primaryCraftingList.GetComponentsInChildren<Transform>())
        {
            if(currentCraftingModButton == primaryCraftingList.transform)
            {
                continue;
            }

            Destroy(currentCraftingModButton.gameObject);
        }
    }

    void WarningResult()
    {
        warningCalled = true;
    }

    void ChooseNewChassis(int index)
    {
        DestroyPrimaryCraftingList();
        float heightSpacing = 0;
        currentChassisIndex = -1;
        if (index == -1)
        {
            GameObject chassisNoneSecondaryCraftingList = SpawnPrimaryButton("Chassis", "None", null,ref heightSpacing);
            SpawnSecondaryButtons(Item.TypeTag.chassis, chassisNoneSecondaryCraftingList.transform);
            return;
        }
        else
        {
            Item currentChassis = chassisList[index];
            currentChassisIndex = index;

            GameObject chassisSecondaryCraftingList = 
                SpawnPrimaryButton("Chassis", currentChassis.itemName, currentChassis.inventorySprite, ref heightSpacing);
            SpawnSecondaryButtons(Item.TypeTag.chassis, chassisSecondaryCraftingList.transform);

            List<PrimaryCraftingUIDescriptor> resetEffectorPrimaryButtons = new List<PrimaryCraftingUIDescriptor>();

            for (int i = 0; i < currentChassis.chassisEffectorTransforms.Count; i++)
            {
                int currentEffectorTransformIndex = i;
                string effectorItemName = "None";
                Sprite effectorItemIcon = null;
                if (currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].isOccupied)
                {
                    effectorItemName = currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].currentEffector.itemName;
                    effectorItemIcon = currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].currentEffector.inventorySprite;
                }

                GameObject effectorSecondaryCraftingList = 
                    SpawnPrimaryButton($"Effector { i + 1 }", effectorItemName, effectorItemIcon, ref heightSpacing);
                SpawnMultiComponentSecondaryButtons(Item.TypeTag.effector, effectorSecondaryCraftingList.transform, currentEffectorTransformIndex);

                resetEffectorPrimaryButtons.Add(effectorSecondaryCraftingList.gameObject.GetComponentInParent<PrimaryCraftingUIDescriptor>());
            }

            string gripItemName = "None";
            Sprite gripItemIcon = null;
            if (currentChassis.chassisGripTransform.isOccupied)
            {
                gripItemName = currentChassis.chassisGripTransform.currentGrip.itemName;
                gripItemIcon = currentChassis.chassisGripTransform.currentGrip.inventorySprite;
            }
            GameObject gripSecondaryCraftingList = 
                SpawnPrimaryButton("Grip", gripItemName, gripItemIcon, ref heightSpacing);
            SpawnSecondaryButtons(Item.TypeTag.grip, gripSecondaryCraftingList.transform);

            PrimaryCraftingUIDescriptor resetGripPrimaryButton = gripSecondaryCraftingList.gameObject.GetComponentInParent<PrimaryCraftingUIDescriptor>();

            GameObject resetButton = Instantiate(resetCraftingButtonPrefab);
            resetButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { ResetCurrentChassis(resetEffectorPrimaryButtons, resetGripPrimaryButton); });
            resetButton.transform.SetParent(primaryCraftingList.transform, false);
        }
    }

    void ResetCurrentChassis(List<PrimaryCraftingUIDescriptor> effectorPrimaryButtons, PrimaryCraftingUIDescriptor gripPrimaryButton)
    {
        string warning = "You are about to remove all components from this chassis!";
        if (!warningCalled)
        {
            warningPanel.SetActive(true);
            warningPanel.GetComponent<WarningMessageUI>().SetWarning(warning, delegate { WarningResult(); }, delegate { WarningResult(); });
            warningPanel.GetComponent<WarningMessageUI>().AddWarningDelegate(delegate { ResetCurrentChassis(effectorPrimaryButtons, gripPrimaryButton); warningCalled = false; }, delegate { warningCalled = false; });
        }
        else
        {
            for (int i = 0; i < chassisList[currentChassisIndex].chassisEffectorTransforms.Count; i++)
            {
                effectorPrimaryButtons[i].SetButtonInformation($"Effector {i + 1}", "None", null);

                if (chassisList[currentChassisIndex].chassisEffectorTransforms[i].isOccupied)
                {
                    ///Removes effector for current slot if there is one.
                    chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject.transform.parent = null;
                    chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.isEquipped = false;
                    chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject.SetActive(false);

                    Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector);
                    chassisList[currentChassisIndex].chassisEffectorTransforms[i].ResetEffectorTransform();
                }
            }

            gripPrimaryButton.SetButtonInformation("Grip", "None", null);

            if (chassisList[currentChassisIndex].chassisGripTransform.isOccupied)
            {
                ///Removes grip from current slot if there is one.
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.transform.parent = null;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.isEquipped = false;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.SetActive(false);

                Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisGripTransform.currentGrip);
                chassisList[currentChassisIndex].chassisGripTransform.ResetGripTransform();
            }
        }
    }
    
    void ChooseNewEffector(int effectorIndex, int effectorTransformIndex, GameObject parentButton)
    {
        if (effectorIndex == -1)
        {   
            parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation($"Effector { effectorTransformIndex + 1 }", "None", null);
            
            if (chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].isOccupied)
            {
                ///Removes effector for current slot if there is one.
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject.transform.parent = null;
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.isEquipped = false;
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject.SetActive(false);
                
                Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector);
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].ResetEffectorTransform();
            }
            return;
        }
        else
        {
            if (chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].isOccupied)
            {
                if (chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject == effectorList[effectorIndex].gameObject)
                {
                    return;
                }

                ///Removes whatever effector was in the slot prior to a new one being added.
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject.transform.parent = null;
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.isEquipped = false;
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject.SetActive(false);

                Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector);
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].ResetEffectorTransform();
            }

            if (effectorList[effectorIndex].isEquipped)
            {
                Item attachedEffectorChassis = effectorList[effectorIndex].gameObject.GetComponentInParent<Item>();
                if (attachedEffectorChassis.gameObject == chassisList[currentChassisIndex].gameObject)
                {
                    for (int i = 0; i < chassisList[currentChassisIndex].chassisEffectorTransforms.Count; i++)
                    {
                        if (chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject == effectorList[effectorIndex].gameObject)
                        {
                            foreach (PrimaryCraftingUIDescriptor currentButton in primaryCraftingList.GetComponentsInChildren<PrimaryCraftingUIDescriptor>())
                            {
                                if(currentButton.titleTextMesh.text == $"Effector {i + 1}")
                                {
                                    currentButton.SetButtonInformation($"Effector {i + 1}", "None", null);
                                    break;
                                }
                            }

                            chassisList[currentChassisIndex].chassisEffectorTransforms[i].ResetEffectorTransform();
                            break;
                        }
                        else if(i == effectorTransformIndex)
                        {
                            continue;
                        }
                    }
                    
                    ///Moves effector from one slot on current chassis to currently selected one.
                    effectorList[effectorIndex].gameObject.transform.position = chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.position;
                    effectorList[effectorIndex].gameObject.transform.rotation = chassisList[currentChassisIndex].gameObject.transform.rotation;
                    chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].AddNewEffectorTransform(effectorList[effectorIndex]);

                    parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation($"Effector { effectorTransformIndex + 1 }", effectorList[effectorIndex].itemName, effectorList[effectorIndex].inventorySprite);
                    return;
                }
                else
                {
                    string warning = "The effector you are trying to use is attached to a different object!";
                    if (!warningCalled)
                    {
                        warningPanel.SetActive(true);
                        warningPanel.GetComponent<WarningMessageUI>().SetWarning(warning, delegate { WarningResult(); }, delegate { WarningResult(); });
                        warningPanel.GetComponent<WarningMessageUI>().AddWarningDelegate(delegate { ChooseNewEffector(effectorIndex, effectorTransformIndex, parentButton); warningCalled = false; }, delegate { warningCalled = false; });
                    }
                    else
                    {
                        for (int i = 0; i < chassisList.Count; i++)
                        {
                            if (chassisList[i].gameObject == attachedEffectorChassis.gameObject)
                            {
                                for(int j = 0; j < chassisList[i].chassisEffectorTransforms.Count; j++)
                                {
                                    if(chassisList[i].chassisEffectorTransforms[j].currentEffector.gameObject == effectorList[effectorIndex])
                                    {
                                        chassisList[i].chassisEffectorTransforms[j].ResetEffectorTransform();
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        ///Removes effector from other chassis and places it on this one.
                        effectorList[effectorIndex].transform.parent = chassisList[currentChassisIndex].gameObject.transform;
                        effectorList[effectorIndex].transform.position = chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.position;
                        effectorList[effectorIndex].transform.rotation = chassisList[currentChassisIndex].gameObject.transform.rotation;
                        chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].AddNewEffectorTransform(effectorList[effectorIndex]);

                        parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation($"Effector { effectorTransformIndex + 1 }", effectorList[effectorIndex].itemName, effectorList[effectorIndex].inventorySprite);
                        return;
                    }
                }
            }
            else
            {
                ///Adds effector into current slot, removes from inventory.
                effectorList[effectorIndex].transform.parent = chassisList[currentChassisIndex].gameObject.transform;
                effectorList[effectorIndex].gameObject.transform.position = chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.position;
                effectorList[effectorIndex].gameObject.transform.rotation = chassisList[currentChassisIndex].gameObject.transform.rotation;
                effectorList[effectorIndex].isEquipped = true;
                effectorList[effectorIndex].gameObject.SetActive(true);
                
                chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].AddNewEffectorTransform(effectorList[effectorIndex]);
                for (int i = 0; i < Inventory.Instance.inventory.Count; i++)
                {
                    if(Inventory.Instance.inventory[i].gameObject == effectorList[effectorIndex].gameObject)
                    {
                        Inventory.Instance.inventory.RemoveAt(i);
                        break;
                    }
                }

                parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation($"Effector { effectorTransformIndex + 1 }", effectorList[effectorIndex].itemName, effectorList[effectorIndex].inventorySprite);
            }
        }
    }

    void ChooseNewGrip(int gripIndex, GameObject parentButton)
    {
        if (gripIndex == -1)
        {
            parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation("Grip", "None", null);

            if (chassisList[currentChassisIndex].chassisGripTransform.isOccupied)
            {
                ///Removes grip from current slot if there is one.
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.transform.parent = null;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.isEquipped = false;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.SetActive(false);

                Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisGripTransform.currentGrip);
                chassisList[currentChassisIndex].chassisGripTransform.ResetGripTransform();
            }
            return;
        }
        else
        {
            if (chassisList[currentChassisIndex].chassisGripTransform.isOccupied)
            {
                if (chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject == gripList[gripIndex].gameObject)
                {
                    return;
                }

                ///Removes whatever effector was in the slot prior to a new one being added.
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.transform.parent = null;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.isEquipped = false;
                chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject.SetActive(false);

                Inventory.Instance.AddToInventory(chassisList[currentChassisIndex].chassisGripTransform.currentGrip);
                chassisList[currentChassisIndex].chassisGripTransform.ResetGripTransform();
            }

            if (gripList[gripIndex].isEquipped)
            {
                Item attachedGripChassis = gripList[gripIndex].gameObject.GetComponentInParent<Item>();

                string warning = "The grip you are trying to use is attached to a different object!";
                if (!warningCalled)
                {
                    warningPanel.SetActive(true);
                    warningPanel.GetComponent<WarningMessageUI>().SetWarning(warning, delegate { WarningResult(); }, delegate { WarningResult(); });
                    warningPanel.GetComponent<WarningMessageUI>().AddWarningDelegate(delegate { ChooseNewGrip(gripIndex, parentButton); warningCalled = false; }, delegate { warningCalled = false; });
                }
                else
                {
                    for (int i = 0; i < chassisList.Count; i++)
                    {
                        if (chassisList[i].gameObject == attachedGripChassis.gameObject)
                        {
                            chassisList[i].chassisGripTransform.ResetGripTransform();
                            break;
                        }
                    }
                    ///Removes grip from other chassis and places it on this one.
                    gripList[gripIndex].transform.parent = chassisList[currentChassisIndex].gameObject.transform;
                    gripList[gripIndex].transform.position = chassisList[currentChassisIndex].chassisGripTransform.componentTransform.position;
                    gripList[gripIndex].transform.rotation = chassisList[currentChassisIndex].gameObject.transform.rotation;
                    chassisList[currentChassisIndex].chassisGripTransform.AddNewGripTransform(gripList[gripIndex]);

                    parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation("Grip", gripList[gripIndex].itemName, gripList[gripIndex].inventorySprite);
                    return;
                }
            }
            else
            {
                ///Adds grip into current slot, removes from inventory.
                gripList[gripIndex].transform.parent = chassisList[currentChassisIndex].gameObject.transform;
                gripList[gripIndex].gameObject.transform.position = chassisList[currentChassisIndex].chassisGripTransform.componentTransform.position;
                gripList[gripIndex].gameObject.transform.rotation = chassisList[currentChassisIndex].gameObject.transform.rotation;
                gripList[gripIndex].isEquipped = true;
                gripList[gripIndex].gameObject.SetActive(true);

                chassisList[currentChassisIndex].chassisGripTransform.AddNewGripTransform(gripList[gripIndex]);
                for (int i = 0; i < Inventory.Instance.inventory.Count; i++)
                {
                    if (Inventory.Instance.inventory[i].gameObject == gripList[gripIndex].gameObject)
                    {
                        Inventory.Instance.inventory.RemoveAt(i);
                        break;
                    }
                }

                parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation("Grip", gripList[gripIndex].itemName, gripList[gripIndex].inventorySprite);
            }
        }

    }

    GameObject SpawnPrimaryButton(string title, string itemTitle, Sprite itemIcon,ref float heightSpacing)
    {
        GameObject obj = Instantiate(craftingModButtonPrefab);
        obj.GetComponent<PrimaryCraftingUIDescriptor>().SetButtonInformation(title, itemTitle, itemIcon);
        obj.transform.SetParent(primaryCraftingList.transform, false);
        obj.GetComponent<PrimaryCraftingUIDescriptor>().MoveSecondaryCraftingRect(0, heightSpacing);
        heightSpacing += craftingModButtonPrefab.GetComponent<RectTransform>().rect.height;
        return obj.GetComponent<PrimaryCraftingUIDescriptor>().secondaryCraftingList;
    }

    void SpawnMultiComponentSecondaryButtons(Item.TypeTag typeTag, Transform secondaryButtonParent, int currentComponentIndexOnChassis)
    {
        GameObject noneObj = Instantiate(modItemPrefab);
        noneObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(null, "None");
        noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
        noneObj.transform.SetParent(secondaryButtonParent, false);

        switch (typeTag)
        {
            case Item.TypeTag.effector:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewEffector(-1, currentComponentIndexOnChassis, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });

                for (int i = 0; i < effectorList.Count; i++)
                {
                    int effectorIndex = i;
                    GameObject effectorObj = Instantiate(modItemPrefab);
                    effectorObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(effectorList[effectorIndex].inventorySprite, effectorList[effectorIndex].itemName);
                    effectorObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewEffector(effectorIndex, currentComponentIndexOnChassis, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });
                    effectorObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    effectorObj.transform.SetParent(secondaryButtonParent, false);
                }
                break;
        }
    }

    void SpawnSecondaryButtons(Item.TypeTag typeTag, Transform secondaryButtonParent)
    {
        GameObject noneObj = Instantiate(modItemPrefab);
        noneObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(null, "None");
        noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
        noneObj.transform.SetParent(secondaryButtonParent, false);

        switch (typeTag)
        {
            case Item.TypeTag.chassis:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewChassis(-1); });

                for (int i = 0; i < chassisList.Count; i++)
                {
                    int chassisIndex = i;
                    GameObject chassisObj = Instantiate(modItemPrefab);
                    chassisObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(chassisList[chassisIndex].inventorySprite, chassisList[chassisIndex].itemName);
                    chassisObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewChassis(chassisIndex); });
                    chassisObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    chassisObj.transform.SetParent(secondaryButtonParent, false);
                }
                break;
            case Item.TypeTag.grip:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewGrip(-1, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });

                for (int i = 0; i < gripList.Count; i++)
                {
                    int gripIndex = i;
                    GameObject gripObj = Instantiate(modItemPrefab);
                    gripObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(gripList[gripIndex].inventorySprite, gripList[gripIndex].itemName);
                    gripObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewGrip(gripIndex, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });
                    gripObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    gripObj.transform.SetParent(secondaryButtonParent, false);
                }
                break;
        }
    }

    private void Update()
    {
        if (craftingPanel.activeSelf && !isActive)
        {
            isActive = true;
            OnEnableCraftingPanel();
        }
        else if (!craftingPanel.activeSelf && isActive)
        {
            isActive = false;
            OnDisableCraftingPanel();
        }
    }
}
