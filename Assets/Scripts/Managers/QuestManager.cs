using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Quest
{
    public string questName = "";
    public Vector3 questStartLocation = Vector3.zero;
    public bool isActive = false;
    public bool isCompleted = false;

    [HideInInspector] public UnityEvent OnQuestComplete;

    [SerializeField] public int currentObjective = 0;
    [SerializeField] private List<Objective> objectives = new List<Objective>();

    public void Activate()
    {
        if (!isCompleted)
        {
            isActive = true;
        }
    }

    public Objective GetCurrentObjective()
    {
        if (!isActive)
        {
            return null;
        }

        if (currentObjective >= objectives.Count)
        {
            return null;
        }
        else
        {
            return objectives[currentObjective];
        }
    }

    public List<Objective> GetGizmosInformation()
    {
        return objectives;
    }

    public void UpdateCurrentObjective()
    {
        if (!isActive)
        {
            return;
        }

        currentObjective++;
        if (currentObjective > objectives.Count - 1 && !isCompleted)
        {
            Complete();
        }
    }

    private void Complete()
    {
        if (isActive && !isCompleted && currentObjective > objectives.Count - 1)
        {
            isCompleted = true;
            isActive = false;

            OnQuestComplete?.Invoke();
        }
    }
}

[System.Serializable]
public class Objective
{
    public enum GoalType
    {
        Gather,
        Craft,
        Location,
        Talk
    };
    public GoalType goalType = GoalType.Location;
    public string objectiveDescription = "";
    public bool isCompleted = false;

    public UnityEvent OnObjectiveComplete;

    public float activationRadius = 3f;
    public Vector3 targetWorldPosition = Vector3.zero;

    public string itemName = "";

    public string npcName = "";
    public TextAsset npcDialogue = null;
    public Sprite npcSprite = null;

    public int numberToCollect = 0;
    public int collectedAmount = 0;

    public void AddGatheringItem(string nItemName)
    {
        if (goalType == GoalType.Gather)
        {
            if (nItemName == itemName)
            {
                collectedAmount++;
            }

            if (collectedAmount >= numberToCollect)
            {
                Complete();
            }
        }
    }

    public void CraftItem(string nItemName)
    {
        if (goalType == GoalType.Craft)
        {
            if (nItemName == itemName)
            {
                Complete();
            }
        }
    }

    public void CheckLocation(Vector3 currentPosition)
    {
        if (goalType == GoalType.Location)
        {
            if (Vector3.Distance(currentPosition, targetWorldPosition) < activationRadius)
            {
                Complete();
            }
        }
    }

    public KeyValuePair<TextAsset, Sprite> NPCTalkedTo(string nNPCName)
    {
        if (goalType == GoalType.Talk)
        {
            if (nNPCName == npcName)
            {
                Complete();
                return new KeyValuePair<TextAsset, Sprite>(npcDialogue, npcSprite);
            }
            else
            {
                return new KeyValuePair<TextAsset, Sprite>(null, null);
            }
        }

        return new KeyValuePair<TextAsset, Sprite>(null, null);
    }

    private void Complete()
    {
        if (!isCompleted)
        {
            OnObjectiveComplete?.Invoke();
            isCompleted = true;
        }
    }
}

[System.Serializable]
public struct QuestDataModel
{
    public bool isActive;
    public bool isQuestComplete;
    public List<bool> isObjectiveComplete;
    public List<int> objectiveItemsCollected;
}

public class QuestManager : SingletonMonoBehaviour<QuestManager>, ISaveable
{
    [Header("UI Variables")]
    public TMPro.TextMeshProUGUI questTextMesh;
    public TMPro.TextMeshProUGUI objectiveTextMesh;
    public TMPro.TextMeshProUGUI generalInformationTextMesh;

    [Header("Questing Variables")]
    public bool activateFirstOnStart = false;
    public float questActivationRadius = 3f;
    [SerializeField] private List<Quest> levelQuests = new List<Quest>();
    public int currentQuestIndex = 0;

    private GameObject questStartPE = null;
    private GameObject locationStartPE = null;
    private ObjectPooler.Key questStartKey = ObjectPooler.Key.QuestStartParticle;
    private ObjectPooler.Key locationTargetKey = ObjectPooler.Key.LocationTargetParticle;

    public object CaptureState()
    {
        List<QuestDataModel> tempQuestDataModels = new List<QuestDataModel>();
        for (int i = 0; i < levelQuests.Count; i++)
        {
            List<int> currentObjectiveItemsCollected = new List<int>();
            List<bool> currentObjectiveCompleteStatus = new List<bool>();

            for (int j = 0; j < levelQuests[i].GetGizmosInformation().Count; j++)
            {
                currentObjectiveItemsCollected.Add(levelQuests[i].GetGizmosInformation()[j].collectedAmount);
                currentObjectiveCompleteStatus.Add(levelQuests[i].GetGizmosInformation()[j].isCompleted);
            }

            QuestDataModel currentQuestDataModel = new QuestDataModel
            {
                isActive = levelQuests[i].isActive,
                isQuestComplete = levelQuests[i].isCompleted,
                isObjectiveComplete = new List<bool>(currentObjectiveCompleteStatus),
                objectiveItemsCollected = new List<int>(currentObjectiveItemsCollected),
            };

            tempQuestDataModels.Add(currentQuestDataModel);
        }

        if (currentQuestIndex > levelQuests.Count - 1 || currentQuestIndex == -1)
        {
            return new SaveData
            {
                currentQuestIndex = -1,
                currentObjectiveIndex = -1,
                questDataModels = new List<QuestDataModel>(tempQuestDataModels),
            };
        }
        else
        {
            return new SaveData
            {
                currentQuestIndex = currentQuestIndex,
                currentObjectiveIndex = levelQuests[currentQuestIndex].currentObjective,
                questDataModels = new List<QuestDataModel>(tempQuestDataModels),
            };
        }
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        levelQuests[currentQuestIndex].isActive = false;

        if (questStartPE != null)
        {
            questStartPE.SetActive(false);
            questStartPE = null;
        }

        if (locationStartPE != null)
        {
            locationStartPE.SetActive(false);
            locationStartPE = null;
        }

        for (int i = 0; i < saveData.questDataModels.Count; i++)
        {
            levelQuests[i].isActive = saveData.questDataModels[i].isActive;
            levelQuests[i].isCompleted = saveData.questDataModels[i].isQuestComplete;

            for (int j = 0; j < saveData.questDataModels[i].isObjectiveComplete.Count; j++)
            {
                levelQuests[i].GetGizmosInformation()[j].isCompleted = saveData.questDataModels[i].isObjectiveComplete[j];
                levelQuests[i].GetGizmosInformation()[j].collectedAmount = saveData.questDataModels[i].objectiveItemsCollected[j];
            }
        }

        currentQuestIndex = saveData.currentQuestIndex;
        if (currentQuestIndex > levelQuests.Count - 1 || currentQuestIndex == -1)
        {
            return;
        }

        levelQuests[currentQuestIndex].currentObjective = saveData.currentObjectiveIndex;

        if (levelQuests[currentQuestIndex].isActive)
        {
            levelQuests[currentQuestIndex].Activate();
            InitQuestInfo();
        }
        else
        {
            ResetQuestInfo();
        }
    }

    [System.Serializable]
    private struct SaveData
    {
        public int currentQuestIndex;
        public int currentObjectiveIndex;
        public List<QuestDataModel> questDataModels;
    };

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < levelQuests.Count; i++)
        {
            Gizmos.color = Color.green;
            Vector3 currentStartPosition = levelQuests[i].questStartLocation;
            Gizmos.DrawWireSphere(currentStartPosition, 0.5f);
            Gizmos.DrawLine(currentStartPosition, currentStartPosition + Vector3.up);

            Gizmos.DrawWireSphere(currentStartPosition, questActivationRadius);

            GUIStyle startStyle = new GUIStyle();
            startStyle.normal.textColor = Color.green;
            Handles.Label(currentStartPosition + Vector3.up, $"Quest: {levelQuests[i].questName}, start position.", startStyle);

            for (int j = 0; j < levelQuests[i].GetGizmosInformation().Count; j++)
            {
                if (levelQuests[i].GetGizmosInformation()[j].goalType == Objective.GoalType.Location)
                {
                    Gizmos.color = Color.red;
                    Vector3 currentLocationPosition = levelQuests[i].GetGizmosInformation()[j].targetWorldPosition;
                    Gizmos.DrawWireSphere(currentLocationPosition, 0.5f);
                    Gizmos.DrawLine(currentLocationPosition, currentLocationPosition + Vector3.up);

                    Gizmos.DrawWireSphere(currentLocationPosition, levelQuests[i].GetGizmosInformation()[j].activationRadius);

                    GUIStyle locationStyle = new GUIStyle();
                    locationStyle.normal.textColor = Color.red;
                    Handles.Label(currentLocationPosition + Vector3.up, $"Quest: {levelQuests[i].questName}, Objective {j + 1}, target.", locationStyle);
                }
            }
        }
    }

    new void Awake()
    {
        base.Awake();
        
        for(int i = 0; i < levelQuests.Count; i++)
        {
            levelQuests[i].OnQuestComplete.AddListener(CurrentQuestComplete);

            for (int j = 0; j < levelQuests[i].GetGizmosInformation().Count; j++)
            {
                levelQuests[i].GetGizmosInformation()[j].OnObjectiveComplete.AddListener(levelQuests[i].UpdateCurrentObjective);
                levelQuests[i].GetGizmosInformation()[j].OnObjectiveComplete.AddListener(CurrentObjectiveComplete);
            }
        }
    }

    private void Start()
    {
        if (activateFirstOnStart)
        {
            if (currentQuestIndex == 0)
            {
                levelQuests[0].Activate();
                InitQuestInfo();
            }   
        }
    }

    private void CurrentObjectiveComplete()
    {
        Debug.Log("Complete current Objective!");
        
        if (locationStartPE != null)
        {
            locationStartPE.SetActive(false);
            locationStartPE = null;
        }

        SetObjectiveInfo();
    }

    private void CurrentQuestComplete()
    {
        Debug.Log("Complete quest!");
        ResetQuestInfo();
        currentQuestIndex++;
    }

    void SetObjectiveInfo()
    {
        if (!HasNewQuest())
        {
            return;
        }

        if (levelQuests[currentQuestIndex].isCompleted || !levelQuests[currentQuestIndex].isActive)
        {
            return;
        }

        Objective currentObjective = levelQuests[currentQuestIndex].GetCurrentObjective();
        if (currentObjective == null)
        {
            return;
        }

        if (currentObjective.goalType == Objective.GoalType.Gather)
        {
            objectiveTextMesh.text = $"{currentObjective.objectiveDescription}\n{currentObjective.collectedAmount} out of {currentObjective.numberToCollect} {currentObjective.itemName}";
        }
        else
        {
            objectiveTextMesh.text = currentObjective.objectiveDescription;
        }
    }

    void InitQuestInfo()
    {
        questTextMesh.text = levelQuests[currentQuestIndex].questName;

        SetObjectiveInfo();
    }
    
    void SpawnQuestStartPE()
    {
        if (currentQuestIndex != -1)
        {
            questStartPE = ObjectPooler.GetPooler(questStartKey).GetPooledObject();
            questStartPE.transform.position = GetCurrentQuest().questStartLocation;
            questStartPE.SetActive(true);
        }
    }

    void SpawnLocationTargetPE()
    {
        if (currentQuestIndex != -1)
        {
            locationStartPE = ObjectPooler.GetPooler(locationTargetKey).GetPooledObject();
            locationStartPE.transform.position = GetCurrentQuest().GetCurrentObjective().targetWorldPosition;
            locationStartPE.SetActive(true);
        }
    }

    void ResetQuestInfo()
    {
        questTextMesh.text = "";
        objectiveTextMesh.text = "";
    }

    void TryStartQuest()
    {
        if (currentQuestIndex >= levelQuests.Count || currentQuestIndex == -1)
        {
            return;
        }

        Collider[] objectsInRange = Physics.OverlapSphere(levelQuests[currentQuestIndex].questStartLocation, questActivationRadius);
        for (int i = 0; i < objectsInRange.Length; i++)
        {
            if (objectsInRange[i].gameObject.GetComponentInChildren<Player>() != null)
            {
                if (!levelQuests[currentQuestIndex].isActive)
                {
                    generalInformationTextMesh.text = $"Press 'R' to start the quest: {levelQuests[currentQuestIndex].questName}";
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        levelQuests[currentQuestIndex].Activate();
                        InitQuestInfo();

                        generalInformationTextMesh.text = "";
                        if (questStartPE != null)
                        {
                            questStartPE.SetActive(false);
                            questStartPE = null;
                        }
                    }
                    break;
                }
            }
        }
    }

    public bool HasNewQuest()
    {
        return currentQuestIndex < levelQuests.Count;
    }

    public bool IsCurrentQuestActive()
    {
        if (currentQuestIndex > levelQuests.Count - 1 || currentQuestIndex == -1)
        {
            return false;
        }

        return levelQuests[currentQuestIndex].isActive;
    }

    public Quest GetCurrentQuest()
    {
        if (currentQuestIndex > levelQuests.Count - 1 || currentQuestIndex == -1)
        {
            return null;
        }

        return levelQuests[currentQuestIndex];
    }

    private void Update()
    {
        if (!HasNewQuest())
        {
            return;
        }

        if (!IsCurrentQuestActive())
        {
            if (questStartPE == null)
            {
                SpawnQuestStartPE();
            }

            TryStartQuest();
        }
        else
        {
            Objective currentObjective = GetCurrentQuest().GetCurrentObjective();
            if (currentObjective != null)
            {
                if (currentObjective.goalType == Objective.GoalType.Location && locationStartPE == null)
                {
                    SpawnLocationTargetPE();
                }
            }
            
            SetObjectiveInfo();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < levelQuests.Count; i++)
        {
            levelQuests[i].OnQuestComplete.RemoveAllListeners();

            for (int j = 0; j < levelQuests[i].GetGizmosInformation().Count; j++)
            {
                levelQuests[i].GetGizmosInformation()[j].OnObjectiveComplete.RemoveAllListeners();
            }
        }
    }
}
