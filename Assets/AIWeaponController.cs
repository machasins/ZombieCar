using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RotateTowards))]
public class AIWeaponController : MonoBehaviour
{
    public float detectionRadius = 15.0f;
    public float shootingRadius = 5.0f;
    public float predictionPercent = 0.5f;

    private static Rigidbody2D player;
    private WeaponProjectile currentWeapon;
    private RotateTowards rotate;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<WorldComponentReference>().player.GetComponent<Rigidbody2D>();

        currentWeapon = GetComponentInChildren<WeaponProjectile>();
        rotate = GetComponent<RotateTowards>();

        rotate.target = null;
    }

    private void Update()
    {
        float playerDistance = Vector2.Distance(player.transform.position, transform.position);
        if (playerDistance <= detectionRadius)
        {
            rotate.SetPosition(player.transform.position + (Vector3)player.velocity * predictionPercent);
            if (playerDistance <= shootingRadius)
                Shoot();
        }
        else
            rotate.SetPosition(transform.position + 5 * transform.up);
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
