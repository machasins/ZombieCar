using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObjectiveDisplay : MonoBehaviour
{
    public float verticalOffset;
    public Vector2 outOfViewBounds;
    public float movementTrackTime;

    private static WorldComponentReference wcr;
    private QuestTracker questTracker;
    private Image marker;
    private RectTransform t;

    private Tweener movement;

    private Vector2 lastPosition;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        questTracker = wcr.GetComponent<QuestTracker>();

        marker = GetComponent<Image>();
        t = GetComponent<RectTransform>();

        //movement = t.DOMove(t.position, movementTrackTime).SetAutoKill(false);
        lastPosition = t.position;
    }

    private void Update()
    {
        bool questActive = questTracker.GetQuestPosition(out Vector3 questPosition);

        if (questActive)
        {
            Vector3 missionPosition = questPosition + Vector3.up * verticalOffset;
            Vector2 camPos = wcr.cameraPosition.transform.position;

            Vector2 pos = new(missionPosition.x, missionPosition.y);

            //if (screenPos.x > screenBounds.x || screenPos.y > screenBounds.y || screenPos.x < screenBoundsOffset.x || screenPos.y < screenBoundsOffset.y)
            pos = new Vector2(Mathf.Clamp(pos.x, camPos.x - outOfViewBounds.x, camPos.x + outOfViewBounds.x), Mathf.Clamp(pos.y, camPos.y - outOfViewBounds.y, camPos.y + outOfViewBounds.y));

            t.position = pos;

            //if (pos != lastPosition)
            //{
            //movement.ChangeEndValue((Vector3)pos, true).Restart();
            //lastPosition = pos;
            //}
        }

        marker.color = (questActive) ? Color.white : Color.clear;
    }
}
