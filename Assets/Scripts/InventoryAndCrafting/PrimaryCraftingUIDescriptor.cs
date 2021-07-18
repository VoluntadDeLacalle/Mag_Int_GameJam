using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrimaryCraftingUIDescriptor : MonoBehaviour
{
    public TextMeshProUGUI titleTextMesh;
    public TextMeshProUGUI itemNameTextMesh;
    public Image buttonIcon;
    public GameObject secondaryCraftingList;

    public void SetButtonInformation(string title, string itemTitle, Sprite itemSprite)
    {
        titleTextMesh.text = title;
        itemNameTextMesh.text = itemTitle;

        if (itemSprite == null)
        {
            buttonIcon.enabled = false;
        }
        else
        {
            buttonIcon.enabled = true;
            buttonIcon.sprite = itemSprite;
        }       
    }

    public void ToggleSecondaryCrafting()
    {
        secondaryCraftingList.SetActive(!secondaryCraftingList.activeInHierarchy);
        Transform[] otherTransforms = gameObject.transform.parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < otherTransforms.Length; i++)
        {
            PrimaryCraftingUIDescriptor currentPCUIDescriptor = otherTransforms[i].gameObject.GetComponent<PrimaryCraftingUIDescriptor>();
            if(currentPCUIDescriptor == null)
            {
                continue;
            }
            else if(otherTransforms[i] == gameObject.transform)
            {
                continue;
            }

            currentPCUIDescriptor.secondaryCraftingList.SetActive(false);
        }
    }

    public void MoveSecondaryCraftingRect(float newX, float newY)
    {
        RectTransform currentRect = secondaryCraftingList.GetComponent<RectTransform>();
        currentRect.anchoredPosition = new Vector2(currentRect.anchoredPosition.x + newX, currentRect.anchoredPosition.y + newY);
    }
}
