using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    public float minScale = 0.9f;       // Minimum scale value
    public float maxScale = 1.1f;       // Maximum scale value
    public float breathSpeed = 1.0f;    // Breathing speed
    private Vector3 originalScale;      // The original scale of the object

    private float randomOffset;

    private void Start()
    {
        // Store the original scale of the object
        originalScale = transform.localScale;

        randomOffset = Random.value;
    }

    private void Update()
    {
        // Calculate the new scale using a sine wave to create a breathing effect
        float scaleFactor = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * breathSpeed + randomOffset, 1.0f));

        // Apply the new scale to the object
        transform.localScale = originalScale * scaleFactor;
    }
}
