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

    void Awake()
    {
        originalMaxDistance = maxDistance;
        maxFireTimer = fireTimer;
        maxRayDrawTimer = rayDrawTimer;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //Gizmos.DrawRay(currentRay);
    }

    public override void Activate()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0 && Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButton(1))
            {
                Ray tempRay = Crosshair.Instance.GetCrosshairPosition();
                Vector3 endPoint = tempRay.origin + tempRay.direction;

                UpdatePowerDetection(new Ray(shotTransform.position, (shotTransform.position - endPoint)));
            }
            else
            {
                UpdatePowerDetection(new Ray(shotTransform.position, (shotTransform.position - transform.position).normalized * maxDistance));
            }


            fireTimer = maxFireTimer;
            hasFired = true;
            hasDrawnLine = false;

            if (QuestManager.Instance.IsCurrentQuestActive())
            {
                Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                if (currentObjective != null)
                {
                    currentObjective.ActivateItem(itemName);
                }
            }
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

    void UpdatePowerDetection(Ray firedRay)
    {
        Ray currentShot = firedRay;
        currentRay = currentShot;

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
                currentEnemy.SetStun();
            }

            JunkerBot currentJunker = hitInfo.collider.gameObject.GetComponent<JunkerBot>();
            if (currentJunker != null)
            {
                currentJunker.stateMachine.switchState(JunkerStateMachine.StateType.Disabled);
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

    void LateUpdate()
    {
        if (hasFired && !hasDrawnLine)
        {
            Vector3 endPoint = currentRay.origin + currentRay.direction;

            Vector3[] positions = { shotTransform.position, shotTransform.position - (shotTransform.position - endPoint) * maxDistance};
            lineRenderer.SetPositions(positions);
            lineRenderer.enabled = true;
            hasDrawnLine = true;
        }
    }
}
