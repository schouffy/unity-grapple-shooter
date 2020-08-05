using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : EnemyAI 
{
    public Transform CanonTip;
    public Transform Turret;
    public Transform Barrel;
    public Constants.PoolTag ProjectileType;
    public LayerMask LasersHitLayers;
    public float MaxShootDistance;
    public float RateOfFire;
    public GameObject ImpactPrefab;
    public GameObject ExplosionPrefab;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        // rotate Turret on Y axis
        var pointToRotateXZTo = new Vector3(_playerAimPoint.position.x, Turret.position.y, _playerAimPoint.position.z);
        Turret.LookAt(pointToRotateXZTo, Vector3.up);
        // Rotate barrel on X axis
        Barrel.LookAt(_playerAimPoint, Vector3.up);
    }

    public override void TakeDamage(float Damage, Vector3 position, Vector3? projectileDirection)
    {
        Instantiate(ImpactPrefab, position, Quaternion.identity);
        base.TakeDamage(Damage, position, projectileDirection);
    }

    public override void Die()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        base.Die();
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);

        while (this.enabled)
        {
            // TODO only do it if distance with player is short enough



            // if it can see player, shoot at it
            var attackDirection = (_playerAimPoint.position - CanonTip.position);// * 100;
            Debug.DrawRay(CanonTip.position, attackDirection, Color.red);
            if (Physics.Raycast(CanonTip.position, attackDirection, out var hitInfo, MaxShootDistance, LasersHitLayers))
            {
                if (hitInfo.collider.gameObject.tag == Constants.PlayerTag)
                {
                    // Fire
                    ObjectPool.Instance.SpawnFromPool(ProjectileType, CanonTip.position, Quaternion.LookRotation(attackDirection, Vector3.up));
                    yield return new WaitForSeconds(1f / RateOfFire);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
