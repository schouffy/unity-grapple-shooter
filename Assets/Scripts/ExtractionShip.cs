using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionShip : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => this.Summon(p));
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.SummonExtractionShip, (p) => this.Summon(p));
    }

    void Summon(EventParam eventParam)
    {
        Debug.Log("Extraction ship is on the way");

        // animate arrival

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.PlayerTag)
        {
            // player is on ship, can't move anymore
            // ship moves away
            // fade to black
            // load next level
        }
    }
}
