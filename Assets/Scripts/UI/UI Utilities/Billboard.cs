using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Player.Instance.vThirdPersonCamera.target.position, Vector3.up);
    }
}
