using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DetectorEffector : Item
{
    [Header("Detector Variables")]
    public int maxRadius = 5;
    public float radiusGrowthTime = 1;
    public float detectorMoveThreshold = 0.1f;
    public GameObject detectorRadiusObj;
    public List<Material> detectableMats = new List<Material>();

    [Header("Modifier Variables")]
    public int amplifiedMaxRadius = 10;

    private float currentRadius = 0;
    private int originalMaxRadius = 0;

    private void Awake()
    {        
        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", currentRadius);
            detectableMats[i].SetVector("_Center", detectorRadiusObj.transform.position);
        }

        originalMaxRadius = maxRadius;
    }

    public override void Activate()
    {
        if (currentRadius > 0)
        {
            UpdateVisibility();
        }
        
        if (Input.GetMouseButton(0))
        {
            if (QuestManager.Instance.IsCurrentQuestActive())
            {
                Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                if (currentObjective != null)
                {
                    currentObjective.ActivateItem(itemName);
                }
            }

            if (currentRadius < maxRadius - 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, maxRadius, radiusGrowthTime * Time.deltaTime);
                SetDetectorRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = maxRadius;
            SetDetectorRadiusSphereScale(maxRadius);
        }
        else
        {
            if(currentRadius > 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, 0, radiusGrowthTime * Time.deltaTime);
                SetDetectorRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = 0;
            SetDetectorRadiusSphereScale(0.0f);

            for (int i = 0; i < detectableMats.Count; i++)
            {
                detectableMats[i].SetFloat("_Radius", 0);
                detectableMats[i].SetVector("_Center", Vector3.zero);
            }
        }
    }

    void UpdateSphereNotAlive()
    {
        if (currentRadius > 0.1f)
        {
            currentRadius = Mathf.Lerp(currentRadius, 0, radiusGrowthTime * Time.deltaTime);
            SetDetectorRadiusSphereScale(currentRadius);
            return;
        }
        currentRadius = 0;
        SetDetectorRadiusSphereScale(0.0f);

        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", 0);
            detectableMats[i].SetVector("_Center", Vector3.zero);
        }
    }

    void SetDetectorRadiusSphereScale(float radius)
    {
        float radiusX = radius * (1 / transform.localScale.x);
        float radiusY = radius * (1 / transform.localScale.y);
        float radiusZ = radius * (1 / transform.localScale.z);

        detectorRadiusObj.transform.localScale = new Vector3(radiusX * 2, radiusY * 2, radiusZ * 2); //Multiplication is temp.
    }

    void UpdateVisibility()
    {
        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", currentRadius);
            detectableMats[i].SetVector("_Center", detectorRadiusObj.transform.position);
        }
    }

    public override void ModifyComponent(ModifierItem.ModifierType modifierType)
    {
        switch (modifierType)
        {
            case ModifierItem.ModifierType.Amplifier:
                maxRadius = amplifiedMaxRadius;
                break;
            case ModifierItem.ModifierType.Exploding:
                break;
            case ModifierItem.ModifierType.Reflector:
                break;
        }
    }

    public override void UnmodifyComponent(ModifierItem.ModifierType modifierType)
    {
        switch (modifierType)
        {
            case ModifierItem.ModifierType.Amplifier:
                maxRadius = originalMaxRadius;
                break;
            case ModifierItem.ModifierType.Exploding:
                break;
            case ModifierItem.ModifierType.Reflector:
                break;
        }
    }

    public override void OnUnequip()
    {
        foreach (ModifierItem.ModifierType modifierType in (ModifierItem.ModifierType[])Enum.GetValues(typeof(ModifierItem.ModifierType)))
        {
            UnmodifyComponent(modifierType);
        }

        currentRadius = 0;

        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", 0);
            detectableMats[i].SetVector("_Center", Vector3.zero);
        }
    }

    void Update()
    {
        if (itemType != TypeTag.effector)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not effector!");
        }

        if (!Player.Instance.IsAlive() || !Player.Instance.vThirdPersonInput.CanMove())
        {
            UpdateSphereNotAlive();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", 0);
            detectableMats[i].SetVector("_Center", Vector3.zero);
        }
    }
}