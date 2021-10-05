using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public void LoadPlayScene()
    {
        SceneManager.LoadScene(GameManager.Instance.GetLastSavedScene());
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
