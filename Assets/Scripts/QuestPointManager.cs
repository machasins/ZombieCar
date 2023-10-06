using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPointManager : MonoBehaviour
{
    public QuestAsset quest;

    private ProgressionPointAsset[] progressionPoints;
    private QuestPoint[] questPoints;

    private Dictionary<ProgressionPointAsset, QuestPoint> points;

    private void Awake()
    {
        if (!quest)
        {
            Debug.LogError("Quest Object \"" + gameObject.name + "\"'s [QuestPointManager] does not have a quest assigned to it.");
            return;
        }

        points = new();
        questPoints = GetComponentsInChildren<QuestPoint>();
        progressionPoints = quest.GetProgressionPoints(); 
        
        for (int i = 0; i < progressionPoints.Length; i++)
        {
            QuestPoint target = null;
            foreach (QuestPoint q in questPoints)
                target = (q.progressionPoint == progressionPoints[i]) ? q : target;

            points[progressionPoints[i]] = target;
        }
    }

    public bool GetQuestObjective(out Vector3 position)
    {
        if (quest.NextProgressionPoint && points.TryGetValue(quest.NextProgressionPoint, out QuestPoint ret) && ret != null)
            return ret.GetQuestObjective(out position);

        position = Vector3.positiveInfinity;
        return false;
    }
}
