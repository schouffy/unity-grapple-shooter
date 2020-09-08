using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : PlayerTriggerZone
{
    private bool _alreadyEntered;
    public float FadeToBlackTime;

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject.tag != Constants.PlayerTag)
        {
            if (other.gameObject.GetComponent<EnemyAI>() != null)
            {
                other.gameObject.GetComponent<EnemyAI>().Die();
            }
        }
    }

    public override void OnPlayerEnter(GameObject player)
    {
        if (_alreadyEntered)
            return;

        _alreadyEntered = true;

        base.OnPlayerEnter(player);
        StartCoroutine(GameOver(player));
    }

    IEnumerator GameOver(GameObject player)
    {
        EventManager.TriggerEvent(EventType.GameOver, new GameOverEventParam { FadeToBlackTime = FadeToBlackTime });

        yield return new WaitForSeconds(FadeToBlackTime);
        GetComponent<AudioSource>().Play();
        player.GetComponent<Rigidbody>().isKinematic = true; // to make player stop falling and reaching big position numbers
    }
}
