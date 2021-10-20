using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicNPCController : NPCCharacter
{
    public GameObject activationTextGO;
    public TMPro.TextMeshPro npcTextMesh;

    public TextAsset basicTextFile = null;
    public Sprite talkerIcon = null;

    void TryTalk()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, talkRadius);
        if (collidersInRange.Length == 0)
        {
            activationTextGO.SetActive(false);
            npcTextMesh.text = "";
            return;
        }

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            if (collidersInRange[i].gameObject.GetComponentInChildren<Player>() != null)
            {
                if (basicTextFile != null)
                {
                    activationTextGO.SetActive(true);
                    npcTextMesh.text = $"Press 'T' to talk to {characterName}";
                }
                else
                {
                    if (QuestManager.Instance.IsCurrentQuestActive())
                    {
                        Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                        if (currentObjective != null)
                        {
                            if (currentObjective.npcName == characterName)
                            {
                                activationTextGO.SetActive(true);
                                npcTextMesh.text = $"Press 'T' to talk to {characterName}";
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.T) && Player.Instance.vThirdPersonInput.CanMove())
                {
                    activationTextGO.SetActive(false);

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
