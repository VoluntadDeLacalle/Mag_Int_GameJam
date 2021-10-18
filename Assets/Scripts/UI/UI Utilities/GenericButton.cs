using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericButton : MonoBehaviour
{
    [SerializeField] public string menuClick = string.Empty;

    public void PlayClickSFX()
    {
        if (menuClick != string.Empty)
        {
            AudioManager.Get().Play(menuClick);
        }
    }
}
