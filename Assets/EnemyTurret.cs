using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Damageable 
{
    public Transform CanonTip;
    public Constants.PoolTag ProjectileType;
    public LayerMask LasersHitLayers;
    public float MaxShootDistance;
    public float RateOfFire;

    private void Start()
    {
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Constants.Player.transform, Vector3.up);
    }

    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);
        // FX
    }

    public override void Die()
    {
        base.Die();
        // FX
    }

    IEnumerator Attack()
    {
        while (true)
        {
            // TODO only do it if distance with player is short enough



            // if it can see player, shoot at it
            var attackDirection = (Constants.Player.transform.position - CanonTip.position) * 100;
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
