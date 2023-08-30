using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeFollow : MonoBehaviour
{
    public class Point
    {
        public Vector3 position;
        public Quaternion rotation;
        public float distance;
    }

    public SnakeFollow head;

    [Header("Head Settings")]
    public int memoryLength;
    public float minimumDistance;

    [Header("Body Settings")]
    public float followerDistance;

    [HideInInspector] public List<Point> memory;

    public delegate void UpdateDelegate();
    public UpdateDelegate updater;

    private Vector3 prevPosition;

    private void Awake()
    {
        if (!head)
        {
            memory = new List<Point>
            {
                new Point
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    distance = 0
                },
                new Point
                {
                    position = transform.position + transform.up * -1000,
                    rotation = transform.rotation,
                    distance = 1000
                }
            };
        }
    }

    private void Start()
    {
        if (head)
        {
            head.updater += SetTarget;
            SetTarget();
        }
    }

    private void FixedUpdate()
    {
        if (!head)
        {
            if (Vector3.Distance(prevPosition, transform.position) >= minimumDistance)
            {
                prevPosition = transform.position;
                AddPoint();
            }

            updater();
        }
    }

    private void AddPoint()
    {
        Point point = new()
        {
            position = transform.position,
            rotation = transform.rotation,
            distance = 0
        };

        memory[0].distance = Vector3.Distance(transform.position, memory[0].position);

        memory.Insert(0, point);

        if (memory.Count > memoryLength)
            memory.RemoveAt(memory.Count - 1);
    }

    private void SetTarget()
    {
        Point prev = head.memory[0], next = head.memory[0];
        float distance = head.memory[0].distance;

        for (int i = 1; i < head.memory.Count; i++)
        {
            prev = head.memory[i - 1];
            next = head.memory[i];

            distance += next.distance;
            if (distance >= followerDistance)
                break;
        }

        float t = (next.distance - followerDistance) / (next.distance - prev.distance);
        transform.DOMove(Vector3.Lerp(next.position, prev.position, t), 0.1f);
        transform.DORotate(Quaternion.Lerp(next.rotation, prev.rotation, t).eulerAngles, 0.1f);
    }
}
