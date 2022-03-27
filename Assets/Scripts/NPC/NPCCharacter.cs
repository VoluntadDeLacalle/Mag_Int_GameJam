using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCharacter : MonoBehaviour, ISaveable
{
    public string characterName;
    public float talkRadius = 2.5f;

    public object CaptureState()
    {
        List<float> currentPosition = new List<float>();
        currentPosition.Add(transform.position.x);
        currentPosition.Add(transform.position.y);
        currentPosition.Add(transform.position.z);

        return new SaveData
        {
            npcPosition = currentPosition
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        transform.position = new Vector3(saveData.npcPosition[0], saveData.npcPosition[1], saveData.npcPosition[2]);
    }

    [System.Serializable]
    private struct SaveData
    {
        public List<float> npcPosition;
    }
}
