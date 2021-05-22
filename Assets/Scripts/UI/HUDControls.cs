using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDControls : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject craftingUI;
    public GameObject winScreenUI;

    void Start()
    {
        inventoryUI.SetActive(false);
        craftingUI.SetActive(false);
       //winScreenUI.SetActive(false);

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
