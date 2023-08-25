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
    public Type damageType;

    private void OnTriggerStay2D(Collider2D other)
    {
        Health h = other.GetComponent<Health>();
        if (h && h.damageType == damageType)
        {
            print("damaged " + other.gameObject.name);
            h.Damage(damageAmount);
        }
    }
}
