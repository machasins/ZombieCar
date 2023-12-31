using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    public Rigidbody2D mainCarController;

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

    private bool isReloading = false;
    private bool isShotDelayed = false;

    private float reloadProgress;

    private int currentClip = 0;

    private void Start()
    {
        if (!factory)
            factory = FindObjectOfType<ProjectileFactory>();
    }

    private void OnEnable()
    {
        reloadProgress = 1.0f;

        isReloading = false;
        isShotDelayed = false;

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
                inheritedVelocity = mainCarController.velocity,
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
            StartCoroutine(General.CallbackAfterTimeTracker(reloadDelay,
                x => { reloadProgress = x / reloadDelay; }, 
                () => { currentClip = maxClipSize; isReloading = false; }
            ));
        }
    }

    public int GetCurrentClip()
    {
        return currentClip;
    }

    public float GetReloadProgress()
    {
        return reloadProgress;
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
