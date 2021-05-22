using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDControls : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject craftingUI;

    public Button backButton;

    public static bool GameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(false);
        craftingUI.SetActive(false);

        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (craftingUI.activeSelf)
            {
                toggleOpt(craftingUI);
            }

            toggleOpt(inventoryUI);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (inventoryUI.activeSelf)
            {
                toggleOpt(inventoryUI);
            }

            toggleOpt(craftingUI);
        }
    }

    public void toggleOpt(GameObject uiName)
    {
        if(uiName.activeSelf)
        {
            uiName.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            uiName.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
