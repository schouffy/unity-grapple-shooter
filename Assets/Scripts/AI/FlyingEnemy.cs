using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : BaseFlyingEnemy
{
    public Constants.PoolTag ProjectileType;
    public Transform[] BarrelTips;
    private int _currentBarrelTip = 0;
    public LayerMask LasersHitLayers;
    public float AttackSpeed;
    public float MaxShootDistance;
    public float RateOfFire;

    public GameObject ImpactPrefab;
    public GameObject ExplosionPrefab;
    public GameObject DestroyedEnemyPrefab;
    public AudioClip LaserSound;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Status == EnemyStatus.Idle)
        {
            Idle();
        }
    }

    Transform GetNextBarrelTip()
    {
        _currentBarrelTip = (_currentBarrelTip + 1) % BarrelTips.Length;
        return BarrelTips[_currentBarrelTip];
    }

    protected override IEnumerator Attack()
    {
        Mesh.materials = _attackMaterials;

        while (this.enabled)
        {
            var barrelTip = GetNextBarrelTip();
            var attackDirection = (_playerAimPoint.position - barrelTip.position);
            var canCast = Physics.SphereCast(transform.position, 0.5f, _playerAimPoint.position - transform.position, out var hitInfo, 100, LasersHitLayers);
            bool canSeePlayer = canCast && hitInfo.collider != null && hitInfo.collider.gameObject.tag == Constants.PlayerTag;

            if (canSeePlayer && Vector3.Distance(transform.position, _playerAimPoint.position) < MaxShootDistance)
            {
                // if in range and player is visible, shoot player
                if (hitInfo.collider.gameObject.tag == Constants.PlayerTag)
                {
                    // Fire
                    ObjectPool.Instance.SpawnFromPool(ProjectileType, barrelTip.position, Quaternion.LookRotation(attackDirection, Vector3.up));
                    GetComponent<AudioSource>().PlayOneShot(LaserSound);
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
                if (Vector3.Distance(_currentPatrollingDestination, transform.position) < 2)
                {
                    // Get a new destination around the starting point
                    _currentPatrollingDestination = GetRandomPointAround(transform.position, 20f);
                }

                // move to _currentPatrollingDestination
                transform.position = Vector3.MoveTowards(transform.position, _currentPatrollingDestination, AttackSpeed * Time.deltaTime);
            }
            transform.LookAt(_playerAimPoint);

            yield return null;
        }
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        Instantiate(ImpactPrefab, damageInfo.ImpactPoint, Quaternion.identity);
        base.TakeDamage(damageInfo);

        if (Status == EnemyStatus.Idle)
        {
            Status = EnemyStatus.Attacking;
            StartCoroutine(Attack());
        }
    }

    public override void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        Instantiate(DestroyedEnemyPrefab, transform.position, Quaternion.identity);
        base.Die();
    }
}
