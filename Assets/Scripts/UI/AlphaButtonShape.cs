using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaButtonShape : MonoBehaviour
{
    public float alphaThreshold = 0.5f;

    private Image button;

    private void Awake()
    {
        button = GetComponent<Image>();
        button.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}
