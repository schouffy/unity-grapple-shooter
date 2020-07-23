using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBullet : MonoBehaviour, IPoolable
{
    public float Speed;

    public void OnSpawn()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }
}
