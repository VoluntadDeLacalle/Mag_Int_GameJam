using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

/// <summary>
/// Uses code from "DapperDino" on Youtube: https://www.youtube.com/watch?v=f5GvfZfy3yk
/// </summary>

public class SaveSystem : MonoBehaviour
{
    public static string saveFileExtention = ".gav";

    [ContextMenu("Save")]
    public static void Save(string savePath)
    {
        var state = LoadFile(savePath);
        CaptureState(state);
        SaveFile(state, savePath);
    }

    [ContextMenu("Load")]
    public static void Load(string savePath)
    {
        var state = LoadFile(savePath);
        RestoreState(state);
    }

    public static void ResetSaveFile(string savePath)
    {
        string trueSavePath = $"{Application.persistentDataPath}/{savePath}{saveFileExtention}";
        if (File.Exists(trueSavePath))
        {
            File.Delete(trueSavePath);
        }
    }

    private static void SaveFile(object state, string savePath)
    {
        string trueSavePath = $"{Application.persistentDataPath}/{savePath}{saveFileExtention}";
        
        var surrogateSelector = new SurrogateSelector();
        surrogateSelector.AddSurrogate(typeof(Transform), new StreamingContext(StreamingContextStates.All), new TransformSurrogate());
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
        surrogateSelector.AddSurrogate(typeof(Item), new StreamingContext(StreamingContextStates.All), new ItemSurrogate());
        surrogateSelector.AddSurrogate(typeof(ChassisComponentTransform), new StreamingContext(StreamingContextStates.All), new ChassisComponentTransformSurrogate());
        surrogateSelector.AddSurrogate(typeof(ChassisGripTransform), new StreamingContext(StreamingContextStates.All), new ChassisGripTransformSurrogate());

        using (var stream = File.Open(trueSavePath, FileMode.Create))
        {
            var formatter = new BinaryFormatter { SurrogateSelector = surrogateSelector };
            formatter.Serialize(stream, state);
        }
    }

    private static Dictionary<string, object> LoadFile(string savePath)
    {
        string trueSavePath = $"{Application.persistentDataPath}/{savePath}{saveFileExtention}";

        var surrogateSelector = new SurrogateSelector();
        //Transform and Vector3
        surrogateSelector.AddSurrogate(typeof(Transform), new StreamingContext(StreamingContextStates.All), new TransformSurrogate());
        surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
        surrogateSelector.AddSurrogate(typeof(Item), new StreamingContext(StreamingContextStates.All), new ItemSurrogate());
        surrogateSelector.AddSurrogate(typeof(ChassisComponentTransform), new StreamingContext(StreamingContextStates.All), new ChassisComponentTransformSurrogate());
        surrogateSelector.AddSurrogate(typeof(ChassisGripTransform), new StreamingContext(StreamingContextStates.All), new ChassisGripTransformSurrogate());

        if (!File.Exists(trueSavePath))
        {
            return new Dictionary<string, object>();
        }

        using (FileStream stream = File.Open(trueSavePath, FileMode.Open))
        {
            var formatter = new BinaryFormatter { SurrogateSelector = surrogateSelector };
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    private static void CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>(true))
        {
            state[saveable.Id] = saveable.CaptureState();
        }
    }

    private static void RestoreState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>(true))
        {
            if (state.TryGetValue(saveable.Id, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }
}
