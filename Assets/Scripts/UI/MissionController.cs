using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    [HideInInspector] public List<Mission> missions;
    [HideInInspector] public int activeMission;

    private void Update()
    {
        float complete = missions[activeMission].percentComplete();
        if (complete >= 1.0f)
        {
            Mission mission = missions[activeMission];
            missions.RemoveAt(activeMission);
            if (mission.followUp != null)
                AddMission(mission.followUp);
        }
        else
        {

        }

        SetActiveMission();
    }

    public void AddMission(Mission newMission)
    {
        missions.Add(newMission);
        activeMission = missions.Count - 1;
    }

    private void SetActiveMission()
    {
        List<Mission> culled = new(missions.ToArray());
        culled.RemoveAll(c => !c.shouldActivate());
        if (culled.Count > 0)
        {
            culled.Sort((c, d) => c.priority.CompareTo(d.priority));
            activeMission = missions.FindIndex(c => c == culled[0]);
        }
        else
            activeMission = missions.Count - 1;
    }
}

public class Mission
{
    public delegate float Completion();
    public delegate bool Active();

    public string name;
    public string description;
    public string objective;

    public float priority;

    public Mission followUp;
    public Completion percentComplete;
    public Active shouldActivate;
}
