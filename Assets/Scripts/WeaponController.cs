using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private bool isFiring = false;

    private static WorldComponentReference wcr;

    private WeaponProjectile currentWeapon;

    private void Awake()
    {
        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void OnEnable()
    {
        isFiring = false;
    }

    private void Update()
    {
        if (!isFiring && wcr.weaponFireInput.action.WasPressedThisFrame())
            isFiring = true;
        if (isFiring && wcr.weaponFireInput.action.WasReleasedThisFrame())
            isFiring = false;

        if (isFiring)
            Shoot();
    }

    public void Shoot()
    {
        RefreshCurrentWeapon();

        if (gameObject.activeInHierarchy)
            currentWeapon.Shoot();
    }

    public int GetClipRemaining()
    {
        RefreshCurrentWeapon();

        if (gameObject.activeInHierarchy)
            return currentWeapon.GetCurrentClip();
        return 0;
    }

    public int GetMaxClipSize()
    {
        RefreshCurrentWeapon();

        if (gameObject.activeInHierarchy)
            return currentWeapon.maxClipSize;
        return 0;
    }

    public float GetReloadProgress()
    {
        RefreshCurrentWeapon();

        if (gameObject.activeInHierarchy)
            return currentWeapon.GetReloadProgress();
        return 1;
    }

    public bool IsReloading()
    {
        RefreshCurrentWeapon();

        if (gameObject.activeInHierarchy)
            return currentWeapon.IsReloading();
        return false;
    }

    private void RefreshCurrentWeapon()
    {
        if (!currentWeapon || !currentWeapon.isActiveAndEnabled)
            currentWeapon = GetComponentInChildren<WeaponProjectile>();
    }
}
