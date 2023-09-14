using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General
{
    public delegate void Callback();
    public delegate void Tracker(float time);
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
        float time = 0.0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            tracker(time);
            yield return new WaitForEndOfFrame();
        }
        callback();
    }
}
