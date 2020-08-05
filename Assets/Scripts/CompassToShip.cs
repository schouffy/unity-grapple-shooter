using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassToShip : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => { gameObject.SetActive(true); });
    }

    void Update()
    {
        transform.LookAt(Constants.Ship.transform);
    }
}
