using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : EnemyAI 
{
    public EnemyStatus Status;

    public Transform CanonTip;
    public Transform Turret;
    public Transform Barrel;
    public Constants.PoolTag ProjectileType;
    public LayerMask LasersHitLayers;
    public float MaxShootDistance;
    public float RateOfFire;
    public GameObject ImpactPrefab;
    public GameObject ExplosionPrefab;
    public GameObject DestroyedEnemyPrefab;
    public AudioSource LaserAudioSource;
    public AudioClip LaserSound;

    protected override void Start()
    {
        Status = EnemyStatus.Idle;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Status == EnemyStatus.Idle)
        {
            if (Vector3.Distance(_playerAimPoint.position, transform.position) < MaxShootDistance)
            {
                Status = EnemyStatus.Attacking;
                StartCoroutine(Attack());
            }
        }
        else
        {
            // rotate Turret on Y axis
            var pointToRotateXZTo = new Vector3(_playerAimPoint.position.x, Turret.position.y, _playerAimPoint.position.z);
            Turret.LookAt(pointToRotateXZTo, Vector3.up);
            // Rotate barrel on X axis
            Barrel.LookAt(_playerAimPoint, Vector3.up);
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

    IEnumerator Attack()
    {
        Mesh.materials = _attackMaterials;

        while (this.enabled)
        {

            // if it can see player, shoot at it
            var attackDirection = (_playerAimPoint.position - CanonTip.position);// * 100;
            if (Physics.Raycast(CanonTip.position, attackDirection, out var hitInfo, MaxShootDistance, LasersHitLayers))
            {
                if (hitInfo.collider.gameObject.tag == Constants.PlayerTag)
                {
                    // Fire
                    ObjectPool.Instance.SpawnFromPool(ProjectileType, CanonTip.position, Quaternion.LookRotation(attackDirection, Vector3.up));
                    LaserAudioSource.PlayOneShot(LaserSound);
                    yield return new WaitForSeconds(1f / RateOfFire);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
