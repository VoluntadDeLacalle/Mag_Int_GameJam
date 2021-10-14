using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicNPCController : NPCCharacter
{
    public TextAsset basicTextFile = null;
    public Sprite talkerIcon = null;

    void TryTalk()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, talkRadius);
        if (collidersInRange.Length == 0)
        {
            return;
        }

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            if (collidersInRange[i].gameObject.GetComponentInChildren<Player>() != null)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (QuestManager.Instance.IsCurrentQuestActive())
                    {
                        Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                        if (currentObjective != null)
                        {
                            KeyValuePair<TextAsset, Sprite> textboxInput = currentObjective.NPCTalkedTo(characterName);
                            Textbox.Instance.EnableTextbox(textboxInput.Key, textboxInput.Value, false);
                        }
                    }
                    else
                    {
                        Textbox.Instance.EnableTextbox(basicTextFile, talkerIcon, false);
                    }
                }
            }
        }
    }

    private void Update()
    {
        TryTalk();
    }
}
