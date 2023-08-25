using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public InputActionReference weaponFireInput;

    private WeaponProjectile currentWeapon;
    private InputAction input;

    private bool isFiring = false;

    private void Start()
    {
        currentWeapon = GetComponentInChildren<WeaponProjectile>();
        input = weaponFireInput.ToInputAction();
    }

    private void Update()
    {
        if (!isFiring && input.WasPressedThisFrame())
            isFiring = true;
        if (isFiring && input.WasReleasedThisFrame())
            isFiring = false;

        if (isFiring)
            Shoot();
    }

    public void ChangeWeapon(WeaponProjectile weapon)
    {
        currentWeapon = weapon;
    }

    public void Shoot()
    {
        currentWeapon.Shoot();
    }
}
