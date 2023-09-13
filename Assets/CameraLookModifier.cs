using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookModifier : MonoBehaviour
{
    public float playerVelocityInfluenceScale;
    [Range(0.0f, 1.0f)] public float cursorPositionInfluence;

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

        transform.position = player.position 
            + (Vector3)(Mathf.Lerp(0, playerVelocityInfluenceScale, player_rb.velocity.magnitude / player_car.maxSpeed) * player_rb.velocity.normalized) 
            + cursorPositionInfluence * (cursor.position - player.position);
    }
}
