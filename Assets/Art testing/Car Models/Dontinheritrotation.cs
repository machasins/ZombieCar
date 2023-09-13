using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dontinheritrotation : MonoBehaviour
{
    private Quaternion lastparentrotation;

    private void Start()
    {
        // Store the initial rotation of the object and its parent transform.
        lastparentrotation = transform.parent.localRotation;

    }

    private void Update()
    {
        transform.localRotation = Quaternion.Inverse(transform.parent.localRotation) * lastparentrotation * transform.localRotation;

        lastparentrotation = transform.parent.localRotation;

    }
}
