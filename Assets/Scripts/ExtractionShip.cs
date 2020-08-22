using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionShip : MonoBehaviour
{
    public Animator ShipAnimator;
    public Transform PlayerExtractPosition;
    public AudioSource DepartureFxSource;
    public AudioClip DepartureThrustFx;
    public AudioClip DepartureMusic;

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
        DepartureFxSource.PlayOneShot(DepartureThrustFx);
        yield return new WaitForSeconds(3f);
        DepartureFxSource.PlayOneShot(DepartureMusic);
        yield return new WaitForSeconds(3f);

        // fade to black
        // load next level
        EventManager.TriggerEvent(EventType.LevelEnd, new EndLevelEventParam { FadeToBlackTime = 2f });
    }

    IEnumerator MovePlayerInShip()
    {
        var playerController = Constants.Player.GetComponent<PlayerController>();
        var playerTransform = Constants.Player.transform;

        Constants.Player.GetComponent<Rigidbody>().isKinematic = true;
        Constants.Player.GetComponentInChildren<GrapplingHook>().Ungrapple();
        playerController.RemoveControlFromPlayer();
        
        while (true)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, PlayerExtractPosition.position, 2f * Time.deltaTime);
            
            if (Vector3.Distance(playerTransform.position, PlayerExtractPosition.position) < 1)
            {
                playerTransform.parent = PlayerExtractPosition;

                var beginRotationTime = Time.time;
                while (Time.time - beginRotationTime < 2f)
                {
                    playerTransform.localRotation = Quaternion.Euler(
                        0,
                        Mathf.Lerp(playerTransform.localRotation.eulerAngles.y, 0, 1.5f * Time.deltaTime),
                        0);
                    playerController.playerCameraHolder.localRotation = Quaternion.identity;
                    yield return null;
                }
                break;
            }
            yield return null;
        }
    }
}
