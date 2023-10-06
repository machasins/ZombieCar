using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QO_Point : QuestObjectiveMarker
{
    public Vector3 point;

    public override Vector3 GetQuestObjective()
    {
        return point;
    }
}
