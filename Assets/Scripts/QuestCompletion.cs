using Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestCompletion : MonoBehaviour
{
    public UnityEvent onComplete;

    public void Complete()
    {
        onComplete.Invoke();
    }
}
