using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : Damageable
{
    public Transform AimPoint;
    public AudioClip HitFx;
    public AudioClip DeathFx;
    public float GameOverFadeToBlackTime;

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
        // camera falls
        GetComponent<Animator>().SetTrigger("Die");

        // death sound
        GetComponent<AudioSource>().PlayOneShot(DeathFx);

        // fade to black
        EventManager.TriggerEvent(EventType.GameOver, new GameOverEventParam { FadeToBlackTime = GameOverFadeToBlackTime });
    }
}
