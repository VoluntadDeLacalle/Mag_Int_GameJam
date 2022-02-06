using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetAssist : MonoBehaviour
{
    private vThirdPersonCamera mainCamera;

    public GameObject aimTargetObj;
    public float localDistFromCam = 4.5f;

    private GameObject personalTargetObj;

    private void Start()
    {
        mainCamera = Player.Instance.vThirdPersonCamera;

        float posX = localDistFromCam / 6;
        float posY = localDistFromCam / 6;
        personalTargetObj = new GameObject();
        personalTargetObj.transform.parent = mainCamera.transform;
        personalTargetObj.transform.localPosition = new Vector3(posX, posY, localDistFromCam);
        personalTargetObj.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        aimTargetObj.transform.position = personalTargetObj.transform.position;
    }
}
