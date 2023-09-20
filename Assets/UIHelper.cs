using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{
    public void SetActiveChildren(bool active)
    {
        foreach (Transform t in transform)
            t.gameObject.SetActive(active);
    }

    public void ClickButton()
    {
        GetComponent<Button>().onClick.Invoke();
    }

    public void FadeOut(float time)
    {
        StartCoroutine(FadeCoroutine(GetComponent<CanvasGroup>(), time, 1.0f, 0.0f));
    }

    public void FadeIn(float time)
    {
        StartCoroutine(FadeCoroutine(GetComponent<CanvasGroup>(), time, 0.0f, 1.0f));
    }

    private IEnumerator FadeCoroutine(CanvasGroup c, float time, float from, float to)
    {
        float totalTime = Time.time + time;

        c.blocksRaycasts = false;
        c.interactable = false;

        while (Time.time < totalTime)
        {
            c.alpha = Mathf.Lerp(from, to, 1.0f - (totalTime - Time.time) / time);
            yield return null;
        }

        c.alpha = to;

        c.blocksRaycasts = to > 0.0f;
        c.interactable = to > 0.0f;
    }
}
