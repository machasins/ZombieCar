using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.Util;
using System;

[RequireComponent(typeof(Seeker))]
public class AICarController : AIPath
{
    public float minDriftTime = 0.25f;

    private CarController car;
    private Rigidbody2D rb;

    private bool isDrifting = false;

    private Vector3 lastPosition;
    private bool isStuck = false;
    private float negate = 1.0f;

    protected override void Awake()
    {
        car = GetComponent<CarController>();
        rb = GetComponent<Rigidbody2D>();

        maxSpeed = car.maxSpeed;
        lastPosition = transform.position;
    }

    public override void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
    {
        // Steering direction
        float steeringInput = Mathf.Sign(-tr.up.x * velocity2D.y + tr.up.y * velocity2D.x);
        float desiredAngle = Vector2.Angle(tr.up, velocity2D);

        steeringInput *= Mathf.Lerp(0.0f, 1.0f, desiredAngle / 30.0f);

        // Acceleration Amount
        float accelerationInput = Mathf.Clamp((velocity2D.magnitude - rb.velocity.magnitude) / car.maxSpeed, -1.0f, 1.0f);

        // Stuck check
        if (Mathf.Abs(accelerationInput) <= 0.1f || Vector3.Distance(lastPosition, tr.position) >= 0.1f)
            isStuck = false;
        else if (!isStuck)
            General.StartConditionalCallbackAfterTime(this,
                () => isStuck = true,
                1.0f,
                () => isStuck,
                () => General.StartCallbackAfterTime(this, () => negate = -1, 0.5f, () => { negate = 1.0f; isStuck = false; })
            );
        lastPosition = tr.position;

        // Drifing check
        if (desiredAngle > 45.0f)
            General.StartCallbackAfterTime(this, () => isDrifting = true, minDriftTime, () => isDrifting = false);

        // Setting inputs
        car.SetDrifting(isDrifting);
        car.SetInputVector(new Vector2(steeringInput, negate * accelerationInput));
    }
}
