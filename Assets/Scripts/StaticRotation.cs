using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    private Quaternion startingRotation;

    private void Start()
    {
        // Store the initial rotation of the object and its parent transform.
        startingRotation = Quaternion.identity;

    }

    private void Update()
    {
        transform.rotation = Quaternion.identity; // startingRotation;
    }
}
