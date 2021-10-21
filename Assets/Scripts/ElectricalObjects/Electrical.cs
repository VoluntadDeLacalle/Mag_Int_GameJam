using UnityEngine;
using UnityEngine.Events;

public class Electrical : MonoBehaviour
{
    [Header("Electrical Parent Class Variables")]
    public string electricalLayerName = "Electrical";
    public bool poweredOnAwake = false;

    public UnityEvent OnActivated;
    public UnityEvent OnDeactived;

    protected bool isPowered = false;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(electricalLayerName);

        if (poweredOnAwake)
        {
            isPowered = true;
        }
    }

    public void SetIsPowered(bool shouldPower)
    {
        isPowered = shouldPower;

        if (shouldPower)
        {
            OnActivated?.Invoke();
        }
        else
        {
            OnDeactived?.Invoke();
        }
    }

    public bool IsPowered()
    {
        return isPowered;
    }
}
