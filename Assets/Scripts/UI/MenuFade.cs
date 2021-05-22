using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFade : MonoBehaviour
{
    public Animator anim;
    private string levelToLoad;

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // Player has McGuffin
        {
            FadeToStart("Start Menu");
        }
    }

    public void FadeToStart (string levelName)
    {
        levelToLoad = levelName;
        anim.SetTrigger("FadeToStart");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
