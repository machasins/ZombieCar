using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QO_Object : QuestObjectiveMarker
{
    public Transform obj;

    public override Vector3 GetQuestObjective()
    {
        return obj.position;
    }
}
