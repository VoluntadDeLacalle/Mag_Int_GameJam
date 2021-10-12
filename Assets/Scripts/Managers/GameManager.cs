using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>, ISaveable
{
    [Header("Save Files")]
    public string gameManagerSaveFile;
    public string levelManagerSaveFile;
    public string sceneSaveFile;

    [Header("Scenes")]
    public string mainMenuName = string.Empty;
    public List<string> sceneNames = new List<string>();

    [SerializeField]
    private int currentSavedScene = 0;
    private bool hasLoadedInitially = false;

    public object CaptureState()
    {
        return new SaveData
        {
            savedSceneIndex = currentSavedScene
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        currentSavedScene = saveData.savedSceneIndex;
    }

    [Serializable]
    private struct SaveData
    {
        public int savedSceneIndex;
    }

    new void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this.gameObject);
    }

    [ContextMenu("ResetSaveFile")]
    public void ResetSaveFiles()
    {
        SaveSystem.ResetSaveFile(gameManagerSaveFile);
        SaveSystem.ResetSaveFile(levelManagerSaveFile);
        SaveSystem.ResetSaveFile(sceneSaveFile);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name == mainMenuName && !hasLoadedInitially)
        {
            SaveSystem.Load(gameManagerSaveFile);
            hasLoadedInitially = true;
        }
        else
        {
            if (scene.name != mainMenuName)
            {
                SaveSystem.Load(levelManagerSaveFile);
                SaveSystem.Load(sceneSaveFile);
            }
        }
    }

    public void SaveGameManager()
    {
        SaveSystem.Save(gameManagerSaveFile);
    }

    public void SaveScene()
    {
        for (int i = 0; i < sceneNames.Count; i++)
        {
            if (sceneNames[i] == SceneManager.GetActiveScene().name)
            {
                currentSavedScene = i;
                break;
            }
        }

        SaveSystem.Save(sceneSaveFile);
        SaveSystem.Save(levelManagerSaveFile);
    }

    public bool HasSaveData()
    {
        bool returnValue = SaveSystem.DoesFileExist(gameManagerSaveFile) || SaveSystem.DoesFileExist(levelManagerSaveFile) || SaveSystem.DoesFileExist(sceneSaveFile);
        return returnValue;
    }

    public string GetLastSavedScene()
    {
        return sceneNames[currentSavedScene];
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name != mainMenuName)
        {
            SaveScene();
            SaveGameManager();
        }
        else
        {
            SaveGameManager();
        }

        if (Application.isEditor)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
        }
        else
        {
            Application.Quit();
        }
    }

    //private void OnApplicationQuit()
    //{
    //    if (Application.isEditor)
    //    {
    //        QuitGame();
    //    }
    //}

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
