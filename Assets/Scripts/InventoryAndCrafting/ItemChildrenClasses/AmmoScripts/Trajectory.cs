using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    public enum TrajectoryType
    {
        straight,
        arc,
        none
    };

    public TrajectoryType trajectoryType = TrajectoryType.none;
}
