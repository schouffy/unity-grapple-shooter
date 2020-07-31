using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject GameOverScreen;
    public GameObject LevelEndScreen;
    public UnityEngine.UI.Text Health;
    public UnityEngine.UI.Text Score;

    private void Start()
    {
        UpdateScore(0);
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth(((IntegerEventParam)p).Value));
        EventManager.StartListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver());
        EventManager.StartListening(EventType.LevelEnd, (p) => this.EndLevel());
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth(((IntegerEventParam)p).Value));
        EventManager.StopListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver());
        EventManager.StopListening(EventType.LevelEnd, (p) => this.EndLevel());
    }

    void UpdateHealth(int health)
    {
        Health.text = health.ToString();
    }

    void UpdateScore(int score)
    {
        Score.text = score + "/" + GameManager.instance.PlayerScoreToReach.ToString();
    }

    void GameOver()
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        GameOverScreen.SetActive(true);
    }

    void EndLevel()
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        LevelEndScreen.SetActive(true);
    }

}
