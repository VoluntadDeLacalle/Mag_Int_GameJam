using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIDescriptor : MonoBehaviour
{
    public UnityEngine.UI.Image currentImage;
    public TMPro.TextMeshProUGUI currentTextMesh;
    public void ApplyDescriptors(Sprite newSprite, string newTitle)
    {
        currentImage.sprite = newSprite;
        currentTextMesh.text = newTitle;
    }
}
