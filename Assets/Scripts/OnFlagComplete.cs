using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnFlagComplete : MonoBehaviour
{
    public FlagDependency[] flags;
    public UnityEvent onComplete;

    private bool hasBeenCompleted = false;

    void Update()
    {
        if (!hasBeenCompleted)
        {
            bool satisfied = true;
            foreach (FlagDependency dependency in flags)
                satisfied = satisfied && dependency.Satisfied();

            if (satisfied)
            {
                onComplete.Invoke();
                hasBeenCompleted = true;
            }
        }
    }
}
