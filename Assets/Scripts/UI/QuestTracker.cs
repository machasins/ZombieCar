using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public Transform startingObjective;
    public QuestPointManager startingQuest;

    private QuestPointManager questCurrent;
    private Transform questObjective;
    private Vector3 questPosition;

    enum TrackingType { quest, transform, position, none }

    private TrackingType trackingType = TrackingType.none;

    private void Start()
    {
        questPosition = Vector3.zero;

        SetQuestObjective(startingObjective);
        SetQuestObjective(startingQuest);
    }

    public bool GetQuestPosition(out Vector3 position) 
    {
        switch (trackingType)
        {
            case TrackingType.quest:
                return questCurrent.GetQuestObjective(out position);
            case TrackingType.transform:
                bool ret = questObjective && questObjective.gameObject.activeInHierarchy;
                position = (ret) ? questObjective.position : Vector3.positiveInfinity;
                return ret;
            case TrackingType.position:
                position = questPosition;
                return true;
            case TrackingType.none:
                position = Vector3.positiveInfinity;
                return false;
        }

        position = Vector3.positiveInfinity;
        return false;
    }

    public void SetQuestObjective(QuestPointManager quest)
    {
        if (quest)
        {
            trackingType = TrackingType.quest;
            questCurrent = quest;
        }
    }

    public void SetQuestObjective(Transform target)
    {
        if (target)
        {
            trackingType = TrackingType.transform;
            questObjective = target;
        }
    }

    public void SetQuestObjective(Vector3 target)
    {
        trackingType = TrackingType.position;
        questPosition = target;
    }

    public void ClearQuestObjective()
    {
        trackingType = TrackingType.none;
    }
}
