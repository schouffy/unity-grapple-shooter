using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsSway : MonoBehaviour
{
    public float amount;
    public float maxAmount;
    public float smoothAmount;

    private Vector3 _initialPos;

    // Start is called before the first frame update
    void Start()
    {
        _initialPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amount - Input.GetAxis("Horizontal") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount - Input.GetAxis("Vertical") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPos = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + _initialPos, Time.deltaTime * smoothAmount);
    }
}
