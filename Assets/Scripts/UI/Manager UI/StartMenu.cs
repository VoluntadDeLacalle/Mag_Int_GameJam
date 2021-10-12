using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject warningPanel;
    public Button continueButton;
    public List<Button> mainMenuButtons = new List<Button>();
    private bool continueButtonOriginalState = false;
    private bool warningCalled = false;

    private void Start()
    {
        if (GameManager.Instance.HasSaveData())
        {
            continueButton.interactable = true;
            continueButtonOriginalState = true;
        }
        else
        {
            continueButton.interactable = false;
            continueButtonOriginalState = false;
        }
    }

    public void LoadPlay()
    {
        if (!GameManager.Instance.HasSaveData())
        {
            SceneManager.LoadScene(GameManager.Instance.GetLastSavedScene());
        }
        else
        {
            StartNewGame();
        }
    }

    public void LoadContinue()
    {
        SceneManager.LoadScene(GameManager.Instance.GetLastSavedScene());
    }

    private void ToggleMainMenuButtonInteraction(bool shouldToggle)
    {
        for (int i = 0; i < mainMenuButtons.Count; i++)
        {
            if (mainMenuButtons[i] == continueButton && shouldToggle)
            {
                mainMenuButtons[i].interactable = continueButtonOriginalState;
                continue;
            }

            mainMenuButtons[i].interactable = shouldToggle;
        }
    }

    void WarningResult()
    {
        warningCalled = true;
    }

    private void StartNewGame()
    {
        string warning = "This will erase your progress and reset the game!";
        if (!warningCalled)
        {
            warningPanel.SetActive(true);
            ToggleMainMenuButtonInteraction(false);
            warningPanel.GetComponent<WarningMessageUI>().SetWarning(warning, delegate { WarningResult(); }, delegate { WarningResult(); });
            warningPanel.GetComponent<WarningMessageUI>().AddWarningDelegate(delegate { StartNewGame(); warningCalled = false; ToggleMainMenuButtonInteraction(true); }, delegate { warningCalled = false; ToggleMainMenuButtonInteraction(true); });
        }
        else
        {
            GameManager.Instance.ResetSaveFiles();
            SceneManager.LoadScene(GameManager.Instance.GetLastSavedScene());
        }
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
