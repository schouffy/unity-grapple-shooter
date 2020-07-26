using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : Damageable
{
    public Transform AimPoint;

    void Start()
    {
        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new IntegerEventParam() { Value = (int)Health });
    }

    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);
        //Debug.Log("Taken " + Damage + " damage");

        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new IntegerEventParam() { Value = (int)Health });
    }

    public override void Die()
    {
        EventManager.TriggerEvent(EventType.GameOver, null);
    }
}
