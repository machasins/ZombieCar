using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 20.0f;
    public float accelerationFactor = 30.0f;
    public float maxDrag = 3.0f;
    public float dragActivationSpeed = 3.0f;
    public float turnFactor = 3.5f;
    public float slipFactor = 0.95f;

    [Header("Drifting Settings")]
    public float driftActivationSpeed = 7.0f;
    public float driftTurnFactor = 5.0f;
    public float driftSlipFactor = 0.99f;

    private bool isDrifting;

    private float turn = 0.0f;
    private float targetTurn = 0.0f;

    private float drift = 0.0f;
    private float targetDrift = 0.0f;

    private float accelerationInput = 0;
    private float steeringInput = 0;

    private float rotationAngle = 0;

    private float forwardVelocity = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        turn = turnFactor;
        drift = slipFactor;
    }

    void FixedUpdate()
    {
        ApplyDrifting();

        ApplyEngineForce();

        //if (!isDrifting)
            KillOrthogonalVelocity();

        ApplySteering();
    }

    private void ApplyDrifting()
    {
        targetTurn = (isDrifting) ? driftTurnFactor : turnFactor;
        targetDrift = (isDrifting) ? driftSlipFactor : slipFactor;

        turn = Mathf.Lerp(turn, targetTurn, driftActivationSpeed * Time.fixedDeltaTime);
        drift = Mathf.Lerp(drift, targetDrift, driftActivationSpeed * Time.fixedDeltaTime);
    }

    private void ApplyEngineForce()
    {
        forwardVelocity = Vector2.Dot(transform.up, rb.velocity);

        if ((forwardVelocity > maxSpeed && accelerationInput > 0) ||
            (forwardVelocity < -maxSpeed * 0.5f && accelerationInput < 0) ||
            (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput != 0))
            return;

        if (accelerationInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, maxDrag, dragActivationSpeed * Time.fixedDeltaTime);
        else
            rb.drag = 0;

        Vector2 engineForceVector = accelerationFactor * accelerationInput * transform.up;

        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        float minSpeedAllowTurning = Mathf.Clamp01(rb.velocity.sqrMagnitude / 64);

        rotationAngle -= steeringInput * turn * minSpeedAllowTurning;

        rb.MoveRotation(rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * drift;
    }

    public void SetInputVector(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public void SetDrifting(InputAction.CallbackContext context)
    {
        isDrifting = context.ReadValue<float>() > 0;
    }
}
