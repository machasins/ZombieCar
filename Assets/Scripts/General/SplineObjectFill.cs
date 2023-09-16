using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class SplineObjectFill : MonoBehaviour
{
    public List<Transform> prefabs;
    public bool randomRotation = false;

    List<Transform> spawnedObjects = new();
    EdgeCollider2D spline;

    public void Awake()
    {
        spline = GetComponent<EdgeCollider2D>();

        spawnedObjects = new(GetComponentsInChildren<Transform>());
        spawnedObjects.Remove(transform);

        if (spawnedObjects.Count <= 0)
            SpawnObjects();
    }

    public List<Transform> SpawnObjects()
    {
        if (!spline)
            spline = GetComponent<EdgeCollider2D>();

        for (int i = 0; i < spline.pointCount - 1; ++i)
            spawnedObjects.Add(Instantiate(prefabs[Random.Range(0, prefabs.Count)], 
                (Vector3)spline.points[i] + spline.transform.position, 
                (randomRotation) ? Quaternion.Euler(0, 0, Random.Range(0, 360)) : Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, spline.points[i + 1] - spline.points[i])), 
                transform));

        AstarPath.active.Scan();

        return spawnedObjects;
    }

    public void DestroyObjects()
    {
        foreach (Transform t in spawnedObjects)
            if (t) DestroyImmediate(t.gameObject);

        AstarPath.active.Scan();
    }

    public Transform GetObject(int index)
    {
        return spawnedObjects[index % spawnedObjects.Count];
    }
}


[CustomEditor(typeof(SplineObjectFill))]
public class SplineObjectFillEditor : Editor
{
    List<Transform> spawnedObjects = new();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Spawn Objects"))
        {
            spawnedObjects = new((target as SplineObjectFill).GetComponentsInChildren<Transform>());
            spawnedObjects.Remove((target as SplineObjectFill).transform);
            foreach (Transform t in spawnedObjects)
                if (t) DestroyImmediate(t.gameObject);
            spawnedObjects = (target as SplineObjectFill).SpawnObjects();
        }

        if (GUILayout.Button("Remove Objects"))
        {
            spawnedObjects = new((target as SplineObjectFill).GetComponentsInChildren<Transform>());
            spawnedObjects.Remove((target as SplineObjectFill).transform);
            foreach (Transform t in spawnedObjects)
                if (t) DestroyImmediate(t.gameObject);
            spawnedObjects.Clear();
        }
    }
}