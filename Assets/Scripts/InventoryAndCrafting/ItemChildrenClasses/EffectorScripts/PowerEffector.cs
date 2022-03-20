using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PowerEffector : Item
{
    [Header("Power Specific Variables")]
    public float maxDistance = 5;
    private float currentDistance = 0;
    public float beamGrowTime = 0.5f;
    public Transform shotTransform;
    
    [Header("Juice Variables")]
    public LineRenderer lineRenderer;
    public GameObject baseBeam;
    public GameObject collisionBeam;
    public GameObject sparks;

    private float originalMaxDistance = 0;
    private float collidedDistance = 0;
    private bool hasCollided = false;
    private Ray currentRay;

    [Header("Modifier Variables")]
    public int amplifiedMaxDistance = 10;

    void Awake()
    {
        originalMaxDistance = maxDistance;
        lineRenderer.enabled = true;
    }

    public override void Activate()
    {
        if (currentDistance > 0)
        {
            UpdateBeamNotAlive();
        }

        Battery batteryCheck = gameObject.GetComponent<Battery>();

        if (batteryCheck != null)
        {
            if (!BatteryChargeUI.Instance.batteryUIObj.activeSelf)
            {
                BatteryChargeUI.Instance.ShowBatteryCharge(true);
            }

            if ((Input.GetMouseButtonDown(0) && Input.GetMouseButton(1)) || (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
            {
                batteryCheck.ShouldDrainBattery(true);
            }
            else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                batteryCheck.ShouldDrainBattery(false);
            }
        }

        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            if (batteryCheck != null)
            {
                if (batteryCheck.GetCurrentFill() == 0)
                {
                    if (collisionBeam.activeSelf)
                    {
                        collisionBeam.SetActive(false);
                    }

                    if (sparks.activeSelf)
                    {
                        sparks.SetActive(false);
                    }

                    hasCollided = false;

                    return;
                }
            }

            if (!baseBeam.activeSelf)
            {
                baseBeam.SetActive(true);
            }

            if (QuestManager.Instance.IsCurrentQuestActive())
            {
                Objective currentObjective = QuestManager.Instance.GetCurrentQuest().GetCurrentObjective();
                if (currentObjective != null)
                {
                    currentObjective.ActivateItem(itemName);
                }
            }

            if (!hasCollided)
            {
                if (collisionBeam.activeSelf)
                {
                    Debug.Log("Turned off");
                    collisionBeam.SetActive(false);
                }

                if (sparks.activeSelf)
                {
                    sparks.SetActive(false);
                }

                if (currentDistance < maxDistance - 0.1f)
                {
                    currentDistance = Mathf.Lerp(currentDistance, maxDistance, beamGrowTime * Time.deltaTime);
                    SetBeamDistance(currentDistance);
                    return;
                }
                currentDistance = maxDistance;
                SetBeamDistance(maxDistance);
            }
            else
            {
                currentDistance = collidedDistance;
                SetBeamDistance(currentDistance);
            }
        }
        else
        {
            if (collisionBeam.activeSelf)
            {
                collisionBeam.SetActive(false);
            }

            if (sparks.activeSelf)
            {
                sparks.SetActive(false);
            }

            hasCollided = false;

            if (currentDistance > 0.1f)
            {
                currentDistance = Mathf.Lerp(currentDistance, 0, beamGrowTime * Time.deltaTime);
                SetBeamDistance(currentDistance);
                return;
            }
            currentDistance = 0;
            SetBeamDistance(0.0f);

            if (baseBeam.activeSelf)
            {
                baseBeam.SetActive(false);
            }
        }
    }

    void SetBeamDistance(float dist)
    {
        UpdatePowerDetection(shotTransform.position, (shotTransform.position - transform.position).normalized);

        Vector3[] positions = { shotTransform.position, shotTransform.position + (shotTransform.position - transform.position).normalized * dist };
        lineRenderer.SetPositions(positions);
    }

    void UpdatePowerDetection(Vector3 origin, Vector3 direction)
    {
        Ray currentShot = new Ray(origin, direction);
        currentRay = currentShot;

        RaycastHit hitInfo;
        bool raycast;
        if (hasCollided)
        {
            raycast = Physics.Raycast(currentShot, out hitInfo, collidedDistance);
        }
        else
        {
            raycast = Physics.Raycast(currentShot, out hitInfo, currentDistance);
        }

        if (raycast) 
        {
            hasCollided = true;
            collidedDistance = hitInfo.distance + 0.1f;

            EffectorActions effectorActions = hitInfo.collider.gameObject.GetComponent<EffectorActions>();
            if (effectorActions != null)
            {
                effectorActions.PowerEffectorAction();
            }

            if (!collisionBeam.activeSelf)
            {
                collisionBeam.SetActive(true);
            }

            if (!sparks.activeSelf)
            {
                sparks.SetActive(true);
            }

            collisionBeam.transform.position = hitInfo.point;
            sparks.transform.position = hitInfo.point;
        }
        else
        {
            Collider[] collidersInRange = Physics.OverlapSphere(shotTransform.position, 0.1f);
            if (collidersInRange.Length > 0 && Input.GetMouseButton(0)
                && collidersInRange[0].gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {

                hasCollided = true;
                collidedDistance = currentDistance;

                EffectorActions effectorActions = collidersInRange[0].gameObject.GetComponent<EffectorActions>();
                if (effectorActions != null)
                {
                    effectorActions.PowerEffectorAction();
                }
            }
            else
            {
                hasCollided = false;
                collidedDistance = 0;
            }
        }
    }

    void UpdateBeamNotAlive()
    {
        if (currentDistance > 0.1f)
        {
            currentDistance = Mathf.Lerp(currentDistance, 0, beamGrowTime * Time.deltaTime);
            SetBeamDistance(currentDistance);
            return;
        }
        currentDistance = 0;
        SetBeamDistance(0.0f);

        if (baseBeam.activeSelf)
        {
            baseBeam.SetActive(false);
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
        currentDistance = 0;
    }

    public override void OnUnequip()
    {
        foreach (ModifierItem.ModifierType modifierType in (ModifierItem.ModifierType[])Enum.GetValues(typeof(ModifierItem.ModifierType)))
        {
            UnmodifyComponent(modifierType);
        }

        currentDistance = 0;

        Battery batteryCheck = gameObject.GetComponent<Battery>();

        if (batteryCheck != null)
        {
            if (BatteryChargeUI.Instance.batteryUIObj.activeSelf)
            {
                BatteryChargeUI.Instance.ShowBatteryCharge(false);
            }
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
            UpdateBeamNotAlive();
        }

        Battery batteryCheck = gameObject.GetComponent<Battery>();
        if (batteryCheck != null)
        {
            if (batteryCheck.GetCurrentFill() == 0 && batteryCheck.GetBatteryDrainStatus())
            {
                UpdateBeamNotAlive();
            }
        }

        Debug.Log(currentDistance);
    }
}
