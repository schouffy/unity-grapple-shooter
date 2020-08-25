using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : DisableOnGameOverMonoBehaviour
{
    public Transform BulletSpawnLocation;
    public LayerMask LasersHitLayers;

    [Header("Gun")]
    public float RateOfFire;
    public float MaxEnergy = 100;
    public float _currentEnergy;
    public float ShotEnergyCost = 7;
    public float EnergyRetrievingRate = 10;
    public float CoolDownDelay = 2;
    private float _lastFireTime = 0;

    public Animator Animator;
    public CameraShake CameraShake;

    [Header("Sounds")]
    public AudioSource AudioSource;
    public AudioClip BlasterSound;
    public AudioClip NotEnoughEnergyFx;
    public AudioClip CoolDownFx;

    private void Start()
    {
        _currentEnergy = 0;
    }

    void Update()
    {
        if (Input.GetButton("Fire") && (_lastFireTime + 1f / RateOfFire) <= Time.time && _currentEnergy < MaxEnergy)
        {
            _lastFireTime = Time.time;
            _currentEnergy += ShotEnergyCost;
            if (_currentEnergy > MaxEnergy)
            {
                _currentEnergy = MaxEnergy;
                AudioSource.PlayOneShot(NotEnoughEnergyFx);
            }

            Animator.SetTrigger("Shoot");
            AudioSource.PlayOneShot(BlasterSound);
            CameraShake.ShakeFromShooting();

            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hitInfo, 100.0f);
            
            var target = hitInfo.point;
            
            if (target == Vector3.zero)
                target = transform.position + Camera.main.transform.forward * 100f;
            
            Quaternion spawnDirection = Quaternion.LookRotation(target - BulletSpawnLocation.position, Vector3.up);

            var laser = ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.PlayerLaserBullet, BulletSpawnLocation.position, spawnDirection);
            laser.GetComponent<LaserBullet>().SetInitialTarget(target);
        }
        if (_currentEnergy > 0 && _lastFireTime + CoolDownDelay < Time.time)
        {
            if (_currentEnergy == MaxEnergy)
                AudioSource.PlayOneShot(CoolDownFx);

            _currentEnergy -= EnergyRetrievingRate * Time.deltaTime;
            if (_currentEnergy < 0)
                _currentEnergy = 0;
        }
        if (Input.GetButtonDown("Fire") && _currentEnergy == MaxEnergy)
        {
            AudioSource.PlayOneShot(NotEnoughEnergyFx);
        }
    }
}
