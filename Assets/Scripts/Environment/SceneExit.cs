using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneExit : MonoBehaviour
{
    public string nextSceneName = string.Empty;
    private bool isPlayerInRange = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() != null)
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() != null)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() != null)
        {
            isPlayerInRange = false;
        }
    }

    public void MoveToScene(string sceneName)
    {
        LevelManager.Instance.LoadNextActiveScene(sceneName);
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                LevelManager.Instance.LoadNextActiveScene(nextSceneName);
            }
        }
    }
}
