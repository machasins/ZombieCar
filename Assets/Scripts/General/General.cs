using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General
{
    public delegate void Callback();
    public delegate void Tracker(float time);
    public delegate bool Condition();
    public static IEnumerator CallbackAfterTime(float duration, Callback callback)
    {
        yield return new WaitForSeconds(duration);
        callback();
    }

    public static void StartCallbackAfterTime(MonoBehaviour obj, Callback precall, float duration, Callback callback)
    {
        precall();
        obj.StartCoroutine(CallbackAfterTime(duration, callback));
    }

    public static IEnumerator CallbackAfterTimeTracker(float duration, Tracker tracker, Callback callback)
    {
        float waitTime = Time.time + duration;
        while (Time.time < waitTime)
        {
            tracker(duration - (waitTime - Time.time));
            yield return new WaitForEndOfFrame();
        }
        callback();
    }

    public static void StartConditionalCallbackAfterTime(MonoBehaviour obj, Callback precall, float duration, Condition condition, Callback callback)
    {
        precall();
        obj.StartCoroutine(ConditionalCallbackAfterTime(duration, condition, callback));
    }

    public static IEnumerator ConditionalCallbackAfterTime(float duration, Condition condition, Callback callback)
    {
        float waitTime = Time.time + duration;
        while (Time.time < waitTime)
        {
            if (!condition()) yield break;
            yield return new WaitForEndOfFrame();
        }
        callback();
    }
}
