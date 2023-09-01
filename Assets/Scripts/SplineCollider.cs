using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class SplineCollider : MonoBehaviour
{
    public List<Vector2> points = new() { new(2.5f, 2.5f), new(-2.5f, -2.5f) };
    public List<Vector2> handles = new() { new(0.0f, -2.5f), new(-2.5f, 0.0f) };

    [Range(2, 64)]
    public int pointsQuantity = 2;
    [Range(1, 128)]
    public int lineQuality = 30;

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 handlerP0, Vector3 handlerP1, Vector3 p1)
    {
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; //first term
        p += 3f * uu * t * handlerP0; //second term
        p += 3f * u * tt * handlerP1; //third term
        p += ttt * p1; //fourth term

        return p;
    }

    Vector3 CalculateBezier(float t)
    {
        t = Mathf.Clamp(t, 0, pointsQuantity);
        int offset = (int)t;

        if (offset == pointsQuantity - 1)
            return points[^1];

        return CalculateBezierPoint(t - offset, points[offset], points[offset] + handles[offset], points[offset + 1] - handles[offset + 1], points[offset + 1]);
    }

    public Vector2[] Calculate2DPoints()
    {
        List<Vector2> splinePoints = new() {  };
        List<float> arcLengths = new() { 0 };
        List<float> tValues = new() { 0 };

        float totalLength = 0.0f;
        Vector2 previousPoint = points[0];
        Vector2 currentPoint;

        void AddPoint(float t)
        {
            //splinePoints.Add(p);
            tValues.Add(t);

            currentPoint = CalculateBezier(t);
            totalLength += Vector2.Distance(previousPoint, currentPoint);
            arcLengths.Add(totalLength);
            previousPoint = currentPoint;
        }

        int totalPoints = ((pointsQuantity - 1) * lineQuality);
        for (int i = 1; i <= totalPoints; ++i)
            AddPoint(1.0f / lineQuality * i);

        int arcIndex = 0;
        float avgArcLength = totalLength / (totalPoints - 1);
        float currentLength = 0;
        for (int i = 0; i < totalPoints; ++i)
        {
            float target = avgArcLength * i;
            if (target == currentLength)
                splinePoints.Add(CalculateBezier(tValues[arcIndex]));
            else
            {
                while (arcIndex + 1 < arcLengths.Count && currentLength < target)
                    currentLength = arcLengths[++arcIndex];

                float t = (target - arcLengths[arcIndex - 1]) / (arcLengths[arcIndex] - arcLengths[arcIndex - 1]);
                splinePoints.Add(CalculateBezier(Mathf.Lerp(tValues[arcIndex - 1], tValues[arcIndex], t)));
            }
        }


        return splinePoints.ToArray();
    }
}
