using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform BulletSpawnLocation;
    public LayerMask LasersHitLayers;

    public float RateOfFire;
    private float _lastFireTime = 0;

    public Animator Animator;


    void Update()
    {
        if (Input.GetButton("Fire") && (_lastFireTime + 1f / RateOfFire) <= Time.time)
        {
            Animator.SetTrigger("Shoot");

            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hitInfo, 100.0f);
            var target = hitInfo.point;
            if (target == Vector3.zero)
                target = transform.position + Camera.main.transform.forward * 100f;
            _lastFireTime = Time.time;

            ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.PlayerLaserBullet, BulletSpawnLocation.position, Quaternion.LookRotation(target - BulletSpawnLocation.position, Vector3.up));
        }
    }
}
