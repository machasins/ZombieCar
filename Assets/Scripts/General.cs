using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General
{
    public delegate void Callback();
    public static IEnumerator CallbackAfterTime(float duration, Callback callback)
    {
        yield return new WaitForSeconds(duration);
        callback();
    }
}
