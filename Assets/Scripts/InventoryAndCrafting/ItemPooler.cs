using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPooler : SingletonMonoBehaviour<ItemPooler>
{
    [SerializeField]
    private List<GameObject> gameItems = new List<GameObject>();

    public Dictionary<string, GameObject> itemDictionary = new Dictionary<string, GameObject>();

    new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < gameItems.Count; i++)
        {
            Item currentGameItem = gameItems[i].GetComponent<Item>();

            if (currentGameItem != null)
            {
                if (!itemDictionary.ContainsKey(currentGameItem.itemName))
                {
                    itemDictionary.Add(currentGameItem.itemName, gameItems[i]);
                }
            }
        }
    }

    public GameObject InstantiateItemByName(string itemName)
    {
        if (!itemDictionary.ContainsKey(itemName))
        {
            Debug.LogError($"Item Dictionary does not contain current item: {itemName}");
            return null;
        }

        return Instantiate(itemDictionary[itemName]);
    }
}