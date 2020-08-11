using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public float Damage;
    public Vector3 ImpactPoint;
    public Vector3? ImpactNormal;
    public Vector3? ProjectileDirection;
    public Constants.PoolTag? ProjectileType;
}

public class Damageable : MonoBehaviour
{
    public float Health;

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (Health <= 0)
            return;

        Health -= damageInfo.Damage;

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
