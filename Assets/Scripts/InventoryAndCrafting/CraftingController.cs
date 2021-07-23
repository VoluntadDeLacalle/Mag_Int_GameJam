using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour
{
    [Header("Crafting UI Variables")]
    public GameObject craftingPanel;
    public GameObject warningPanel;
    public GameObject primaryCraftingList;
    public GameObject modItemPrefab;
    public GameObject craftingModButtonPrefab;
    public GameObject resetCraftingButtonPrefab;

    private GameObject resetButton;
    private bool isActive = false;
    private int currentChassisIndex = -1;
    private bool warningCalled = false;
    private List<Item> chassisList = new List<Item>();
    private List<Item> effectorList = new List<Item>();
    private List<Item> gripList = new List<Item>();


    [Header("Item Viewer Variables")]
    public Camera itemViewerCamera;
    public ItemViewer itemViewer;
    public GameObject itemViewerPlayerModel;
    [Range(10,360)]
    public int playerModelAngularSpeed = 10;
    private Quaternion originalPlayerModelRotation;

    private ObjectPooler.Key primaryButtonUIKey = ObjectPooler.Key.PrimaryCraftingUIButtons;
    private ObjectPooler.Key secondaryButtonUIKey = ObjectPooler.Key.SecondaryCraftingUIButtons;

    private void Start()
    {
        originalPlayerModelRotation = itemViewerPlayerModel.transform.rotation;

        resetButton = Instantiate(resetCraftingButtonPrefab);
        resetButton.transform.SetParent(gameObject.transform, false);
        resetButton.SetActive(false);

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

                    GameObject visualChassis = Inventory.Instance.visualItemDictionary[tempChassisItem.gameObject];
                    visualChassis.transform.parent = itemViewer.handAttachment;
                    visualChassis.transform.localPosition = tempChassisItem.localHandPos;
                    visualChassis.transform.localRotation = Quaternion.Euler(tempChassisItem.localHandRot);
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
        DisableWholeVisualChassis();

        for (int i = 0; i < chassisList.Count; i++)
        {
            DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[i].gameObject]);
        }

        chassisList.Clear();
        effectorList.Clear();
        gripList.Clear();

        currentChassisIndex = -1;

        itemViewerPlayerModel.transform.rotation = originalPlayerModelRotation;
    }

    void DestroyPrimaryCraftingList()
    {
        foreach(Transform currentCraftingModButton in primaryCraftingList.GetComponentsInChildren<Transform>())
        {
            if (currentCraftingModButton.gameObject.GetComponent<PrimaryCraftingUIDescriptor>() == null)
            {
                continue;
            }

            foreach(Transform secondaryCraftingModButton in currentCraftingModButton.gameObject.GetComponent<PrimaryCraftingUIDescriptor>().secondaryCraftingList.GetComponentsInChildren<Transform>())
            {
                if (secondaryCraftingModButton.GetComponent<ItemUIDescriptor>() == null)
                {
                    continue;
                }

                secondaryCraftingModButton.gameObject.GetComponent<ItemUIDescriptor>().ApplyDescriptors(null, "None");
                secondaryCraftingModButton.gameObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                secondaryCraftingModButton.SetParent(ObjectPooler.GetPooler(secondaryButtonUIKey).gameObject.transform, false);
                secondaryCraftingModButton.gameObject.SetActive(false);
            }

            currentCraftingModButton.gameObject.GetComponent<PrimaryCraftingUIDescriptor>().SetButtonInformation("ERROR", "None", null);
            currentCraftingModButton.gameObject.GetComponent<PrimaryCraftingUIDescriptor>().ResetSecondaryCraftingRect();
            currentCraftingModButton.gameObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            currentCraftingModButton.SetParent(ObjectPooler.GetPooler(primaryButtonUIKey).gameObject.transform, false);
            currentCraftingModButton.gameObject.SetActive(false);
        }

        resetButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        resetButton.transform.SetParent(gameObject.transform, false);
        resetButton.SetActive(false);
    }

    void WarningResult()
    {
        warningCalled = true;
    }

    void DisableVisualItem(GameObject visualItem)
    {
        visualItem.SetActive(false);
        visualItem.transform.SetParent(Inventory.Instance.visualItemParent.transform);
    }

    void DisableWholeVisualChassis()
    {
        if (currentChassisIndex != -1)
        {
            for (int i = 0; i < chassisList[currentChassisIndex].chassisEffectorTransforms.Count; i++)
            {
                if (chassisList[currentChassisIndex].chassisEffectorTransforms[i].isOccupied)
                {
                    DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject]);
                }
            }

            if (chassisList[currentChassisIndex].chassisGripTransform.isOccupied)
            {
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject]);
            }

            Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].SetActive(false);
        }
    }

    void EnableVisualItem(GameObject visualItem, Transform visualParent, Vector3 localPosition, Quaternion localRotation)
    {
        visualItem.transform.parent = visualParent;
        visualItem.transform.localPosition = localPosition;
        visualItem.transform.rotation = localRotation;
        visualItem.SetActive(true);
    }

    void ChooseNewChassis(int index)
    {
        DestroyPrimaryCraftingList();

        float heightSpacing = 0;
        if (index == -1)
        {
            GameObject chassisNoneSecondaryCraftingList = SpawnPrimaryButton("Chassis", "None", null,ref heightSpacing);
            SpawnSecondaryButtons(Item.TypeTag.chassis, chassisNoneSecondaryCraftingList.transform);

            DisableWholeVisualChassis();

             currentChassisIndex = -1;
            return;
        }
        else
        {
            Item currentChassis = chassisList[index];
            currentChassisIndex = index;

            GameObject visualChassis = Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject];
            visualChassis.SetActive(true);

            GameObject chassisSecondaryCraftingList = 
                SpawnPrimaryButton("Chassis", currentChassis.itemName, currentChassis.inventorySprite, ref heightSpacing);
            SpawnSecondaryButtons(Item.TypeTag.chassis, chassisSecondaryCraftingList.transform);

            List<PrimaryCraftingUIDescriptor> resetEffectorPrimaryButtons = new List<PrimaryCraftingUIDescriptor>();

            for (int i = 0; i < currentChassis.chassisEffectorTransforms.Count; i++)
            {
                int currentEffectorTransformIndex = i;
                string effectorItemName = "None";
                Sprite effectorItemIcon = null;
                bool currentPointIsOccupied = false;

                if (currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].isOccupied)
                {
                    effectorItemName = currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].currentEffector.itemName;
                    effectorItemIcon = currentChassis.chassisEffectorTransforms[currentEffectorTransformIndex].currentEffector.inventorySprite;
                    currentPointIsOccupied = true;
                }

                GameObject effectorSecondaryCraftingList = 
                    SpawnPrimaryButton($"Effector { i + 1 }", effectorItemName, effectorItemIcon, ref heightSpacing);
                SpawnMultiComponentSecondaryButtons(Item.TypeTag.effector, effectorSecondaryCraftingList.transform, currentEffectorTransformIndex);

                if (currentPointIsOccupied)
                {
                    GameObject visualEffector = Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[currentEffectorTransformIndex].currentEffector.gameObject];
                    EnableVisualItem(visualEffector, visualChassis.transform, chassisList[currentChassisIndex].chassisEffectorTransforms[i].componentTransform.localPosition, visualChassis.transform.rotation);
                }

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

            if (chassisList[currentChassisIndex].chassisGripTransform.isOccupied)
            {
                GameObject visualGrip = Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject];
                EnableVisualItem(visualGrip, visualChassis.transform, chassisList[currentChassisIndex].chassisGripTransform.componentTransform.localPosition, visualChassis.transform.rotation);
            }
            
            PrimaryCraftingUIDescriptor resetGripPrimaryButton = gripSecondaryCraftingList.gameObject.GetComponentInParent<PrimaryCraftingUIDescriptor>();

            resetButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { ResetCurrentChassis(resetEffectorPrimaryButtons, resetGripPrimaryButton); });
            resetButton.transform.SetParent(primaryCraftingList.transform, false);
            resetButton.SetActive(true);
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
                    DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject]);

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
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject]);

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
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject]);

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
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].currentEffector.gameObject]);

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

                            DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisEffectorTransforms[i].currentEffector.gameObject]);
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

                    GameObject visualEffector = Inventory.Instance.visualItemDictionary[effectorList[effectorIndex].gameObject];
                    EnableVisualItem(visualEffector, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform, chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.localPosition, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform.rotation);

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

                        GameObject visualEffector = Inventory.Instance.visualItemDictionary[effectorList[effectorIndex].gameObject];
                        EnableVisualItem(visualEffector, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform, chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.localPosition, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform.rotation);

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

                GameObject visualEffector = Inventory.Instance.visualItemDictionary[effectorList[effectorIndex].gameObject];
                EnableVisualItem(visualEffector, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform, chassisList[currentChassisIndex].chassisEffectorTransforms[effectorTransformIndex].componentTransform.localPosition, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform.rotation);

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
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject]);

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
                DisableVisualItem(Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].chassisGripTransform.currentGrip.gameObject]);

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

                    GameObject visualGrip = Inventory.Instance.visualItemDictionary[gripList[gripIndex].gameObject];
                    EnableVisualItem(visualGrip, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform, chassisList[currentChassisIndex].chassisGripTransform.componentTransform.localPosition, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform.rotation);

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

                GameObject visualGrip = Inventory.Instance.visualItemDictionary[gripList[gripIndex].gameObject];
                EnableVisualItem(visualGrip, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform, chassisList[currentChassisIndex].chassisGripTransform.componentTransform.localPosition, Inventory.Instance.visualItemDictionary[chassisList[currentChassisIndex].gameObject].transform.rotation);

                parentButton.GetComponentInParent<PrimaryCraftingUIDescriptor>().SetButtonInformation("Grip", gripList[gripIndex].itemName, gripList[gripIndex].inventorySprite);
            }
        }

    }

    GameObject SpawnPrimaryButton(string title, string itemTitle, Sprite itemIcon,ref float heightSpacing)
    {
        GameObject obj = ObjectPooler.GetPooler(primaryButtonUIKey).GetPooledObject();
        obj.GetComponent<PrimaryCraftingUIDescriptor>().ResetSecondaryCraftingRect();
        obj.GetComponent<PrimaryCraftingUIDescriptor>().SetButtonInformation(title, itemTitle, itemIcon);
        obj.transform.SetParent(primaryCraftingList.transform, false);
        obj.GetComponent<PrimaryCraftingUIDescriptor>().MoveSecondaryCraftingRect(0, heightSpacing);
        heightSpacing += craftingModButtonPrefab.GetComponent<RectTransform>().rect.height;
        obj.SetActive(true);
        return obj.GetComponent<PrimaryCraftingUIDescriptor>().secondaryCraftingList;
    }

    void SpawnMultiComponentSecondaryButtons(Item.TypeTag typeTag, Transform secondaryButtonParent, int currentComponentIndexOnChassis)
    {
        GameObject noneObj = ObjectPooler.GetPooler(secondaryButtonUIKey).GetPooledObject();
        noneObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(null, "None");
        noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
        noneObj.transform.SetParent(secondaryButtonParent, false);
        noneObj.SetActive(true);

        switch (typeTag)
        {
            case Item.TypeTag.effector:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewEffector(-1, currentComponentIndexOnChassis, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });

                for (int i = 0; i < effectorList.Count; i++)
                {
                    int effectorIndex = i;
                    GameObject effectorObj = ObjectPooler.GetPooler(secondaryButtonUIKey).GetPooledObject();
                    effectorObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(effectorList[effectorIndex].inventorySprite, effectorList[effectorIndex].itemName);
                    effectorObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewEffector(effectorIndex, currentComponentIndexOnChassis, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });
                    effectorObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    effectorObj.transform.SetParent(secondaryButtonParent, false);
                    effectorObj.SetActive(true);
                }
                break;
        }
    }

    void SpawnSecondaryButtons(Item.TypeTag typeTag, Transform secondaryButtonParent)
    {
        GameObject noneObj = ObjectPooler.GetPooler(secondaryButtonUIKey).GetPooledObject();
        noneObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(null, "None");
        noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
        noneObj.transform.SetParent(secondaryButtonParent, false);
        noneObj.SetActive(true);

        switch (typeTag)
        {
            case Item.TypeTag.chassis:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewChassis(-1); });

                for (int i = 0; i < chassisList.Count; i++)
                {
                    int chassisIndex = i;
                    GameObject chassisObj = ObjectPooler.GetPooler(secondaryButtonUIKey).GetPooledObject();
                    chassisObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(chassisList[chassisIndex].inventorySprite, chassisList[chassisIndex].itemName);
                    chassisObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewChassis(chassisIndex); });
                    chassisObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    chassisObj.transform.SetParent(secondaryButtonParent, false);
                    chassisObj.SetActive(true);
                }
                break;
            case Item.TypeTag.grip:
                noneObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewGrip(-1, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });

                for (int i = 0; i < gripList.Count; i++)
                {
                    int gripIndex = i;
                    GameObject gripObj = ObjectPooler.GetPooler(secondaryButtonUIKey).GetPooledObject();
                    gripObj.GetComponent<ItemUIDescriptor>().ApplyDescriptors(gripList[gripIndex].inventorySprite, gripList[gripIndex].itemName);
                    gripObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { ChooseNewGrip(gripIndex, secondaryButtonParent.GetComponentInParent<PrimaryCraftingUIDescriptor>().gameObject); });
                    gripObj.GetComponentInChildren<Button>().onClick.AddListener(delegate { secondaryButtonParent.gameObject.SetActive(false); });
                    gripObj.transform.SetParent(secondaryButtonParent, false);
                    gripObj.SetActive(true);
                }
                break;
        }
    }

    void RotateItemViewer()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            itemViewerPlayerModel.transform.Rotate(new Vector3(0, -playerModelAngularSpeed * Time.fixedDeltaTime, 0));
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            itemViewerPlayerModel.transform.Rotate(new Vector3(0, playerModelAngularSpeed * Time.fixedDeltaTime, 0));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            itemViewerPlayerModel.transform.rotation = originalPlayerModelRotation;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            RotateItemViewer();
        }

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
