using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QO_ClosestObject : QuestObjectiveMarker
{
    public Transform[] objs;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    public override Vector3 GetQuestObjective()
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;
        foreach (Transform t in objs)
        {
            if (!t.gameObject.activeInHierarchy)
                continue;

            float currDist = Vector2.Distance(wcr.player.transform.position, t.position);
            if (currDist < minDist)
            {
                minDist = currDist;
                nearest = t;
            }
        }

        return (nearest) ? nearest.position : Vector3.zero;
    }
}
