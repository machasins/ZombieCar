using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookModifier : MonoBehaviour
{
    [Tooltip("When at max speed, how far to move the camera toward the way the palyer is moving")]
    public float playerVelocityInfluenceScale;
    [Tooltip("How far the cursor can be before camera movement stops")]
    public float cursorPositionMaxDistance;
    [Tooltip("When at max distance, how far the camera moves towards the cursor")]
    public float cursorPositionInfluenceScale;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void Update()
    {
        Transform player = wcr.player.transform;
        Rigidbody2D player_rb = player.GetComponent<Rigidbody2D>();
        CarController player_car = player.GetComponent<CarController>();

        Transform cursor = wcr.cursor.transform;
        Vector3 cursorDir = cursor.position - player.position;

        transform.position = player.position
            + (Vector3)(Mathf.Lerp(0, playerVelocityInfluenceScale, player_rb.velocity.magnitude / player_car.maxSpeed) * player_rb.velocity.normalized)
            + Mathf.Lerp(0, cursorPositionInfluenceScale, cursorDir.magnitude / cursorPositionMaxDistance) * cursorDir.normalized;
    }
}
