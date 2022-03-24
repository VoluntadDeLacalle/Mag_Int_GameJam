using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : SingletonMonoBehaviour<QuestUIManager>
{
    [Header("UI Variables")]
    public TMPro.TextMeshProUGUI questTextMesh;
    public GameObject questTextBackground;
    public TMPro.TextMeshProUGUI objectiveTextMesh;
    public GameObject objectiveTextBackground;
    public TMPro.TextMeshProUGUI generalInformationTextMesh;
    public GameObject questFlavorBackground;
    public TMPro.TextMeshProUGUI questFlavorTextMesh;

    [Header("Compass UI Variables")]
    public Compass compassRef;

    public void InitUI(ref TMPro.TextMeshProUGUI nQuestTextMesh, ref GameObject nQuestTextBackground, ref TMPro.TextMeshProUGUI nObjectiveTextMesh, ref GameObject nObjectiveTextBackground, ref TMPro.TextMeshProUGUI nGeneralInformationTextMesh, ref GameObject nQuestFlavorBackground, ref TMPro.TextMeshProUGUI nQuestFlavorTextMesh)
    {
        nQuestTextMesh = questTextMesh;
        nQuestTextBackground = questTextBackground;

        nObjectiveTextMesh = objectiveTextMesh;
        nObjectiveTextBackground = objectiveTextBackground;

        nGeneralInformationTextMesh = generalInformationTextMesh;
        nQuestFlavorBackground = questFlavorBackground;
        nQuestFlavorTextMesh = questFlavorTextMesh;
    }

    public void InitCompass(ref Compass nCompassRef)
    {
        nCompassRef = compassRef;
    }
}
