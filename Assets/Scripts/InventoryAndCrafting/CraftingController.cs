using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingController : MonoBehaviour
{
    public GameObject craftingPanel;
    public GameObject modItemPrefab;
    private bool isActive = false;

    [Header("Item Viewer Variables")]
    private GameObject currentViewedChassis;
    private int currentViewedChassisIndex = -1;
    public Camera itemViewerCamera;
    public GameObject itemViewerTransform;

    public GameObject chassisUIParent;
    private List<Item> chassisList = new List<Item>();

    public GameObject effectorUIParent;
    private List<Item> effectorList = new List<Item>();

    public GameObject gripUIParent;
    private List<Item> gripList = new List<Item>();

    private void Start()
    {
        //OnDisableCraftingPanel();
    }

    void DeleteChildrenInGroup(GameObject parentGameObject)
    {
        List<Transform> tempChildrenList = new List<Transform>(parentGameObject.GetComponentsInChildren<Transform>());
        for (int i = 0; i < tempChildrenList.Count; i++)
        {
            if (tempChildrenList[i].gameObject == parentGameObject)
            {
                continue;
            }

            Destroy(tempChildrenList[i].gameObject);
        }
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
                    int tempChassisNumb = i;
                    Item tempChassisItem = Inventory.Instance.inventory[tempChassisNumb];
                    chassisList.Add(tempChassisItem);
                    GameObject tempChassisMod = Instantiate(modItemPrefab, chassisUIParent.transform);
                    tempChassisMod.GetComponent<ItemUIDescriptor>().ApplyDescriptors(tempChassisItem.inventorySprite, tempChassisItem.itemName);
                    tempChassisMod.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { ChassisViewer(tempChassisNumb); });
                    break;
                case Item.TypeTag.effector:
                    int tempEffectorNumb = i;
                    Item tempEffectorItem = Inventory.Instance.inventory[tempEffectorNumb];
                    effectorList.Add(tempEffectorItem);
                    GameObject tempEffectorMod = Instantiate(modItemPrefab, effectorUIParent.transform);
                    tempEffectorMod.GetComponent<ItemUIDescriptor>().ApplyDescriptors(tempEffectorItem.inventorySprite, tempEffectorItem.itemName);
                    tempEffectorMod.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { AddEffector(tempEffectorNumb); });
                    break;
                case Item.TypeTag.grip:
                    int tempGripNumb = i;
                    Item tempGripItem = Inventory.Instance.inventory[tempGripNumb];
                    gripList.Add(tempGripItem);
                    GameObject tempGripMod = Instantiate(modItemPrefab, gripUIParent.transform);
                    tempGripMod.GetComponent<ItemUIDescriptor>().ApplyDescriptors(tempGripItem.inventorySprite, tempGripItem.itemName);
                    tempGripMod.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { AddGrip(tempGripNumb); });
                    break;
            }
        }
    }

    void OnDisableCraftingPanel()
    {
        DeleteChildrenInGroup(chassisUIParent);
        DeleteChildrenInGroup(effectorUIParent);
        DeleteChildrenInGroup(gripUIParent);

        chassisList.Clear();
        effectorList.Clear();
        gripList.Clear();

        Destroy(currentViewedChassis);
        currentViewedChassisIndex = -1;
    }

    public void ChassisViewer(int index)
    {
        Debug.Log(index);
        if (currentViewedChassisIndex == index)
        {
            Destroy(currentViewedChassis);
            currentViewedChassisIndex = -1;
            return;
        }

        Destroy(currentViewedChassis);
        currentViewedChassisIndex = -1;

        currentViewedChassis = Instantiate(Inventory.Instance.inventory[index].gameObject);
        currentViewedChassisIndex = index;
        
        currentViewedChassis.transform.position = itemViewerTransform.transform.position;
        currentViewedChassis.transform.rotation = Quaternion.Euler(0, 0, 0);
        currentViewedChassis.SetActive(true);
    }

    public void AddEffector(int index)
    {
        Debug.Log(index);
        if (currentViewedChassisIndex == -1)
        {
            return;
        }
        else if (index == Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].currentOccupiedItemIndex) //Change this to search all effectors later
        {
            List<Item> tempItems = new List<Item>(currentViewedChassis.GetComponentsInChildren<Item>());
            for (int i = 0; i < tempItems.Count; i++)
            {
                if (tempItems[i].itemName == Inventory.Instance.inventory[index].itemName)
                {
                    Destroy(tempItems[i].gameObject);
                    break;
                }
            }

            Inventory.Instance.inventory[index].transform.parent = null;
            Inventory.Instance.inventory[index].isEquipped = false;
            Inventory.Instance.inventory[index].gameObject.SetActive(false);
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].isOccupied = false;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].currentOccupiedItemIndex = -1;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].currentEffector = null;
            return;
        }
        else if (Inventory.Instance.inventory[index].isEquipped)
        {
            return;
        }
        else
        {
            GameObject EffectorObj = Instantiate(Inventory.Instance.inventory[index].gameObject, currentViewedChassis.transform);
            EffectorObj.transform.position = currentViewedChassis.GetComponent<Item>().chassisEffectorTransforms[0].componentTransform.position;
            EffectorObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            EffectorObj.SetActive(true);

            Inventory.Instance.inventory[index].transform.parent = Inventory.Instance.inventory[currentViewedChassisIndex].transform;
            Inventory.Instance.inventory[index].transform.position = Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].componentTransform.position;
            Inventory.Instance.inventory[index].transform.rotation = Inventory.Instance.inventory[currentViewedChassisIndex].gameObject.transform.rotation;
            Inventory.Instance.inventory[index].isEquipped = true;
            Inventory.Instance.inventory[index].gameObject.SetActive(true);
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].isOccupied = true;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].currentOccupiedItemIndex = index;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisEffectorTransforms[0].currentEffector = Inventory.Instance.inventory[index];
        }
    }

    public void AddGrip(int index)
    {
        Debug.Log(index);
        if (currentViewedChassisIndex == -1)
        {
            return;
        }
        else if (index == Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.currentOccupiedItemIndex) //Change this to search all effectors later
        {
            List<Item> tempItems = new List<Item>(currentViewedChassis.GetComponentsInChildren<Item>());
            for (int i = 0; i < tempItems.Count; i++)
            {
                if (tempItems[i].itemName == Inventory.Instance.inventory[index].itemName)
                {
                    Destroy(tempItems[i].gameObject);
                    break;
                }
            }

            Inventory.Instance.inventory[index].transform.parent = null;
            Inventory.Instance.inventory[index].isEquipped = false;
            Inventory.Instance.inventory[index].gameObject.SetActive(false);
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.isOccupied = false;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.currentOccupiedItemIndex = -1;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.currentGrip = null;
            return;
        }
        else if (Inventory.Instance.inventory[index].isEquipped)
        {
            return;
        }
        else
        {
            GameObject EffectorObj = Instantiate(Inventory.Instance.inventory[index].gameObject, currentViewedChassis.transform);
            EffectorObj.transform.position = currentViewedChassis.GetComponent<Item>().chassisGripTransform.componentTransform.position;
            EffectorObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            EffectorObj.SetActive(true);

            Inventory.Instance.inventory[index].transform.parent = Inventory.Instance.inventory[currentViewedChassisIndex].transform;
            Inventory.Instance.inventory[index].transform.position = Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.componentTransform.position;
            Inventory.Instance.inventory[index].transform.rotation = Inventory.Instance.inventory[currentViewedChassisIndex].gameObject.transform.rotation;
            Inventory.Instance.inventory[index].isEquipped = true;
            Inventory.Instance.inventory[index].gameObject.SetActive(true);
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.isOccupied = true;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.currentOccupiedItemIndex = index;
            Inventory.Instance.inventory[currentViewedChassisIndex].chassisGripTransform.currentGrip = Inventory.Instance.inventory[index];
        }
    }

    private void Update()
    {
        if (craftingPanel.activeSelf && !isActive)
        {
            isActive = true;
            //OnEnableCraftingPanel();
        }
        else if (!craftingPanel.activeSelf && isActive)
        {
            isActive = false;
            //OnDisableCraftingPanel();
        }
    }
}
