using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsSpring : MonoBehaviour
{
    public Rigidbody PlayerRB;

    public float MaxDistanceX;
    public float MaxDistanceY;
    public float VelocityMultiplierY;
    public float SpringSpeed;

    private Vector3 _initialLocalPos;

    void Start()
    {
        _initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        var newPosition = new Vector3(
            _initialLocalPos.x - Mathf.Clamp(PlayerRB.velocity.x, -MaxDistanceX, MaxDistanceX),
            _initialLocalPos.y - Mathf.Clamp(PlayerRB.velocity.y * VelocityMultiplierY, -MaxDistanceY, MaxDistanceY),
            _initialLocalPos.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, SpringSpeed * Time.deltaTime);
    }
}
