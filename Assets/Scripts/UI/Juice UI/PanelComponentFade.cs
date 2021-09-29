using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class PanelComponentFade : MonoBehaviour
{
    public enum FadeType
    {
        FadeOut,
        FadeIn
    }

    public FadeType fadeType = FadeType.FadeOut;
    public float duration;
    public bool onSceneLoad = false;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (onSceneLoad)
        {
            fadeType = FadeType.FadeOut;

            StartCoroutine(DoFade(canvasGroup.alpha, (int)fadeType));
        }
    }

    public void Fade(int currentFadeType)
    {
        fadeType = (FadeType)currentFadeType;

        StartCoroutine(DoFade(canvasGroup.alpha, (int)fadeType));

        Debug.Log(gameObject.name);
    }

    IEnumerator DoFade(float start, float end)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
