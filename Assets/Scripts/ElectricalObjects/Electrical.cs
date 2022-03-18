using UnityEngine;
using UnityEngine.Events;

public class Electrical : MonoBehaviour, ISaveable
{
    [Header("Electrical Parent Class Variables")]
    public string electricalLayerName = "Electrical";
    public bool poweredOnAwake = false;

    public UnityEvent OnActivated;
    public UnityEvent OnDeactived;

    protected bool isPowered = false;

    public object CaptureState()
    {
        return new SaveData
        {
            poweredState = IsPowered()
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        SetIsPowered(saveData.poweredState);
    }

    [System.Serializable]
    private struct SaveData
    {
        public bool poweredState;
    }

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

        GameManager.Instance.SaveScene();
    }

    public bool IsPowered()
    {
        return isPowered;
    }
}
