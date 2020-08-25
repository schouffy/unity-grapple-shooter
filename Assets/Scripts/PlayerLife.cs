using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : Damageable
{
    public Transform AimPoint;
    public AudioClip HitFx;
    public AudioClip DeathFx;
    public AudioClip AutoHealFx;
    public float GameOverFadeToBlackTime;

    private float _initialHealth;
    private float _lastDamageTime;
    public float DelayBeforeStartAutoHeal;
    public float AutoHealRate;
    private bool _healing;

    void Start()
    {
        _initialHealth = Health;
        EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new HealthEventParam() { Health = (int)Health });
    }

    private void Update()
    {
        if (Health < _initialHealth && !_healing && _lastDamageTime + DelayBeforeStartAutoHeal <= Time.time)
        {
            StartCoroutine(AutoHeal());
            _healing = true;
        }
    }

    IEnumerator AutoHeal()
    {
        GetComponent<AudioSource>().PlayOneShot(AutoHealFx);
        while (Health < _initialHealth)
        {
            if (_lastDamageTime + DelayBeforeStartAutoHeal > Time.time)
            {
                _healing = false;
                GetComponent<AudioSource>().Stop();
                break;
            }

            Health += AutoHealRate / 10f;
            EventManager.TriggerEvent(EventType.PlayerHealthUpdated, new HealthEventParam() { Health = (int)Health });

            yield return new WaitForSeconds(0.1f);
        }
        _healing = false;
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        _lastDamageTime = Time.time;
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
