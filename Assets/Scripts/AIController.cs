using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
public class AIController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public bool doPatrol = true;
    public float patrolSpeed = 0.0f;

    [Header("Follow Settings")]
    public bool doFollow = true;
    public float followSpeed = 0.0f;
    public float followRadius = 20.0f;
    public float followLostRadius = 40.0f;

    private bool isFollowing = false;

    private AIPath movement;
    private AIDestinationSetter follow;
    private Patrol patrol;

    private void Start()
    {
        movement = GetComponent<AIPath>();

        if (!doPatrol || patrolSpeed <= 0.0f)
            patrolSpeed = movement.maxSpeed;

        if (!doFollow || followSpeed <= 0.0f)
            followSpeed = movement.maxSpeed;

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
        movement.maxSpeed = (isFollowing) ? followSpeed : patrolSpeed;

        if (doFollow)
        {
            float distance = Vector3.Distance(transform.position, follow.target.position);
            if (!isFollowing && distance <= followRadius)
            {
                isFollowing = true;
                follow.enabled = true;

                if (patrol)
                    patrol.enabled = false;
            }
            else if (isFollowing && distance > followLostRadius)
            {
                isFollowing = false;
                follow.enabled = false;

                if (patrol)
                    patrol.enabled = true;
            }
        }
    }
}
