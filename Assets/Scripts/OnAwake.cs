using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAwake : MonoBehaviour
{
    public UnityEvent awake;

    private void Awake()
    {
        awake.Invoke();
        this.enabled = false;
    }
}
