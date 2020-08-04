using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : Damageable
{
    protected Transform _playerAimPoint;

    void OnEnable()
    {
        EventManager.StartListening(EventType.GameOver, (p) => this.DisableAI(p));
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.GameOver, (p) => this.DisableAI(p));
    }

    void DisableAI(EventParam eventParam)
    {
        this.enabled = false;
    }

    protected virtual void Start()
    {
        _playerAimPoint = Constants.Player.GetComponent<PlayerLife>().AimPoint;
    }
}
