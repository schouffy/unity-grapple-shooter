using System.Collections;
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

    public override void TakeDamage(float Damage, Vector3 position, Vector3? projectileDirection)
    {
        base.TakeDamage(Damage, position, projectileDirection);
        GetComponent<AudioSource>().PlayOneShot(HitFx);
        //Debug.Log("Taken " + Damage + " damage");

        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new HealthEventParam() { Health = (int)Health, ProjectileDirection = projectileDirection });
    }

    public override void Die()
    {
        EventManager.TriggerEvent(EventType.GameOver, null);
    }
}
