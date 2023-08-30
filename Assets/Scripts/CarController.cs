using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 20.0f;
    public float reverseFactor = 0.75f;
    public float accelerationFactor = 30.0f;
    public float minDrag = 1.0f;
    public float maxDrag = 3.0f;
    public float dragActivationSpeed = 3.0f;
    public float turnFactor = 3.5f;
    public float slipFactor = 0.95f;

    [Header("Drifting Settings")]
    public float driftActivationSpeed = 7.0f;
    public float driftTurnFactor = 5.0f;
    public float driftSlipFactor = 0.99f;

    [Header("Boost Settings")]
    public float boostChargeTime = 2.0f;
    public float boostStrength = 15.0f;



    private float speed;
    private float turn = 0.0f;
    private float targetTurn = 0.0f;

    private float drift = 0.0f;
    private float targetDrift = 0.0f;

    private float boostTime = 0.0f;

    private bool isDrifting;
    private float accelerationInput = 0;
    private float steeringInput = 0;

    private float rotationAngle = 0;

    private float forwardVelocity = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        speed = maxSpeed;
        turn = turnFactor;
        drift = slipFactor;
    }

    void FixedUpdate()
    {
        ApplyDrifting();

        ApplyBoosting();

        ApplyEngineForce();

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

    private void ApplyBoosting()
    {
        if (boostTime >= boostChargeTime && !isDrifting)
        {
            boostTime = 0;
            rb.AddForce(transform.up * boostStrength, ForceMode2D.Impulse);
        }

        if (isDrifting && steeringInput != 0 && accelerationInput > 0)
            boostTime += Time.fixedDeltaTime;
        else
            boostTime = 0;
    }

    private void ApplyEngineForce()
    {
        forwardVelocity = Vector2.Dot(transform.up, rb.velocity);

        if ((forwardVelocity > speed && accelerationInput > 0) ||
            (forwardVelocity < -speed * reverseFactor && accelerationInput < 0) ||
            (rb.velocity.sqrMagnitude > speed * speed && accelerationInput != 0))
            return;

        if (accelerationInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, maxDrag, dragActivationSpeed * Time.fixedDeltaTime);
        else
            rb.drag = minDrag;

        Vector2 engineForceVector = accelerationFactor * accelerationInput * transform.up;

        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        float minSpeedAllowTurning = Mathf.Clamp01(rb.velocity.magnitude / 8);

        rotationAngle -= steeringInput * turn * minSpeedAllowTurning;

        rb.MoveRotation(rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * drift;
    }
    
    public bool IsTireSkid(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = Vector3.Dot(transform.right, rb.velocity);
        isBraking = false;

        if (accelerationInput < 0 && forwardVelocity > 0)
        {
            isBraking = true;
            return true;
        }

        if (Mathf.Abs(lateralVelocity) > 4.0f)
            return true;

        return false;
    }

    public void SetInputVector(Vector2 input)
    {
        steeringInput = input.x;
        accelerationInput = input.y;
    }

    public void SetDrifting(float input)
    {
        isDrifting = input > 0;
    }

    public void SetDrifting(bool input)
    {
        isDrifting = input;
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
