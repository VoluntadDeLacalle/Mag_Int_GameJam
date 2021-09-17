using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthEffector : Item
{
    [Header("Stealth Variables")]
    public int maxRadius = 5;
    public float radiusGrowthTime = 1;
    public GameObject stealthRadiusObj;
    public List<Material> stealthMats = new List<Material>();

    [Header("Modifier Variables")]
    public int amplifiedMaxRadius = 10;

    private float currentRadius = 0;
    private int originalMaxRadius = 0;

    private void Awake()
    {
        for (int i = 0; i < stealthMats.Count; i++)
        {
            stealthMats[i].SetFloat("_StealthRadius", currentRadius);
            stealthMats[i].SetVector("_StealthCenter", transform.position);
        }

        originalMaxRadius = maxRadius;
    }

    public override void Activate()
    {
        if (currentRadius > 0)
        {
            UpdateActiveRadius();
        }

        if (Input.GetMouseButton(0))
        {
            if (currentRadius < maxRadius - 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, maxRadius, radiusGrowthTime * Time.deltaTime);
                SetStealthRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = maxRadius;
            SetStealthRadiusSphereScale(maxRadius);
        }
        else
        {
            if (currentRadius > 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, 0, radiusGrowthTime * Time.deltaTime);
                SetStealthRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = 0;
            SetStealthRadiusSphereScale(0.0f);
        }
    }

    void SetStealthRadiusSphereScale(float radius)
    {
        stealthRadiusObj.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2); //Multiplication is temp.
    }

    void UpdateActiveRadius()
    {
        for (int i = 0; i < stealthMats.Count; i++)
        {
            stealthMats[i].SetFloat("_StealthRadius", currentRadius);
            stealthMats[i].SetVector("_StealthCenter", transform.position);
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
        for (int i = 0; i < stealthMats.Count; i++)
        {
            stealthMats[i].SetFloat("_StealthRadius", 0);
            stealthMats[i].SetVector("_StealthCenter", Vector3.zero);
        }
    }
}
