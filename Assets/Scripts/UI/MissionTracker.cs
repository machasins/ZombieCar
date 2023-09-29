using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTracker : MonoBehaviour
{
    public Transform startingObjective;

    private Transform missionObjective;
    private Vector3 missionPosition;

    private bool useTransform = false;
    private bool isActiveMission = false;

    private void Start()
    {
        missionPosition = Vector3.zero;

        SetMissionObjective(startingObjective);
    }

    public bool IsMissionActive()
    {
        return isActiveMission;
    }

    public Vector3 GetMissionPosition() 
    {
        return (useTransform && missionObjective) ? missionObjective.position : missionPosition; 
    }

    public void SetMissionObjective(Transform target)
    {
        missionObjective = target;
        useTransform = true;
        isActiveMission = target != null;
    }

    public void SetMissionObjective(Vector3 target)
    {
        missionPosition = target;
        useTransform = false;
        isActiveMission = true;
    }

    public void ClearMissionObjective()
    {
        isActiveMission = false;
    }
}
