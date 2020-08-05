using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float Health;

    public virtual void TakeDamage(float Damage, Vector3 impactPoint, Vector3? projectileDirection)
    {
        if (Health <= 0)
            return;

        Health -= Damage;

        if (Health < 0)
            Health = 0;

        if (Health == 0)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
