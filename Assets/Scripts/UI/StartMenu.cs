using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public string loadSceneName;    

    public Button startButton;
    public Button optionsButton;
    public Button quitButton;
    public Button backButton;

    public GameObject optionsUI;

    public void Start()
    {
        optionsUI.SetActive(false);
    }

    public void NewGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(loadSceneName);
    }

    public void Options()
    {
        
        if (optionsUI.activeSelf)
        {
            optionsUI.SetActive(false);
        }
        else
        {
            optionsUI.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
