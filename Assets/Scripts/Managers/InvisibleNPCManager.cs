using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleNPCManager : MonoBehaviour, ISaveable
{
    public List<InvisibleNPCEffectorActions> invisibleNPCActions = new List<InvisibleNPCEffectorActions>();
    public bool shouldStartDetecting = false;
    public bool hasCompletedAction = false;

    public object CaptureState()
    {
        return new SaveData
        {
            detectionStatus = shouldStartDetecting
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        shouldStartDetecting = saveData.detectionStatus;
    }

    [System.Serializable]
    private struct SaveData
    {
        public bool detectionStatus;
    }

    void CheckNPCStatus()
    {
        bool allNPCHit = true;
        if (invisibleNPCActions.Count == 0)
        {
            Debug.Log("NO NPCs REGISTERED.");
            return;
        }

        for (int i = 0; i < invisibleNPCActions.Count; i++)
        {
            if (!invisibleNPCActions[i].hasBeenHit && allNPCHit)
            {
                allNPCHit = false;
            }
        }

        if (allNPCHit)
        {
            Debug.Log("ALL NPCs enabled!");
            hasCompletedAction = true;
        }
    }

    void UpdateInvisibleNPCEnabledStatus(bool shouldEnable)
    {
        for (int i = 0; i < invisibleNPCActions.Count; i++)
        {
            SaveEnabledState enabledState = invisibleNPCActions[i].gameObject.GetComponent<SaveEnabledState>();
            
            if (enabledState.gameObject.activeSelf != shouldEnable)
            {
                enabledState.ShouldEnableObject(shouldEnable);
            }
        } 
    }

    private void Update()
    {
        UpdateInvisibleNPCEnabledStatus(shouldStartDetecting);

        if (shouldStartDetecting && !hasCompletedAction)
        {
            CheckNPCStatus();
        }
    }
}
