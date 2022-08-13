using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button menuButton;
    public Button resumeButton;
    public Button quitButton;

    public GameObject pauseMenuUI;

    void OnEnable()
    {
        if (GameManager.Instance == null || GameManager.Instance.inputManager == null)
        {
            return;
        }

        if (GameManager.Instance.inputManager.currentControlScheme != "Keyboard")
        {
            StartCoroutine(SelectButtonLater());
        }
    }

    IEnumerator SelectButtonLater()
    {
        yield return null;
        resumeButton.Select();
    }

    void OnDisable()
    {
        StopCoroutine(SelectButtonLater());
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        LevelManager.Instance.LoadMainMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Quiting game...");
        LevelManager.Instance.QuitGame();
    }
}