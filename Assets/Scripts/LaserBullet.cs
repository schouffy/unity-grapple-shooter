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

    public Constants.PoolTag ProjectileType;

    // This is how i handle the "target in the crosshair but there is an object in the bullet trajectory"
    // I only hit the target initially aimed, and ignore everything else
    // I display it on top of other layers to not having visual issues such as going through walls
    // Obviously it only works with hitscans or very rapid fire bullets
    private Vector3? _initialTarget; // Target that was in the crosshair initially (player shots only)
    public bool ShouldChangeLayer;
    public string InitialLayer;
    public string LayerToChangeTo;
    public float LayerChangeDelay;
    private bool _layerChanged;
    public Renderer ChangeLayerOn;

    public void OnSpawn()
    {
        _spawnTime = Time.time;
        _initialTarget = null;
        if (ShouldChangeLayer)
        {
            _layerChanged = false;
            ChangeLayerOn.gameObject.layer = LayerMask.NameToLayer(InitialLayer);
        }
    }

    public void Update()
    {
        if (ShouldChangeLayer)
        {
            if (!_layerChanged && _spawnTime + LayerChangeDelay < Time.time)
            {
                ChangeLayerOn.gameObject.layer = LayerMask.NameToLayer(LayerToChangeTo);
                _layerChanged = true;
            }
        }

        _previousPosition = transform.position;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * 1000, Speed * Time.deltaTime);

        // raycast to previous position and check if anything is hit
        //Debug.DrawLine(_previousPosition, transform.position, Color.yellow, 1f);

        if (Physics.SphereCast(new Ray(_previousPosition, transform.position - _previousPosition), Thickness, 
            out var hitInfo, (transform.position - _previousPosition).magnitude, WhatToHit))
        {
            if (_initialTarget.HasValue && Vector3.Distance(hitInfo.point, _initialTarget.Value) > 1)
            {
            }
            else
            {
                //Debug.Log("hit: " + hitInfo.collider.gameObject.name + " at " + hitInfo.point);
                DealDamage(hitInfo);
                gameObject.SetActive(false);
            }
        }
        
        if (_spawnTime + MaxLifetime < Time.time)
            gameObject.SetActive(false);
    }

    public void SetInitialTarget(Vector3 target)
    {
        _initialTarget = target;
    }

    private void DealDamage(RaycastHit hitInfo)
    {
        var objectToDamage = hitInfo.collider.gameObject;

        var damageable = objectToDamage.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(new DamageInfo { Damage = Damage, ImpactPoint = hitInfo.point, ImpactNormal = hitInfo.normal, ProjectileDirection = transform.forward, ProjectileType = ProjectileType });
        }
        else
        {
            // generic FX
            ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.LaserGenericImpact, hitInfo.point, Quaternion.LookRotation(hitInfo.normal, Vector3.up));
        }
    }

}
