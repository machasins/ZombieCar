using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    [Header("Shell Settings")]
    public GameObject shells;
    public int maxShells;
    public float shellMoveAmount;
    public float shellMoveTime;

    [Header("Percent Settings")]
    public GameObject percent;
    public float percentMoveTime;

    private static WorldComponentReference wcr;

    private WeaponController weaponController;

    private Tweener percentFill;
    private Image percentImage;
    private float percentFillAmount = 0.0f;

    private int currentShells;
    private float shellY;

    private void Start()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        weaponController = wcr.player.GetComponentInChildren<WeaponController>();

        SetDisplayMethod(weaponController.GetMaxClipSize());

        shellY = shells.transform.GetChild(0).localPosition.y - shellMoveAmount;
        for (int i = 0; i < 6; i++)
            if (i >= maxShells)
                shells.transform.GetChild(i).localPosition -= Vector3.up * shellMoveAmount;

        currentShells = maxShells;

        percentImage = percent.transform.GetChild(1).GetComponent<Image>();
        percentFill = DOTween.To(() => percentFillAmount, x => percentFillAmount = x, 1.0f, 1000).SetAutoKill(false);
    }

    private void Update()
    {
        if (wcr.player.activeInHierarchy)
        {
            SetDisplayMethod(weaponController.GetMaxClipSize());

            if (shells.activeInHierarchy)
                UpdateShells();
            if (percent.activeInHierarchy)
                UpdatePercent();
        }
    }

    private void SetDisplayMethod(int maxClipSize)
    {
        shells.SetActive(maxClipSize <= maxShells);
        percent.SetActive(maxClipSize > maxShells);
    }

    private void UpdateShells()
    {
        void Move(RectTransform t, float amount)
        {
            t.DOLocalMoveY(shellY + amount, shellMoveTime);
        }

        float per = (float)weaponController.GetClipRemaining() / weaponController.GetMaxClipSize();
        if (weaponController.IsReloading())
            per = weaponController.GetReloadProgress();

        per *= weaponController.GetMaxClipSize();

        int direction = (int)Mathf.Sign((int)per - currentShells);
        int max = ((int)per > currentShells) ? (int)per : currentShells;
        int min = ((int)per <= currentShells) ? (int)per : currentShells;

        for (int i = min; i != max; ++i)
        {
            Move(shells.transform.GetChild(i).GetComponent<RectTransform>(), shellMoveAmount * direction);
        }

        currentShells = (int)per;
    }

    private void UpdatePercent()
    {
        float per = (float)weaponController.GetClipRemaining() / weaponController.GetMaxClipSize();
        if (weaponController.IsReloading())
            per = weaponController.GetReloadProgress();
        percentFill.ChangeValues(percentFillAmount, per, percentMoveTime);

        percentImage.fillAmount = percentFillAmount;
    }
}
