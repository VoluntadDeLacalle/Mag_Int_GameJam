using UnityEngine;

public class ElectricalBox : Electrical
{
    public Color activatedColor;
    public Color deactivatedColor;

    public float activationTime = 5;

    public GameObject activeObject;
    public MeshRenderer meshRenderer;

    private void Awake()
    {
        SetIsPowered(true);
    }

    private void Update()
    {
        if (IsPowered())
        {
            meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, activatedColor, activationTime * Time.deltaTime);
        }
        else
        {
            meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, deactivatedColor, activationTime * Time.deltaTime);
        }
    }
}
