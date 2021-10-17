using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelSpawnPoints
{
    public string fromSceneName = "";
    public Transform spawnPoint = null;
}

public class LevelManager : SingletonMonoBehaviour<LevelManager>, ISaveable
{
    public string levelMusic;

    [Header("Spawn Locations Variables")]
    public List<LevelSpawnPoints> levelSpawnPoints = new List<LevelSpawnPoints>();
    public Transform playerSpawnLocation = null;

    private GameObject playerSpawnPointInitialGO;

    [Header("Item Management Variables")]
    private List<string> itemNamesOnAwake = new List<string>();
    public List<string> addedItemNames = new List<string>();

    public object CaptureState()
    {
        return new SaveData
        {
            spawnPoints = levelSpawnPoints,
            lastPlayerTransform = playerSpawnLocation,
            addedItemNames = addedItemNames,
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        List<LevelSpawnPoints> tempSpawnPoints = saveData.spawnPoints;
        Transform tempPlayerLocation = saveData.lastPlayerTransform;
        if(true)
        {
            if (tempPlayerLocation.position != playerSpawnPointInitialGO.transform.position || 
                tempPlayerLocation.rotation != playerSpawnPointInitialGO.transform.rotation)
            {
                Player.Instance.Spawn(tempPlayerLocation);
            }
            else
            {
                Player.Instance.Spawn();
            }
        }

        addedItemNames = new List<string>(saveData.addedItemNames);

        for (int i = 0; i < addedItemNames.Count; i++)
        {
            if (!itemNamesOnAwake.Contains(addedItemNames[i]))
            {
                ItemPooler.Instance.InstantiateItemByName(addedItemNames[i]);
            }
        }
    }

    [System.Serializable]
    private struct SaveData
    {
        public List<LevelSpawnPoints> spawnPoints;
        public Transform lastPlayerTransform;
        public List<string> addedItemNames;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < levelSpawnPoints.Count; i++)
        {
            if (levelSpawnPoints[i].spawnPoint == null)
            {
                continue;
            }

            Vector3 currentStartPosition = levelSpawnPoints[i].spawnPoint.position;
            Gizmos.DrawWireSphere(currentStartPosition, 0.5f);
            Gizmos.DrawLine(currentStartPosition, currentStartPosition + Vector3.up);
        }
    }

    new void Awake()
    {
        base.Awake();

        foreach(var currentItem in FindObjectsOfType<Item>())
        {
            itemNamesOnAwake.Add(currentItem.itemName);
        }

        if (playerSpawnLocation == null)
        {
            playerSpawnPointInitialGO = new GameObject("Empty_PlayerSpawn_[IGNORE]");
            playerSpawnPointInitialGO.transform.parent = transform.root;
            playerSpawnLocation = playerSpawnPointInitialGO.transform;
        }
    }

    void Start()
    {
        string lastSceneName = GameManager.Instance.GetLastSavedScene();
        if (SceneManager.GetActiveScene().name != lastSceneName)
        {
            int index = -1;
            for (int i = 0; i < levelSpawnPoints.Count; i++)
            {
                if (lastSceneName == levelSpawnPoints[i].fromSceneName)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                Player.Instance.Spawn(levelSpawnPoints[index].spawnPoint);
            }
        }

        AudioManager.Get().CrossFadeTo(levelMusic, 2f);
    }

    public bool HasItemName(string itemName)
    {
        return addedItemNames.Contains(itemName);
    }

    public void AddItemName(string itemName)
    {
        addedItemNames.Add(itemName);
    }

    public void RemoveItemName(string itemName)
    {
        addedItemNames.Remove(itemName);
    }

    private void UnloadScene(string sceneName)
    {
        int index = -1;
        for (int i = 0; i < GameManager.Instance.sceneNames.Count; i++)
        {
            if (sceneName == GameManager.Instance.sceneNames[i])
            {
                index = i;
                break;
            }
        }

        playerSpawnLocation.position = levelSpawnPoints[index].spawnPoint.position;
        playerSpawnLocation.rotation = levelSpawnPoints[index].spawnPoint.rotation;

        
    }

    private void UnloadGame()
    {
        playerSpawnLocation.position = Player.Instance.origin.position;
        playerSpawnLocation.rotation = Player.Instance.origin.rotation;
    }

    public void LoadNextActiveScene(string sceneName)
    {
        Time.timeScale = 1.0f;
        //UnloadScene(sceneName);

        ItemPooler.Instance.ResetVisualItems();
        GameManager.Instance.SaveScene();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1.0f;
        UnloadGame();

        ItemPooler.Instance.ResetVisualItems();
        GameManager.Instance.SaveScene();
        SceneManager.LoadScene(GameManager.Instance.mainMenuName);
    }

    public void QuitGame()
    {
        UnloadGame();

        ItemPooler.Instance.ResetVisualItems();
        GameManager.Instance.QuitGame();
    }
}
