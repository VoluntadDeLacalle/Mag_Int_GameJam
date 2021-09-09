using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxTest : MonoBehaviour
{
    public TextAsset textFile;
    public Sprite talkerIcon;
    private bool inRange = false;

    private void Awake()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInChildren<vThirdPersonController>())
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
        inRange = false;
    }

    private void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Textbox.Instance.EnableTextbox(textFile, talkerIcon);
            }
        }
    }
}
