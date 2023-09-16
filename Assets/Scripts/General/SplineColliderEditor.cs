using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(SplineCollider))]
public class SplineColliderEditor : Editor
{
    SplineCollider spline;
    EdgeCollider2D edgeCollider;

    bool showPoints = false;

    int lastPointsQuantity = 0;
    int lastLineQuality = 0;
    List<Vector2> lastPoints = new();
    List<Vector2> lastHandles = new();

    public override void OnInspectorGUI()
    {
        spline = (SplineCollider)target;

        if (spline.TryGetComponent(out edgeCollider))
        {
            serializedObject.Update();
            Show();
            EditorGUILayout.IntSlider(serializedObject.FindProperty("lineQuality"), 1, 64, "Curve Quality", GUILayout.MinWidth(100));
            serializedObject.ApplyModifiedProperties();

            UpdateSpline();

            EditorUtility.SetDirty(spline);
        }
    }

    void Show()
    {
        if (showPoints = EditorGUILayout.Foldout(showPoints, "Points"))
        {
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < spline.points.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Vector4 point = new(spline.points[i].x, spline.points[i].y, spline.handles[i].x, spline.handles[i].y);
                point = EditorGUILayout.Vector4Field("", point);
                spline.points[i] = new(point.x, point.y);
                spline.handles[i] = new(point.z, point.w);

                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20.0f)))
                {
                    spline.points.RemoveAt(i);
                    spline.handles.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Point"))
            {
                spline.points.Add(spline.points[^1] + spline.handles[^1] * 2);
                spline.handles.Add(spline.handles[^1]);
            }
            EditorGUI.indentLevel -= 1;
        }
    }

    void OnSceneGUI()
    {
        if (spline != null)
        {
            Vector3 handle(Vector3 p, Vector3 offset, Color c)
            {
                Handles.color = c;

                Vector3 position = spline.transform.position + p + offset;
                return Handles.FreeMoveHandle(position, Quaternion.identity, 0.04f * HandleUtility.GetHandleSize(position), Vector3.zero, Handles.DotHandleCap) - (position - p);
            }

            void line(Vector3 p, Vector3 h, Color c)
            {
                Handles.color = c;
                Handles.DrawLine(spline.transform.position + p + h, spline.transform.position + p);
            }

            void bezier(Vector3 p1, Vector3 p2, Vector3 h1, Vector3 h2, Color c)
            {
                Handles.DrawBezier(spline.transform.position + p1, spline.transform.position + p2, spline.transform.position + p1 + h1, spline.transform.position + p2 - h2, c, null, 1);
            }

            for (int i = 0; i < spline.pointsQuantity; i++)
            {
                if (i > 0 && !edgeCollider.enabled)
                    bezier(spline.points[i - 1], spline.points[i], spline.handles[i - 1], spline.handles[i], Color.cyan);

                spline.handles[i] = handle(spline.handles[i], spline.points[i], Color.red);
                line(spline.points[i], spline.handles[i], Color.red);
                spline.handles[i] = -handle(-spline.handles[i], spline.points[i], Color.blue);
                line(spline.points[i], -spline.handles[i], Color.blue);

                spline.points[i] = handle(spline.points[i], Vector3.zero, Color.white);
            }

            UpdateSpline();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }

    void UpdateSpline()
    {
        bool condition = spline != null && (spline.pointsQuantity != lastPointsQuantity 
            || spline.lineQuality != lastLineQuality 
            || spline.points.Count != spline.pointsQuantity
            || spline.points.Count != lastPoints.Count
            || spline.handles.Count != lastHandles.Count
            || spline.handles.Count != spline.pointsQuantity);

        for (int i = 0; i < lastPoints.Count; i++)
        {
            if (condition)
                break;
            condition = condition || lastPoints[i] != spline.points[i] || lastHandles[i] != spline.handles[i];
        }

        if (condition)
        {
            spline.pointsQuantity = spline.points.Count;
            
            edgeCollider.points = spline.Calculate2DPoints();

            lastPointsQuantity = spline.pointsQuantity;
            lastLineQuality = spline.lineQuality;
            lastPoints = new(spline.points.ToArray());
            lastHandles = new(spline.handles.ToArray());
        }
    }
}
