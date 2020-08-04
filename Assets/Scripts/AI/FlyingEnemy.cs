﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyAI
{
    public EnemyStatus Status;
    
    private Vector3 _startingPoint;
    private Vector3 _currentPatrollingDestination;
    public float MaxPatrolDistanceFromStartingPoint;
    public float PatrolSpeed;
    public float DetectionRange;

    public Constants.PoolTag ProjectileType;
    public Transform BarrelTip;
    public LayerMask LasersHitLayers;
    public float AttackSpeed;
    public float MaxShootDistance;
    public float RateOfFire;

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
            if (Vector3.Distance(_playerAimPoint.position, transform.position) < DetectionRange)
            {
                Status = EnemyStatus.Attacking;
                StartCoroutine(Attack());
                return;
            }

            if (Vector3.Distance(_startingPoint, transform.position) > MaxPatrolDistanceFromStartingPoint
                || Vector3.Distance(_currentPatrollingDestination, transform.position) < 1)
            {
                // Get a new destination around the starting point
                _currentPatrollingDestination = GetRandomPointAround(_startingPoint, MaxPatrolDistanceFromStartingPoint);
            }

            // move to _currentPatrollingDestination
            transform.position = Vector3.MoveTowards(transform.position, _currentPatrollingDestination, PatrolSpeed * Time.deltaTime);
            transform.LookAt(_currentPatrollingDestination);
        }
    }

    Vector3 GetRandomPointAround(Vector3 center, float maxRadius)
    {
        int maxAttempts = 200;
        int i = 0;
        while (i < maxAttempts)
        {
            var candidate = center + Random.insideUnitSphere * maxRadius;
            if (Physics.OverlapSphere(candidate, 4f).Length == 0)
            {
                return candidate;
            }
        }
        throw new System.Exception("Couldn't find a random point.");
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);

        while (this.enabled)
        {
            var attackDirection = (_playerAimPoint.position - BarrelTip.position) * 100;
            Physics.Raycast(BarrelTip.position, attackDirection, out var hitInfo, 100, LasersHitLayers);
            bool canSeePlayer = hitInfo.collider != null && hitInfo.collider.gameObject.tag == Constants.PlayerTag;

            if (canSeePlayer && Vector3.Distance(transform.position, _playerAimPoint.position) < MaxShootDistance)
            {
                // if in range and player is visible, shoot player
                Debug.DrawRay(BarrelTip.position, attackDirection, Color.red);
                if (hitInfo.collider.gameObject.tag == Constants.PlayerTag)
                {
                    // Fire
                    ObjectPool.Instance.SpawnFromPool(ProjectileType, BarrelTip.position, BarrelTip.rotation);
                    yield return new WaitForSeconds(1f / RateOfFire);
                }
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
                    _currentPatrollingDestination = GetRandomPointAround(transform.position, 5f);
                }

                // move to _currentPatrollingDestination
                transform.position = Vector3.MoveTowards(transform.position, _currentPatrollingDestination, AttackSpeed * Time.deltaTime);
            }
            transform.LookAt(_playerAimPoint);

            yield return null;
        }
    }

    public override void TakeDamage(float Damage, Vector3 position)
    {
        Instantiate(ImpactPrefab, position, Quaternion.identity);
        base.TakeDamage(Damage, position);
    }

    public override void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        base.Die();
    }
}