using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : Damageable
{
    protected Transform _playerAimPoint;

    public Material IdleHalo;
    public Material AttackHalo;
    public Renderer Mesh;
    protected Material[] _idleMaterials;
    protected Material[] _attackMaterials;

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
        _idleMaterials = new Material[] { Mesh.materials[0], IdleHalo };
        _attackMaterials = new Material[] { Mesh.materials[0], AttackHalo };
        Mesh.materials = _idleMaterials;

        _playerAimPoint = Constants.Player.GetComponent<PlayerLife>().AimPoint;
    }

    public override void Die()
    {
        OnDisable();
        base.Die();
    }
}
