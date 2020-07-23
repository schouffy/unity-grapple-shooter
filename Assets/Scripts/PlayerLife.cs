using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : Damageable
{
    public Transform AimPoint;

    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);
        //Debug.Log("Taken " + Damage + " damage");
    }

    public override void Die()
    {
        Debug.Log("Player died");
    }
}
