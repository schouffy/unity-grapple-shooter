using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisingWater : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Speed * Time.deltaTime, transform.position.z);
    }
}
