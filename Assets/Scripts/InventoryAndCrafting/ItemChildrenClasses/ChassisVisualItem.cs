using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChassisVisualItem : VisualItem
{
    private List<ChassisComponentTransform> visualComponentTransforms = new List<ChassisComponentTransform>();
    private ChassisGripTransform visualGripTransform;

    public void AddVisualTransforms(List<ChassisComponentTransform> componentTransforms, ChassisGripTransform gripTransform)
    {
        for (int i = 0; i < componentTransforms.Count; i++)
        {
            visualComponentTransforms.Add(componentTransforms[i]);
        }

        visualGripTransform = gripTransform;
    }

    public List<ChassisComponentTransform> GetVisualComponentTransforms()
    {
        return visualComponentTransforms;
    }

    public ChassisGripTransform GetVisualGripTransform()
    {
        return visualGripTransform;
    }
}
