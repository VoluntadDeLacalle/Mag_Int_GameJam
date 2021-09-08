using BasicTools.ButtonInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : SingletonMonoBehaviour<Textbox>
{
    public TMPro.TextMeshProUGUI textMesh;

    [Header("Juice Variables")]
    public float typeTimer = 1;
    private float maxTypeTimer = 0;

    public char continueIcon;
    public float continueFlashTimer = 1;
    private float maxContinueFlashTimer = 0;

    public Image talkerIcon;
    public Animator talkerIconAnimator;
    public Animator textboxAnimator;

    private string textToType = "";
    private int textToTypeIndex = 0;
    private bool isTyping = false;
    private bool isTextboxEnabled = false;

    private List<string> textLines = new List<string>();
    private int count = 0;

    new void Awake()
    {
        base.Awake();
        maxTypeTimer = typeTimer;
        maxContinueFlashTimer = continueFlashTimer;
        talkerIconAnimator.SetBool("isTalking", false);
    }

    public void EnableTextbox(TextAsset textFile, Sprite newTalkerIcon)
    {
        if (isTextboxEnabled)
        {
            Debug.LogError("Textbox already active!");
            return;
        }

        textMesh.text = "";
        if (textFile != null)
        {
            string tempStr = textFile.text;
            tempStr = tempStr.Replace("~ ", "~");
            tempStr = tempStr.Replace("\n", "");
            tempStr = tempStr.Replace("\r", "");

            textLines = new List<string>(tempStr.Split('~'));
        }

        talkerIcon.sprite = newTalkerIcon;
        textboxAnimator.SetBool("isEnabled", true);
        PlayerController.Instance.ShouldMove(false);
    }

    private void StartFirstText()
    {
        SetText(textLines[0]);
        isTextboxEnabled = true;
    }

    private void DisableTextbox()
    {
        textMesh.text = "";
        isTextboxEnabled = false;
        textboxAnimator.SetBool("isEnabled", false);
    }

    private void Disabled()
    {
        count = 0;
        textLines.Clear();

        talkerIcon.sprite = null;

        typeTimer = maxTypeTimer;
        continueFlashTimer = maxContinueFlashTimer;

        PlayerController.Instance.ShouldMove(true);
    }

    string GetNextBox(string currentText)
    {
        string newText = "";
        string[] currentTextWords = currentText.Split(' ');

        for (int i = 0; i < currentTextWords.Length; i++)
        {
            string tempText = newText;

            if (i > 0)
            {
                tempText += $" {currentTextWords[i]} {continueIcon}";
            }
            else
            {
                tempText += $" {currentTextWords[i]} {continueIcon}";
            }
            
            textMesh.text = tempText;
            textMesh.ForceMeshUpdate();

            if (textMesh.isTextOverflowing)
            {
                string remainingWords = "";
                for (int j = i; j < currentTextWords.Length; j++)
                {
                    if (j > i)
                    {
                        remainingWords += $" {currentTextWords[j]}";
                    }
                    else
                    {
                        remainingWords += currentTextWords[j];
                    }
                }
                textLines.Insert(count + 1, remainingWords);

                break;
            }

            if (i > 0)
            {
                newText += $" {currentTextWords[i]}";
            }
            else
            {
                newText += currentTextWords[i];
            }
        }

        return newText;
    }

    void SetText(string text)
    {
        textToType = GetNextBox(text);
        textToTypeIndex = 0;

        textMesh.text = "";
        count++;

        isTyping = true;
        typeTimer = maxTypeTimer;

        continueFlashTimer = maxContinueFlashTimer;
        talkerIconAnimator.SetBool("isTalking", true);
    }

    void Type()
    {
        typeTimer -= Time.deltaTime;
        if (typeTimer <= 0)
        {
            textMesh.text += textToType[textToTypeIndex];
            textToTypeIndex++;
            typeTimer = maxTypeTimer;

            if (textMesh.text == textToType)
            {
                isTyping = false;
                textMesh.text += $" {continueIcon}";
                talkerIconAnimator.SetBool("isTalking", false);
            }
        }
    }

    void ContinueFlash()
    {
        continueFlashTimer -= Time.deltaTime;
        if (continueFlashTimer <= 0)
        {
            if (textMesh.text.Contains(continueIcon.ToString()))
            {
                textMesh.text = textMesh.text.Replace($" {continueIcon.ToString()}", "");
            }
            else
            {
                textMesh.text += $" {continueIcon}";
            }

            continueFlashTimer = maxContinueFlashTimer;
        }
    }

    private void Update()
    {
        if (!isTextboxEnabled || Time.timeScale < 0.1f)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping)
            {
                if (count < textLines.Count)
                {
                    SetText(textLines[count]);
                }
                else
                {
                    DisableTextbox();
                    return;
                }
            }
            else
            {
                textMesh.text = textToType;
                isTyping = false;
                textMesh.text += $" {continueIcon}";
                talkerIconAnimator.SetBool("isTalking", false);
            }

        }

        if (isTyping)
        {
            Type();
        }
        else
        {
            ContinueFlash();
        }
    }
}
