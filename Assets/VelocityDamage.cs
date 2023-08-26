using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityDamage : Damage
{
    public float minSpeed;
    public float maxSpeed;

    public float minDamage;

    private Rigidbody2D car;

    private void Start()
    {
        car = GetComponentInParent<Rigidbody2D>();
        if (car == null)
        {
            Debug.LogError("VelocityDamage put on object without a rigidbody in parent.");
            this.enabled = false;
        }
    }

    protected override bool DoDamageCheck(Health h)
    {
        return base.DoDamageCheck(h) && car.velocity.magnitude >= minSpeed;
    }

    protected override void DoDamage(Health h)
    {
        if (h.Damage(Mathf.Lerp(minDamage, damageAmount, (car.velocity.magnitude - minSpeed) / (maxSpeed - minSpeed))))
            StartCooldown();
    }
}
