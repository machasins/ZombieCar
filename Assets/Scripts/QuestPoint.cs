using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoint : ProgressionPointComponent
{
    public QuestObjectiveMarker objective;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        QuestTracker t = wcr.GetComponent<QuestTracker>();
        QuestPointManager m = GetComponent<QuestPointManager>();
        OnComplete.AddListener(() => t.SetQuestObjective(m));
    }

    public bool GetQuestObjective(out Vector3 position)
    {
        position = (objective) ? objective.GetQuestObjective() : Vector3.positiveInfinity;
        return objective != null;
    }
}
