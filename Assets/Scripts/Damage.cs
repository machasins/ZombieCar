using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy
    };

    public float damageAmount;
    public float damageCooldown;
    public Type damageType;

    protected bool onCooldown = false;

    protected virtual bool DoDamageCheck(Health h)
    {
        return h && h.damageType == damageType && !onCooldown;
    }

    protected virtual void DoDamage(Health h)
    {
        if (h.Damage(damageAmount))
            StartCooldown();
    }

    protected void StartCooldown()
    {
        onCooldown = true;
        StartCoroutine(General.CallbackAfterTime(damageCooldown, () => onCooldown = false));
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Health h = other.GetComponent<Health>();
        if (DoDamageCheck(h))
            DoDamage(h);
    }
}
