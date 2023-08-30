using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AIWeaponController : MonoBehaviour
{
    public float detectionRadius = 15.0f;

    private static GameObject player;
    private WeaponProjectile currentWeapon;
    private RotateTowards rotate;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<WorldComponentReference>().player;

        currentWeapon = GetComponentInChildren<WeaponProjectile>();
        rotate = GetComponent<RotateTowards>();

        rotate.target = null;
    }

    private void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= detectionRadius)
        {
            rotate.target = player.transform;
            Shoot();
        }
        else
            rotate.target = null;
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
