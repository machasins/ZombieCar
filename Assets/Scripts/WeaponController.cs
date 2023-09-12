using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    private bool isFiring = false;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
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
        GetComponentInChildren<WeaponProjectile>().Shoot();
    }
}
