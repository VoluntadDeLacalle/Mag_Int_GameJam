using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPooler : SingletonMonoBehaviour<ItemPooler>
{
    [SerializeField]
    private List<GameObject> gameItems = new List<GameObject>();

    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < gameItems.Count; i++)
        {
            Item currentGameItem = gameItems[i].GetComponent<Item>();


            if (currentGameItem != null)
            {
                bool previousActive = gameItems[i].activeSelf;
                gameItems[i].SetActive(false);
                GameObject currentGameObject = Instantiate(gameItems[i], this.gameObject.transform);
                gameItems[i].SetActive(previousActive);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < gameItems.Count; i++)
        {
            Item currentItem = gameItems[i].GetComponent<Item>();
            if (currentItem == null)
            {
                continue;
            }

            if (!itemDictionary.ContainsKey(currentItem.itemName))
            {
                itemDictionary.Add(currentItem.itemName, currentItem);
            }
        }
    }

    public void ResetCurrentDictionary()
    {
        itemDictionary.Clear();
    }

    public GameObject GetItemByName(string itemName, Item currentItemRef)
    {
        if (!itemDictionary.ContainsKey(itemName))
        {
            Debug.LogError($"Item Dictionary does not contain current item: {itemName}");
            return null;
        }
        else
        {
            if (itemDictionary[itemName].gameObject.activeSelf)
            {
                return null;
            }
        }

        itemDictionary[itemName].GetComponent<Item>().LoadItem(currentItemRef);
        itemDictionary[itemName].gameObject.SetActive(true);
        currentItemRef.gameObject.SetActive(false);

        return itemDictionary[itemName].gameObject;
    }

    public void RepoolItemByName(string itemName)
    {
        if (!itemDictionary.ContainsKey(itemName))
        {
            Debug.LogError($"Item Dictionary does not contain current item: {itemName}");
            return;
        }

        itemDictionary[itemName].transform.parent = this.gameObject.transform;
        itemDictionary[itemName].gameObject.SetActive(false);
    }
}