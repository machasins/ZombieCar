using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Damage))]
public class ProjectileHandler : MonoBehaviour
{
    ProjectileFactory.ProjectileData projectileData;
    Rigidbody2D rb;

    float currentLife = 0.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.drag = 0;
        rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        transform.localScale += projectileData.scaleRate * Time.fixedDeltaTime * Vector3.one;
        transform.localRotation *= Quaternion.Euler(0, 0, projectileData.rotationRate * Time.fixedDeltaTime);

        rb.velocity += Time.fixedDeltaTime * projectileData.spawnAcceleration * rb.velocity;

        currentLife -= Time.fixedDeltaTime;
        if (currentLife < 0.0f)
            gameObject.SetActive(false);
    }

    public void SetupProjectile(ProjectileFactory.ProjectileData data)
    {
        projectileData = data;

        transform.SetPositionAndRotation(data.spawnPosition, Quaternion.Euler(new Vector3(0, 0, data.spawnAngle)));
        transform.localScale = data.spawnScale;

        Damage d = GetComponent<Damage>();
        d.damageAmount = data.damage;
        d.damageType = data.type;

        if (!rb)
            Awake();

        rb.velocity = transform.up * data.spawnSpeed + (Vector3)data.inheritedVelocity;

        currentLife = data.lifetime;
    }
}
