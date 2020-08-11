using System.Collections;
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

    public AudioClip[] LaserBounces;
    public AudioSource CountdownBeepAudioSource;
    public AnimationCurve CountdownPitchCurve;

    protected float TimeBeforeExplosion;


    protected override void Start()
    {
        base.Start();
        _startingPoint = transform.position;
        _currentPatrollingDestination = transform.position;
    }

    void Update()
    {
        if (Status == EnemyStatus.Idle)
        {
            Idle();
        }
    }

    protected override IEnumerator Attack()
    {

        // Calcul le temps nécessaire pour arriver jusqu'au joueur et règle le compte à rebours
        // émet un bip de plus en plus rapide en fonction du temps restant
        var distance = Vector3.Distance(_playerAimPoint.position, transform.position);
        TimeBeforeExplosion = distance / AttackSpeed;
        float _previousBeepTime = 0;

        //Mesh.materials = _attackMaterials;

        while (this.enabled)
        {
            if (TimeBeforeExplosion <= 0)
            {
                Die();
                break;
            }

            if (_previousBeepTime + (1f / (10f / TimeBeforeExplosion)) < Time.time)
            {
                _previousBeepTime = Time.time;
                StartCoroutine(Blink(TimeBeforeExplosion));
            }

            var attackDirection = (_playerAimPoint.position - transform.position);
            var canCast = Physics.SphereCast(transform.position, 0.3f, attackDirection, out var hitInfo, 100, VisionLayers);
            bool canSeePlayer = canCast && hitInfo.collider != null && hitInfo.collider.gameObject.tag == Constants.PlayerTag;

            if (canSeePlayer && Vector3.Distance(transform.position, _playerAimPoint.position) < 2f)
            {
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

            TimeBeforeExplosion -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Blink(float timeBeforeExplosion)
    {
        Mesh.materials = _attackMaterials;
        CountdownBeepAudioSource.pitch = CountdownPitchCurve.Evaluate(1 - Mathf.Clamp01(timeBeforeExplosion / 10f));
        CountdownBeepAudioSource.Play();
        yield return new WaitForSeconds(0.1f);
        Mesh.materials = _idleMaterials;
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.ImpactNormal.HasValue && damageInfo.ProjectileType.HasValue)
        {
            ObjectPool.Instance.SpawnFromPool(damageInfo.ProjectileType.Value, damageInfo.ImpactPoint, Quaternion.LookRotation(damageInfo.ImpactNormal.Value));
            GetComponent<AudioSource>().PlayOneShot(LaserBounces[Random.Range(0, LaserBounces.Length)]);
        }

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
                damageable.TakeDamage(new DamageInfo { Damage = ExplosionDamage, ImpactPoint = transform.position });
            }
        }

        base.Die();
    }
}
