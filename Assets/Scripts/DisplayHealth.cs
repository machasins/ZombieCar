using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealth : MonoBehaviour
{
    public Image healthBar;
    public Image healthBarRecentDamage;
    public CanvasGroup canvas;

    public float timeToFade = 2.0f;
    public float fadeTime = 0.25f;
    public float recentDamageTime = 1.0f;

    private float time;
    private bool isFadedOut = true;
    private Health health;

    private Tweener recentDamage;

    private void Awake()
    {
        health = GetComponentInParent<Health>();
        health.onDamage.AddListener(OnHealthChange);
        health.onHeal.AddListener(OnHealthChange);

        recentDamage = DOTween.To(() => healthBarRecentDamage.fillAmount, x => healthBarRecentDamage.fillAmount = x, healthBar.fillAmount, recentDamageTime).SetAutoKill(false);

        time = timeToFade;
    }

    private void Update()
    {
        recentDamage.ChangeValues(healthBarRecentDamage.fillAmount, healthBar.fillAmount, recentDamageTime);

        time += Time.deltaTime;
        if (time >= timeToFade && !isFadedOut)
            canvas.DOFade(0.0f, fadeTime).OnComplete(() => isFadedOut = true);
    }

    private void OnHealthChange()
    {
        healthBar.fillAmount = health.GetHealthPercentage();

        time = 0.0f;
        if (isFadedOut)
            canvas.DOFade(1.0f, fadeTime).OnComplete(() => isFadedOut = false);
    }
}
