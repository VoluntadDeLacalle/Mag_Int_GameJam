using System;
using UnityEngine;

public class EMPEffector : Item
{
    public int maxRadius = 5;
    public float radiusGrowthTime = 1;
    public GameObject EMPRadius;

    [Header("Modifier Variables")]
    public int amplifiedMaxRadius = 10;

    private float currentRadius = 0;
    private int originalMaxRadius = 0;

    private void Awake()
    {
        originalMaxRadius = maxRadius;
    }

    public override void Activate()
    {
        if (currentRadius > 0)
        {
            UpdateEMPDetection();
        }

        if (Input.GetMouseButton(0))
        {
            if (currentRadius < maxRadius - 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, maxRadius, radiusGrowthTime * Time.deltaTime);
                SetEMPRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = maxRadius;
            SetEMPRadiusSphereScale(maxRadius);
        }
        else
        {
            if (currentRadius > 0.1f)
            {
                currentRadius = Mathf.Lerp(currentRadius, 0, radiusGrowthTime * Time.deltaTime);
                SetEMPRadiusSphereScale(currentRadius);
                return;
            }
            currentRadius = 0;
            SetEMPRadiusSphereScale(0.0f);
        }
    }

    void SetEMPRadiusSphereScale(float radius)
    {
        EMPRadius.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2); //Multiplication is temp.
    }

    void UpdateEMPDetection()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(EMPRadius.transform.position, currentRadius);

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            Electrical currentElectricalComponent = collidersInRange[i].gameObject.GetComponent<Electrical>();
            if (currentElectricalComponent == null)
            {
                continue;
            }

            if (currentElectricalComponent.IsPowered())
            {
                currentElectricalComponent.SetIsPowered(false);
            }
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
}