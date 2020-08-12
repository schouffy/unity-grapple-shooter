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
    public CameraShake CameraShake;

    [Header("Sound")]
    public AudioSource AudioSource;
    public AudioClip BlasterSound;


    void Update()
    {
        //Animator.SetBool("IsShooting", Input.GetButton("Fire"));

        if (Input.GetButton("Fire") && (_lastFireTime + 1f / RateOfFire) <= Time.time)
        {
            Animator.SetTrigger("Shoot");
            AudioSource.PlayOneShot(BlasterSound);
            CameraShake.ShakeFromShooting();

            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hitInfo, 100.0f);
            
            var target = hitInfo.point;
            
            if (target == Vector3.zero)
                target = transform.position + Camera.main.transform.forward * 100f;
            _lastFireTime = Time.time;

            Quaternion spawnDirection = Quaternion.LookRotation(target - BulletSpawnLocation.position, Vector3.up);

            var laser = ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.PlayerLaserBullet, BulletSpawnLocation.position, spawnDirection);
            laser.GetComponent<LaserBullet>().SetInitialTarget(target);
        }
    }
}
