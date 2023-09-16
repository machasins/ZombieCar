using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDamage : Damage
{
    [Tooltip("The percent of max speed that will cause minumal damage")]
    [Range(0.0f, 1.0f)] public float minSpeed;
    [Tooltip("The percent of max speed that will cause maximal damage")]
    [Range(0.0f, 1.0f)] public float maxSpeed;

    public float minDamage;

    private static WorldComponentReference wcr;

    private void Start()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    protected override bool DoDamageCheck(Health h)
    {
        return base.DoDamageCheck(h) && wcr.player.GetComponent<Rigidbody2D>().velocity.magnitude >= wcr.player.GetComponent<CarController>().maxSpeed * minSpeed;
    }

    protected override void DoDamage(Health h)
    {
        float speedLimit = wcr.player.GetComponent<CarController>().maxSpeed;
        if (h.Damage(Mathf.Lerp(minDamage, damageAmount, (wcr.player.GetComponent<Rigidbody2D>().velocity.magnitude - speedLimit * minSpeed) / (speedLimit * (maxSpeed - minSpeed)))))
            StartCooldown();
    }
}
