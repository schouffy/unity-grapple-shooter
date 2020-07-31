using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : MonoBehaviour, IPoolable
{
    public float Speed;
    public float Damage;
    public float Thickness;
    public float MaxLifetime = 2;
    public LayerMask WhatToHit;

    private float _spawnTime;
    private Vector3 _previousPosition;

    public void OnSpawn()
    {
        _spawnTime = Time.time;
    }

    public void Update()
    {
        _previousPosition = transform.position;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * 1000, Speed * Time.deltaTime);

        // raycast to previous position and check if anything is hit
        //Debug.DrawLine(_previousPosition, transform.position, Color.yellow, 1f);

        if (Physics.SphereCast(new Ray(_previousPosition, transform.position - _previousPosition), Thickness, 
            out var hitInfo, (transform.position - _previousPosition).magnitude, WhatToHit))
        {
            //Debug.Log("hit: " + hitInfo.collider.gameObject.name + " at " + hitInfo.point);
            DealDamage(hitInfo);
            gameObject.SetActive(false);
        }
        
        if (_spawnTime + MaxLifetime < Time.time)
            gameObject.SetActive(false);
    }

    private void DealDamage(RaycastHit hitInfo)
    {
        var objectToDamage = hitInfo.collider.gameObject;

        var damageable = objectToDamage.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(Damage);
        }
        else
        {
            // generic FX
            ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.LaserGenericImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal, Vector3.up));
        }
    }

}
