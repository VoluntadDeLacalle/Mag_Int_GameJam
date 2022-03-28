using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestExternalManager : MonoBehaviour
{
    public void ActivateExternalObjective(string externalObjectiveName)
    {
        if (QuestManager.Instance.IsCurrentQuestActive())
        {
            Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
            if (currentObjective != null)
            {
                currentObjective.ExternalObjective(externalObjectiveName);
            }
        }
    }
}
