using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIDescriptor : MonoBehaviour
{
    public UnityEngine.UI.Image currentImage;
    public TMPro.TextMeshProUGUI currentTextMesh;
    public void ApplyDescriptors(Sprite newSprite, string newTitle)
    {
        currentTextMesh.text = newTitle;

        if (newSprite == null)
        {
            currentImage.enabled = false;
        }
        else
        {
            currentImage.enabled = true;
            currentImage.sprite = newSprite;
        }
    }
}
