using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : SingletonMonoBehaviour<Crosshair>
{
    public Ray GetCrosshairPosition()
    {
        vThirdPersonCamera camera = Player.Instance.vThirdPersonCamera;
        float posX = GetComponent<Image>().rectTransform.anchoredPosition.x;
        float posY = GetComponent<Image>().rectTransform.anchoredPosition.y;

        Ray crosshairRay = camera.ScreenPointToRay(new Vector3(posX, posY, 0));
        return crosshairRay;
    }
}
