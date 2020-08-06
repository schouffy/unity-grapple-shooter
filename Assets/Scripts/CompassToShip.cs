using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassToShip : MonoBehaviour
{
    private void Start()
    {
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => { gameObject.SetActive(true); });
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.LookAt(Constants.Ship.transform);
    }
}
