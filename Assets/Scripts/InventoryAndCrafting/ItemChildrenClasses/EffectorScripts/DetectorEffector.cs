using System;
using System.Collections.Generic;
using UnityEngine;

public class DetectorEffector : Item
{
    public int maxRadius = 5;
    public float radiusGrowthTime = 1;
    public float detectorMoveThreshold = 0.1f;
    public GameObject detectorRadius;
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
            detectableMats[i].SetVector("_Center", transform.position);
        }
        detectorRadius.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);

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
        }
    }

    void SetDetectorRadiusSphereScale(float radius)
    {
        detectorRadius.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2); //Multiplication is temp.
    }

    void UpdateVisibility()
    {
        for (int i = 0; i < detectableMats.Count; i++)
        {
            detectableMats[i].SetFloat("_Radius", currentRadius);
            detectableMats[i].SetVector("_Center", transform.position);
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
    }

    new void Update()
    {
        base.Update();

        if (itemType != TypeTag.effector)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not effector!");
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