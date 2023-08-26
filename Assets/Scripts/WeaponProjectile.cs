using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxClipSize = 6;
    public int projectilesPerShot = 1;
    public float reloadDelay = 1;
    public float shotDelay = 1;
    public float shotSpread = 0;
    public float shotSpeedVarience = 0;

    [Header("Projectile Settings")]
    public float projDamage;
    public float projDamageCooldown;
    public Damage.Type projType = Damage.Type.Player;
    public float projSpeed = 1;
    public float projAcceleration = 0;
    public float projLifetime = 5;
    public Vector2 projScale = new(1,1);
    public float projScaleRate = 0;
    public float projRotationRate = 0;

    private static ProjectileFactory factory;
    private Rigidbody2D car;

    private bool isReloading = false;
    private bool isShotDelayed = false;

    private int currentClip = 0;

    private void Start()
    {
        if (!factory)
            factory = FindObjectOfType<ProjectileFactory>();
        
        car = transform.root.GetComponent<Rigidbody2D>();

        currentClip = maxClipSize;
    }

    public void Shoot()
    {
        if (isReloading || isShotDelayed)
            return;
        
        for (int i = 0; i < projectilesPerShot; i++)
        {
            ProjectileFactory.ProjectileData data = new()
            {
                spawnSpeed = projSpeed + Random.Range(-shotSpeedVarience, shotSpeedVarience),
                spawnScale = projScale,
                spawnAcceleration = projAcceleration,
                inheritedVelocity = car.velocity,
                lifetime = projLifetime,
                damage = projDamage,
                damageCooldown = projDamageCooldown,
                type = projType,
                scaleRate = projScaleRate,
                rotationRate = projRotationRate,

                spawnPosition = transform.position,
                spawnAngle = transform.rotation.eulerAngles.z + Random.Range(-shotSpread, shotSpread)
            };

            factory.Spawn(data);
        }

        isShotDelayed = true;
        StartCoroutine(General.CallbackAfterTime(shotDelay, () => isShotDelayed = false ));

        if (--currentClip <= 0)
        {
            isReloading = true;
            StartCoroutine(General.CallbackAfterTime(reloadDelay, () => {
                currentClip = maxClipSize;
                isReloading = false;
            }));
        }
    }
}
