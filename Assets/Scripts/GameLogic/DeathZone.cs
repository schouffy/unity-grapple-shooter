using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : PlayerTriggerZone
{
    public override void OnPlayerEnter(GameObject player)
    {
        base.OnPlayerEnter(player);
        EventManager.TriggerEvent(EventType.GameOver, null);
        player.GetComponent<Rigidbody>().isKinematic = true; // to make player stop falling and reaching big position numbers
    }
}
