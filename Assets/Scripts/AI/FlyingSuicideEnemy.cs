﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSuicideEnemy : BaseFlyingEnemy
{
    public float ExplosionDamage;
    public float ExplosionRadius;
    public float AttackSpeed;
    public LayerMask VisionLayers;

    public GameObject ImpactPrefab;
    public GameObject ExplosionPrefab;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _startingPoint = transform.position;
        _currentPatrollingDestination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Status == EnemyStatus.Idle)
        {
            Idle();
        }
    }

    protected override IEnumerator Attack()
    {
        Mesh.materials = _attackMaterials;

        while (this.enabled)
        {
            var attackDirection = (_playerAimPoint.position - transform.position);
            var canCast = Physics.SphereCast(transform.position, 0.3f, attackDirection, out var hitInfo, 100, VisionLayers);
            bool canSeePlayer = canCast && hitInfo.collider != null && hitInfo.collider.gameObject.tag == Constants.PlayerTag;

            if (canSeePlayer && Vector3.Distance(transform.position, _playerAimPoint.position) < 2f)
            {
                // if in range and player is visible, shoot player
                Die();
                break;
            }
            else if (canSeePlayer)
            {
                // if player not in range, move to the player
                transform.position = Vector3.MoveTowards(transform.position, _playerAimPoint.position, AttackSpeed * Time.deltaTime);
            }
            else
            {
                // if any obstacle to move or shoot at player, move in a random direction until player is visible
                if (Vector3.Distance(_currentPatrollingDestination, transform.position) < 1)
                {
                    // Get a new destination around the starting point
                    _currentPatrollingDestination = GetRandomPointAround(transform.position, 8f);
                }

                // move to _currentPatrollingDestination
                transform.position = Vector3.MoveTowards(transform.position, _currentPatrollingDestination, AttackSpeed * Time.deltaTime);
            }
            transform.LookAt(_playerAimPoint);

            yield return null;
        }
    }

    public override void TakeDamage(float Damage, Vector3 position, Vector3? projectileDirection)
    {
        Instantiate(ImpactPrefab, position, Quaternion.identity);
        base.TakeDamage(Damage, position, projectileDirection);

        if (Status == EnemyStatus.Idle)
        {
            Status = EnemyStatus.Attacking;
            StartCoroutine(Attack());
        }
    }

    public override void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        var surroundingEntities = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var entity in surroundingEntities)
        {
            var damageable = entity.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(ExplosionDamage, transform.position, null);
            }
        }

        base.Die();
    }
}
