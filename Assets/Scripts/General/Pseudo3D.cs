using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pseudo3D : MonoBehaviour
{
    public float maximumRadius;

    private Transform mainCamera;
    private List<Transform> topBone = new();
    private List<Transform> topParent = new();
    private List<Vector3> originalOffset = new();

    private void Awake()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name.Contains("TOP"))
            {
                topBone.Add(t);
                topParent.Add(t.parent);
                originalOffset.Add(t.position - t.parent.position);
                // set values based on name;
            }
        }

        if (topBone.Count <= 0)
            Debug.LogError("Pseudo3D cannot find TOP bone on object " + name);

    mainCamera = FindObjectOfType<WorldComponentReference>().cameraPosition.transform;
    }

    private void Update()
    {
        for (int i = 0; i < topBone.Count; ++i)
        {
            Vector2 bonePosition = (Vector2)(topParent[i].position + originalOffset[i]);
            Vector2 awayAngle = (bonePosition - (Vector2)mainCamera.position).normalized;
            float distance = Vector2.Distance(bonePosition, mainCamera.position);

            topBone[i].position = bonePosition + awayAngle * maximumRadius * topBone[i].lossyScale.x * distance / 7.5f;
        }
    }
}
