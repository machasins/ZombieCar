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

    private bool isDrifting = false;

    protected override void Awake()
    {
        car = GetComponent<CarController>();
        maxSpeed = car.maxSpeed;
    }

    public override void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
    {
        float steeringInput = Mathf.Sign(-tr.up.x * velocity2D.y + tr.up.y * velocity2D.x);
        float desiredAngle = Vector2.Angle(tr.up, velocity2D);

        steeringInput *= Mathf.Lerp(0.0f, 1.0f, desiredAngle / 45.0f);

        if (desiredAngle > 45.0f)
            General.StartCallbackAfterTime(this, () => isDrifting = true, minDriftTime, () => isDrifting = false);

        car.SetDrifting(isDrifting);
        car.SetInputVector(new Vector2(steeringInput, velocity2D.magnitude / maxSpeed));
    }
}
