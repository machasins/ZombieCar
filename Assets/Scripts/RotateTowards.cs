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
        {
            Quaternion look = Quaternion.LookRotation(target.position - transform.position, Vector3.forward);
            transform.DORotate(new Vector3(0, 0, Mathf.Repeat(-look.eulerAngles.z, 360)), followTime);
        }
        else
            transform.DOLocalRotate(originalRotation.eulerAngles, followTime);
    }
}
