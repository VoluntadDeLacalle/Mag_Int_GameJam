using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        Talk,
        Activate,
        Restore
    };
    public GoalType goalType = GoalType.Location;
    public string objectiveDescription = "";
    public bool isCompleted = false;

    public int numbOfEvents = 0;
    public UnityEvent OnObjectiveComplete;

    public float activationRadius = 3f;
    public Vector3 targetWorldPosition = Vector3.zero;

    public string itemName = "";
    public Item.TypeTag itemType;

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

    public void AddGatheringScrap(string nItemName, int scrapAmount)
    {
        if (goalType == GoalType.Gather)
        {
            if (nItemName == itemName)
            {
                collectedAmount += scrapAmount;
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

    public void ActivateItem(string nItemName)
    {
        if (goalType == GoalType.Activate)
        {
            if (nItemName == itemName)
            {
                Complete();
            }
        }
    }

    public void RestoreItem(string nItemName)
    {
        if (goalType == GoalType.Restore)
        {
            if (nItemName == itemName)
            {
                Complete();
            }
        }
    }

    public void Complete()
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
    public GameObject questTextBackground;
    public TMPro.TextMeshProUGUI objectiveTextMesh;
    public GameObject objectiveTextBackground;
    public TMPro.TextMeshProUGUI generalInformationTextMesh;
    public GameObject questFlavorBackground;
    public TMPro.TextMeshProUGUI questFlavorTextMesh;
    public float questFlavorTimer = 2f;

    [Header("Compass UI Variables")]
    public Compass compassRef;
    public Sprite inactiveQuestMarker;
    public Sprite locationQuestMarker;

    [Header("Audio Variables")]
    public string questStartSFX = string.Empty;
    public string objectiveFinishSFX = string.Empty;
    public string questFinishSFX = string.Empty;

    [Header("Questing Variables")]
    public bool activateFirstOnStart = false;
    public float questActivationRadius = 3f;
    public GameObject markerGO;
    [SerializeField] private List<Quest> levelQuests = new List<Quest>();
    public int currentQuestIndex = 0;

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

        if (!HasNewQuest())
        {
            return;
        }

        levelQuests[currentQuestIndex].isActive = false;

        markerGO.SetActive(false);

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
            #if UNITY_EDITOR
                GUIStyle startStyle = new GUIStyle();
                startStyle.normal.textColor = Color.green;
                Handles.Label(currentStartPosition + Vector3.up, $"Quest: {levelQuests[i].questName}, start position.", startStyle);
            #endif

            for (int j = 0; j < levelQuests[i].GetGizmosInformation().Count; j++)
            {
                if (levelQuests[i].GetGizmosInformation()[j].goalType == Objective.GoalType.Location)
                {
                    Gizmos.color = Color.red;
                    Vector3 currentLocationPosition = levelQuests[i].GetGizmosInformation()[j].targetWorldPosition;
                    Gizmos.DrawWireSphere(currentLocationPosition, 0.5f);
                    Gizmos.DrawLine(currentLocationPosition, currentLocationPosition + Vector3.up);

                    Gizmos.DrawWireSphere(currentLocationPosition, levelQuests[i].GetGizmosInformation()[j].activationRadius);

                    #if UNITY_EDITOR
                        GUIStyle locationStyle = new GUIStyle();
                        locationStyle.normal.textColor = Color.red;
                        Handles.Label(currentLocationPosition + Vector3.up, $"Quest: {levelQuests[i].questName}, Objective {j + 1}, target.", locationStyle);
                    #endif
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

        markerGO.SetActive(false);
        compassRef.ResetQuestMarker();
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
        markerGO.SetActive(false);

        compassRef.ResetQuestMarker();
        SetObjectiveInfo();

        if (!HasNewQuest())
        {
            return;
        }

        if (!levelQuests[currentQuestIndex].isCompleted || levelQuests[currentQuestIndex].isActive)
        {
            if (objectiveFinishSFX != string.Empty)
            {
               AudioManager.instance.Play(objectiveFinishSFX);
            }
        }

        if (IsCurrentQuestActive())
        {
            CheckForPrecompletedItems();
        }
    }

    private void CheckForPrecompletedItems()
    {
        Objective currentObjective = levelQuests[currentQuestIndex].GetCurrentObjective();

        if (currentObjective != null)
        {
            if (currentObjective.goalType == Objective.GoalType.Gather)
            {
                if (currentObjective.itemType == Item.TypeTag.scrap)
                {
                    if (Inventory.Instance.amountOfScrap >= currentObjective.numberToCollect)
                    {
                        currentObjective.Complete();
                        return;
                    }
                    else
                    {
                        currentObjective.collectedAmount += Inventory.Instance.amountOfScrap;
                    }
                }
                else
                {
                    if (Inventory.Instance.Contains(currentObjective.itemName))
                    {
                        currentObjective.Complete();
                        return;
                    }
                }
            }
            else if (currentObjective.goalType == Objective.GoalType.Restore)
            {
                if (Inventory.Instance.Contains(currentObjective.itemName))
                {
                    if (Inventory.Instance.IsItemRestored(currentObjective.itemName))
                    {
                        currentObjective.Complete();
                        return;
                    }
                }
            }
        }
    }

    private void CurrentQuestComplete()
    {
        ResetQuestInfo();
        currentQuestIndex++;

        if (questFinishSFX != string.Empty)
        {
            AudioManager.instance.Play(questFinishSFX);
        }

        questFlavorTextMesh.text = $"Protocol Completed!";
        questFlavorBackground.SetActive(true);
        StartCoroutine(DeactivateQuestFlavor());
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
            if (currentObjective.itemType == Item.TypeTag.scrap)
            {
                objectiveTextMesh.text = $"{currentObjective.objectiveDescription}\n • {Inventory.Instance.amountOfScrap} out of {currentObjective.numberToCollect} {currentObjective.itemName}";
            }
            else
            {
                objectiveTextMesh.text = $"{currentObjective.objectiveDescription}\n • {currentObjective.collectedAmount} out of {currentObjective.numberToCollect} {currentObjective.itemName}";
            }
            
        }
        else
        {
            if (currentObjective.goalType == Objective.GoalType.Location && !compassRef.compassGO.activeInHierarchy)
            {
                compassRef.SetQuestMarker(locationQuestMarker, currentObjective.targetWorldPosition);
            }

            objectiveTextMesh.text = currentObjective.objectiveDescription;
        }

        objectiveTextBackground.SetActive(true);
    }

    void InitQuestInfo()
    {
        questTextMesh.text = levelQuests[currentQuestIndex].questName;
        questTextBackground.SetActive(true);

        SetObjectiveInfo();
    }
    
    void SpawnMarkerGO(Vector3 position, float radius, Color color)
    {
        if (currentQuestIndex != -1)
        {
            markerGO.transform.position = position;
            markerGO.transform.localScale = new Vector3(radius * 2, radius, radius * 2);
            markerGO.GetComponent<Renderer>().material.SetColor("_MainColor", color);
            markerGO.SetActive(true);
        }
    }

    void ResetQuestInfo()
    {
        questTextMesh.text = "";
        objectiveTextMesh.text = "";

        questTextBackground.SetActive(false);
        objectiveTextBackground.SetActive(false);

        compassRef.ResetQuestMarker();
    }

    IEnumerator DeactivateQuestFlavor()
    {
        yield return new WaitForSeconds(questFlavorTimer);

        questFlavorBackground.SetActive(false);
    }

    void TryStartQuest()
    {
        if (currentQuestIndex >= levelQuests.Count || currentQuestIndex == -1 || !Player.Instance.vThirdPersonInput.CanMove())
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
                        
                        compassRef.ResetQuestMarker();
                        InitQuestInfo();

                        if (questStartSFX != string.Empty)
                        {
                            AudioManager.instance.Play(questStartSFX);
                        }

                        questFlavorTextMesh.text = $"Protocol Initiated.\n{levelQuests[currentQuestIndex].questName}";
                        questFlavorBackground.SetActive(true);
                        StartCoroutine(DeactivateQuestFlavor());

                        if (levelQuests[currentQuestIndex].GetCurrentObjective().goalType == Objective.GoalType.Gather || levelQuests[currentQuestIndex].GetCurrentObjective().goalType == Objective.GoalType.Restore)
                        {
                            CheckForPrecompletedItems();
                        }

                        generalInformationTextMesh.text = "";
                        markerGO.SetActive(false);
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
            if (GetCurrentQuest() != null)
            {
                if (!markerGO.activeInHierarchy)
                {
                    SpawnMarkerGO(levelQuests[currentQuestIndex].questStartLocation, questActivationRadius, Color.green);
                }

                if (!compassRef.compassGO.activeInHierarchy)
                {
                    compassRef.SetQuestMarker(inactiveQuestMarker, GetCurrentQuest().questStartLocation);
                }
            }

            TryStartQuest();
        }
        else
        {
            Objective currentObjective = GetCurrentQuest().GetCurrentObjective();
            if (currentObjective != null)
            {
                if (currentObjective.goalType == Objective.GoalType.Location && !markerGO.activeInHierarchy)
                {
                    SpawnMarkerGO(currentObjective.targetWorldPosition, currentObjective.activationRadius, Color.red);
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
