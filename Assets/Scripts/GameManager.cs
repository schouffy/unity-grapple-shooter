using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int PlayerScore;
    public int PlayerScoreToReach;
    private bool _extractionShipSummoned;

    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (!_instance)
                {
                    Debug.LogError("There needs to be one active GameManager script on a GameObject in your scene.");
                }
                else
                {
                    _instance.Init();
                }
            }
            return _instance;
        }
    }

    private void Init()
    {
        PlayerScore = 0;
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.GrabCollectible, (collectible) => this.PlayerCollected(((CollectibleEventParam)collectible).Collectible));
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver(p));
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.GrabCollectible, (collectible) => this.PlayerCollected(((CollectibleEventParam)collectible).Collectible));
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver(p));
    }

    private void PlayerCollected(Collectible collectible)
    {
        PlayerScore += collectible.Value;
        Debug.Log("Collectible acquired. Score: " + PlayerScore);

        if (PlayerScore >= PlayerScoreToReach && !_extractionShipSummoned)
        {
            _extractionShipSummoned = true;
            EventManager.TriggerEvent(EventType.SummonExtractionShip, null);
        }
    }

    private void GameOver(EventParam eventParam)
    {
        Debug.Log("Player died. Game over");
        // enemies stop attacking
        // show some UI
        // press to retry


    }
}
