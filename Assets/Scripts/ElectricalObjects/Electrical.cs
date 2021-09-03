using UnityEngine;

public class Electrical : MonoBehaviour
{
    public string electricalLayerName = "Electrical";

    protected bool isPowered = false;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(electricalLayerName);
    }

    public void SetIsPowered(bool shouldPower)
    {
        isPowered = shouldPower;
    }

    public bool IsPowered()
    {
        return isPowered;
    }
}
