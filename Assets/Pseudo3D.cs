using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pseudo3D : MonoBehaviour
{
    public Transform topBone;
    public float maximumRadius;

    private Transform mainCamera;

    private void Awake()
    {
        if (topBone == null)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.Contains("TOP"))
                {
                    topBone = t;
                    // set values based on name;
                    break;
                }
            }

            if (topBone == null)
                Debug.LogError("Pseudo3D cannot find TOP bone on object " + name);
        }

        mainCamera = FindObjectOfType<WorldComponentReference>().cameraPosition.transform;
    }

    private void Update()
    {
        Vector2 awayAngle = (transform.position - mainCamera.position).normalized;
        float distance = Vector2.Distance(transform.position, mainCamera.position);

        topBone.position = (Vector2)transform.position + awayAngle * Mathf.Lerp(0.0f, maximumRadius * topBone.lossyScale.x, distance / 7.5f);
    }
}
