using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform Target;

    private Vector3 _offset;
    // Start is called before the first frame update
    void Start()
    {
        _offset = Target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Target.position + _offset;
    }
}
