﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : Damageable
{
    public Transform AimPoint;
    public AudioClip HitFx;

    void Start()
    {
        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new HealthEventParam() { Health = (int)Health });
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);
        GetComponent<AudioSource>().PlayOneShot(HitFx);
        //Debug.Log("Taken " + Damage + " damage");

        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new HealthEventParam() { Health = (int)Health, ProjectileDirection = damageInfo.ProjectileDirection });
    }

    public override void Die()
    {
        // TODO death sound
        // camera falls
        // fade to black
        EventManager.TriggerEvent(EventType.GameOver, null);
    }
}
