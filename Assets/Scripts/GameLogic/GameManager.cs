﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int PlayerScore;
    public int PlayerScoreToReach;
    private bool _extractionShipSummoned;
    public static Vector3? RespawnPosition;

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
        
        if (RespawnPosition.HasValue)
        {
            Constants.Player.transform.position = RespawnPosition.Value;
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.CollectibleAcquired, (collectible) => this.PlayerCollected(((IntegerEventParam)collectible).Value));
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver());
        EventManager.StartListening(EventType.LevelEnd, (p) => this.EndLevel());
        EventManager.StartListening(EventType.CheckpointReached, (p) => this.CheckpointReached((CheckpointReachedEventParam)p));
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.CollectibleAcquired, (collectible) => this.PlayerCollected(((IntegerEventParam)collectible).Value));
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver());
        EventManager.StopListening(EventType.LevelEnd, (p) => this.EndLevel());
        EventManager.StopListening(EventType.CheckpointReached, (p) => this.CheckpointReached((CheckpointReachedEventParam)p));
    }

    private void PlayerCollected(int value)
    {
        PlayerScore += value;
        EventManager.TriggerEvent(EventType.PlayerScoreUpdated, new IntegerEventParam { Value = PlayerScore });

        if (PlayerScore >= PlayerScoreToReach && !_extractionShipSummoned)
        {
            _extractionShipSummoned = true;
            EventManager.TriggerEvent(EventType.SummonExtractionShip, null);
        }
    }

    private void CheckpointReached(CheckpointReachedEventParam eventParams)
    {
        RespawnPosition = eventParams.RespawnPosition;
    }

    private void GameOver()
    {
        StartCoroutine(_GameOver());
    }

    IEnumerator _GameOver()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            }
            yield return null;
        }
    }

    private void EndLevel()
    {
        StartCoroutine(_EndLevel());
    }

    IEnumerator _EndLevel()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            }
            yield return null;
        }
    }
}
