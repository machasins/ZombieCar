using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public Health healthRelay;

    public float maxHealth;
    public float invincibilityTime;
    public Damage.Type damageType;

    public UnityEvent onDamage;
    public UnityEvent onHeal;
    public UnityEvent onDeath;

    private bool isHealthRelay = false;

    private float health;
    private bool isInvincible = false;
    private bool hasDied = false;

    private void Awake()
    {
        health = maxHealth;
    }

    private void Start()
    {
        isHealthRelay = healthRelay != null;
    }

    private void Update()
    {
        if (!IsAlive() && !hasDied)
        {
            onDeath.Invoke();
            hasDied = true;
        }
    }

    public bool IsAlive()
    {
        if (isHealthRelay)
            return healthRelay.IsAlive();
        return health > 0;
    }

    public bool Damage(float amount)
    {
        if (isInvincible)
            return false;

        onDamage.Invoke();

        if (isHealthRelay)
            return healthRelay.Damage(amount);

        health = Mathf.Clamp(health - amount, 0, maxHealth);

        if (invincibilityTime > 0)
        {
            isInvincible = true;
            StartCoroutine(General.CallbackAfterTime(invincibilityTime, () => isInvincible = false));
        }

        Debug.Log("damaged " + gameObject.name + ", amount " + amount);

        return true;
    }

    public void Heal(float amount)
    {
        if (isHealthRelay)
            healthRelay.Heal(amount);
        else
            health = Mathf.Clamp(health + amount, 0, maxHealth);

        onHeal.Invoke();
    }

    public float GetHealthPercentage()
    {
        return health / maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }
}
