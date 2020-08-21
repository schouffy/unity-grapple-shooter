using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSpringJointTweaker : MonoBehaviour
{
    public Transform HandsHolder;
    public Transform HandsMesh;

    public float MaxDistance;
    public float PullTogetherForceStrength;

    private Vector3 _anchor;

    void Start()
    {
        _anchor = HandsMesh.GetComponent<SpringJoint>().anchor;
    }

    void LateUpdate()
    {
        if (Vector3.Distance(HandsHolder.position - _anchor, HandsMesh.position) > MaxDistance)
        {
            HandsMesh.position = Vector3.Lerp(HandsMesh.position, HandsHolder.position - _anchor, PullTogetherForceStrength * Time.deltaTime);
        }
    }
}
