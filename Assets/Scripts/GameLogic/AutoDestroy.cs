using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float Delay = 1;

    void Start()
    {
        Destroy(this.gameObject, Delay);
    }
}
