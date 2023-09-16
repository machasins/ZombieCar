using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Transform target;
    public float followTime = 0.5f;

    private Quaternion originalRotation;

    private bool usePosition = false;
    private Vector3 aimPosition;

    private Tweener worldRotate;
    private Tweener localRotate;
    private Vector3 lastPosition;

    private void Awake()
    {
        originalRotation = transform.localRotation;

        worldRotate = transform.DORotate(new Vector3(0, 0, Mathf.Repeat(Vector2.SignedAngle(Vector2.up, Vector2.up), 360)), followTime).SetAutoKill(false);
        localRotate = transform.DOLocalRotate(originalRotation.eulerAngles, followTime).SetAutoKill(false);
    }

    private void Update()
    {
        if (target != null || usePosition)
        {
            Vector3 euler = new(0, 0, Mathf.Repeat(Vector2.SignedAngle(Vector2.up, ((usePosition) ? aimPosition : target.position) - transform.position), 360));

            if (euler == lastPosition) return;

            worldRotate.ChangeEndValue(euler, true).Restart();
            lastPosition = euler;
        }
        else
        {
            if (originalRotation.eulerAngles == lastPosition) return;

            localRotate.ChangeEndValue(originalRotation.eulerAngles, true).Restart();
            lastPosition = originalRotation.eulerAngles;
        }
    }

    public void SetPosition(Vector3 position)
    {
        usePosition = true;
        aimPosition = position;
    }
}
