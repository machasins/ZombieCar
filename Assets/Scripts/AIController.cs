using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public bool doPatrol = true;

    [Header("Follow Settings")]
    public bool doFollow = true;
    public float followRadius = 20.0f;
    public float followLostRadius = 40.0f;

    private bool isFollowing = false;

    private AIDestinationSetter follow;
    private Patrol patrol;

    private void Awake()
    {
        if (doPatrol)
        {
            patrol = GetComponent<Patrol>();
            patrol.enabled = true;
        }

        if (doFollow)
        {
            follow = GetComponent<AIDestinationSetter>();
            follow.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (doFollow)
        {
            if (!isFollowing && Vector3.Distance(transform.position, follow.target.position) <= followRadius)
            {
                isFollowing = true;
                follow.enabled = true;

                if (patrol)
                    patrol.enabled = false;
            }
            else if (isFollowing && Vector3.Distance(transform.position, follow.target.position) > followLostRadius)
            {
                isFollowing = false;
                follow.enabled = false;

                if (patrol)
                    patrol.enabled = true;
            }
        }
    }
}
