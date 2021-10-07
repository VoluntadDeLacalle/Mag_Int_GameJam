using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PowerEffector : Item
{
    public int maxDistance = 5;
    public Transform shotTransform;
    public LineRenderer lineRenderer;

    public float fireTimer = 5;
    private float maxFireTimer = 0;

    public float rayDrawTimer = 2;
    private float maxRayDrawTimer = 0;

    private int originalMaxDistance = 0;
    private bool hasFired = false;
    private bool hasDrawnLine = false;

    private Ray currentRay;

    [Header("Modifier Variables")]
    public int amplifiedMaxDistance = 10;

    private void Awake()
    {
        originalMaxDistance = maxDistance;
        maxFireTimer = fireTimer;
        maxRayDrawTimer = rayDrawTimer;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(currentRay);
    }

    public override void Activate()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0 && Input.GetMouseButtonDown(0))
        {
            UpdatePowerDetection();
            fireTimer = maxFireTimer;
            hasFired = true;
            hasDrawnLine = false;
        }

        if (hasFired)
        {
            rayDrawTimer -= Time.deltaTime;
            if (rayDrawTimer <= 0)
            {
                hasFired = false;
                rayDrawTimer = maxRayDrawTimer;
                lineRenderer.enabled = false;
            }
        }
    }

    void UpdatePowerDetection()
    {
        Ray currentShot = new Ray(shotTransform.position, (shotTransform.position - transform.position).normalized * maxDistance);
        RaycastHit hitInfo;
        if (Physics.Raycast(currentShot, out hitInfo, maxDistance)) 
        {
            Electrical currentElectricalComponent = hitInfo.collider.gameObject.GetComponent<Electrical>();
            if (currentElectricalComponent != null)
            {
                if (!currentElectricalComponent.IsPowered())
                {
                    currentElectricalComponent.SetIsPowered(true);
                    return;
                }

                return;
            }

            Enemy currentEnemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
            if (currentEnemy != null)
            {
                currentEnemy.health.TakeDamage(100);
            }
        }
    }

    public override void ModifyComponent(ModifierItem.ModifierType modifierType)
    {
        switch (modifierType)
        {
            case ModifierItem.ModifierType.Amplifier:
                maxDistance = amplifiedMaxDistance;
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
                maxDistance = originalMaxDistance;
                break;
            case ModifierItem.ModifierType.Exploding:
                break;
            case ModifierItem.ModifierType.Reflector:
                break;
        }
    }

    public override void OnEquip()
    {
        fireTimer = 0;
    }

    public override void OnUnequip()
    {
        foreach (ModifierItem.ModifierType modifierType in (ModifierItem.ModifierType[])Enum.GetValues(typeof(ModifierItem.ModifierType)))
        {
            UnmodifyComponent(modifierType);
        }

        fireTimer = maxFireTimer;
        rayDrawTimer = maxRayDrawTimer;
        hasFired = false;
    }

    void Update()
    {
        if (itemType != TypeTag.effector)
        {
            Debug.LogError($"{itemName} is currently of {itemType} type and not effector!");
        }
    }

    new void LateUpdate()
    {
        base.LateUpdate();

        if (hasFired && !hasDrawnLine)
        {
            Vector3[] positions = { shotTransform.position, shotTransform.position + (shotTransform.position - transform.position).normalized * maxDistance };
            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;
            hasDrawnLine = true;
        }
    }
}
