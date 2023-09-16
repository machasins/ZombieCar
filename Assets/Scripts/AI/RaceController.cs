using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIDestinationSetter))]
public class RaceController : MonoBehaviour
{
    public SplineObjectFill raceTrack;
    public float setNextWaypointDistance = 10.0f;
    public int raceStartingOffset;

    AIDestinationSetter destinationSetter;
    Transform currentlyTracking;
    int currentOffset = 0;

    private void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.enabled = true;

        currentlyTracking = raceTrack.GetObject(raceStartingOffset);

        destinationSetter.target = currentlyTracking;
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(currentlyTracking.position, transform.position);
        if (distanceToTarget <= setNextWaypointDistance)
        {
            currentlyTracking = raceTrack.GetObject(raceStartingOffset + ++currentOffset);
            destinationSetter.target = currentlyTracking;
        }
    }
}
