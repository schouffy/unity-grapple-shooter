using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointZone : PlayerTriggerZone
{
    public Transform RespawnPosition;

    public override void OnPlayerEnter(GameObject player)
    {
        base.OnPlayerEnter(player);

        if (GameManager.RespawnPosition.HasValue && GameManager.RespawnPosition.Value == RespawnPosition.position)
            gameObject.SetActive(false);
        else
        {
            EventManager.TriggerEvent(EventType.CheckpointReached, new CheckpointReachedEventParam
            {
                RespawnPosition = RespawnPosition.position
            });
            gameObject.SetActive(false);
        }
    }
}
