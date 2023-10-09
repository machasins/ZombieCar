using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QO_ClosestObjective : QuestObjectiveMarker
{
    public QuestObjectiveMarker[] objs;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    public override Vector3 GetQuestObjective()
    {
        QuestObjectiveMarker nearest = null;
        float minDist = Mathf.Infinity;
        foreach (QuestObjectiveMarker t in objs)
        {
            if (!t.gameObject.activeInHierarchy)
                continue;

            float currDist = Vector2.Distance(wcr.player.transform.position, t.transform.position);
            if (currDist < minDist)
            {
                minDist = currDist;
                nearest = t;
            }
        }

        return (nearest) ? nearest.GetQuestObjective() : Vector3.positiveInfinity;
    }
}

