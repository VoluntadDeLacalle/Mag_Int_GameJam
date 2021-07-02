using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button menuButton;
    public Button resumeButton;
    public Button quitButton;

    public GameObject pauseMenuUI;

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Start Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quiting game...");
        Application.Quit();
    }
}