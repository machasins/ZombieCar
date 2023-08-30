using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Transform target;
    public float followTime = 0.5f;
    
    private Quaternion originalRotation;

    private void Awake()
    {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (target != null)
            transform.DORotate(new Vector3(0, 0, Mathf.Repeat(Vector2.SignedAngle(Vector2.up, target.position - transform.position), 360)), followTime);
        else
            transform.DOLocalRotate(originalRotation.eulerAngles, followTime);
    }
}
