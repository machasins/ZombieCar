using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectiveMarker : MonoBehaviour
{
    public virtual Vector3 GetQuestObjective()
    {
        return Vector3.zero;
    }
}
