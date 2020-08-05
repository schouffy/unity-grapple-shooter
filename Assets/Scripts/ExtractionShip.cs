using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionShip : MonoBehaviour
{
    public Animator ShipAnimator;
    public Transform PlayerExtractPosition;

    private void Awake()
    {
        ShowMesh(false);
    }

    private void ShowMesh(bool visible)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => this.Summon());
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.SummonExtractionShip, (p) => this.Summon());
    }

    void Summon()
    {
        ShowMesh(true);

        // animate arrival
        ShipAnimator.SetTrigger("Arrival");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.PlayerTag)
        {
            StartCoroutine(LeaveWithShip());
        }
    }
    IEnumerator LeaveWithShip()
    {
        // prevent player from moving and place it on ship
        yield return MovePlayerInShip();

        // ship moves away
        ShipAnimator.SetTrigger("Departure");
        yield return new WaitForSeconds(7f);

        // fade to black
        // load next level
        EventManager.TriggerEvent(EventType.LevelEnd, null);
    }

    IEnumerator MovePlayerInShip()
    {
        Constants.Player.GetComponent<Rigidbody>().isKinematic = true;
        var playerTransform = Constants.Player.transform;
        while (true)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, PlayerExtractPosition.position, 2f * Time.deltaTime);
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, PlayerExtractPosition.rotation, Time.deltaTime);

            if (Vector3.Distance(playerTransform.position, PlayerExtractPosition.position) < 1)
            {
                playerTransform.parent = PlayerExtractPosition;
                break;
            }
            yield return null;
        }
    }
}
