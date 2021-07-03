using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorEffector : Item
{
    public int maxRadius = 5;
    public float radiusGrowthTime = 1;
    public GameObject detectorRadius;
    
    private float currentRadius = 0;
    private List<DetectableObject> previousDetectableObjectsInRange = new List<DetectableObject>();

    public override void Activate()
    {
        if (currentRadius > 0)
        {
            CheckDetectableObjects();
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
        detectorRadius.transform.parent = null;
        detectorRadius.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2); //Multiplication is temp.
        detectorRadius.transform.parent = this.gameObject.transform;
    }

    void CheckDetectableObjects()
    {
        Collider[] currentColliders = Physics.OverlapSphere(transform.position, currentRadius);
        List<DetectableObject> currentDetectableObjectsInRange = new List<DetectableObject>();

        for(int i = 0; i < currentColliders.Length; i++)
        {
            DetectableObject tempObj = currentColliders[i].gameObject.GetComponent<DetectableObject>();
            if (tempObj != null)
            {
                currentDetectableObjectsInRange.Add(tempObj);
                tempObj.ActivateMeshRenderer(true);
            }
        }

        for (int j = 0; j < previousDetectableObjectsInRange.Count; j++)
        {
            if (!currentDetectableObjectsInRange.Contains(previousDetectableObjectsInRange[j]))
            {
                if (previousDetectableObjectsInRange[j] != null)
                {
                    previousDetectableObjectsInRange[j].ActivateMeshRenderer(false);
                }
            }
        }

        previousDetectableObjectsInRange = currentDetectableObjectsInRange;
    }
}