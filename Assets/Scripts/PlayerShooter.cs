using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform BulletSpawnLocation;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire"))
        {
            ObjectPool.Instance.SpawnFromPool(Constants.PoolTag.PlayerLaserBullet, BulletSpawnLocation.position, BulletSpawnLocation.rotation);
        }
    }
}
