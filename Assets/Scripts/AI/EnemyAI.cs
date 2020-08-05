using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : Damageable
{
    protected Transform _playerAimPoint;

    void OnEnable()
    {
        EventManager.StartListening(EventType.GameOver, this.DisableAI);
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.GameOver, this.DisableAI);
    }

    void DisableAI(EventParam eventParam)
    {
        this.enabled = false;
    }

    protected virtual void Start()
    {
        _playerAimPoint = Constants.Player.GetComponent<PlayerLife>().AimPoint;
    }

    public override void Die()
    {
        OnDisable();
        base.Die();
    }
}
