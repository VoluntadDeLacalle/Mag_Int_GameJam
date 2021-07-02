using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDControls : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject pickUpUI;
    public GameObject inventoryUI;
    public GameObject craftingUI;
    public GameObject winScreenUI;

    void Start()
    {
        inventoryUI.SetActive(false);
        craftingUI.SetActive(false);
        pauseUI.SetActive(false);
        winScreenUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (craftingUI.activeSelf)
            {
                toggleOpt(craftingUI);
            }
            if (pauseUI.activeSelf)
            {
                toggleOpt(pauseUI);
            }

            toggleOpt(inventoryUI);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (inventoryUI.activeSelf)
            {
                toggleOpt(inventoryUI);
            }
            if (pauseUI.activeSelf)
            {
                toggleOpt(pauseUI);
            }

            toggleOpt(craftingUI);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (craftingUI.activeSelf)
            {
                toggleOpt(craftingUI);
            }
            if (inventoryUI.activeSelf)
            {
                toggleOpt(inventoryUI);
            }

            toggleOpt(pauseUI);
        }

        if (pickUpUI.activeSelf && Time.timeScale == 0.0f)
        {
            pickUpUI.SetActive(false);
        }
        else if (!pickUpUI.activeSelf && Time.timeScale == 1.0f)
        {
            pickUpUI.SetActive(true);
        }
    }

    public void toggleOpt(GameObject uiName)
    {
        if(uiName.activeSelf)
        {
            uiName.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            uiName.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
