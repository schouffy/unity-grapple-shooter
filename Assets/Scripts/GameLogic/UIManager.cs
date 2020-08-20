using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject GameOverScreen;
    public Image GameOverBlackOverlay;
    public GameObject GameOverText;
    public GameObject LevelEndScreen;
    public GameObject CheckpointReachedScreen;
    public GameObject ExtractionShipOnTheWayScreen;
    public GameObject HurtOverlay;
    public Image HurtOverlayDamageDirection;
    private int? _currentHealth;
    public UnityEngine.UI.Text Health;
    public UnityEngine.UI.Text Score;

    private void Start()
    {
        UpdateScore(0);
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth((HealthEventParam)p));
        EventManager.StartListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver((GameOverEventParam)p));
        EventManager.StartListening(EventType.LevelEnd, (p) => this.EndLevel());
        EventManager.StartListening(EventType.CheckpointReached, (p) => this.CheckpointReached());
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => this.ExtractionShipOnTheWay());
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth((HealthEventParam)p));
        EventManager.StopListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver((GameOverEventParam)p));
        EventManager.StopListening(EventType.LevelEnd, (p) => this.EndLevel());
        EventManager.StopListening(EventType.CheckpointReached, (p) => this.CheckpointReached());
        EventManager.StopListening(EventType.SummonExtractionShip, (p) => this.ExtractionShipOnTheWay());
    }

    void UpdateHealth(HealthEventParam healthEventParam)
    {
        if (healthEventParam.Health < _currentHealth)
            StartCoroutine(_ShowHurtScreen(healthEventParam));
        _currentHealth = healthEventParam.Health;

        Health.text = $"<b>+</b>{healthEventParam.Health.ToString()}";
    }
    IEnumerator _ShowHurtScreen(HealthEventParam healthEventParam)
    {
        //var direction = Constants.Player.transform.forward.normalized - healthEventParam.ProjectileDirection.Value.normalized;

        //var direction = -healthEventParam.ProjectileDirection.Value;
        //direction.y = 0;


        //var angle = Constants.Player.transform.InverseTransformDirection(direction);
        //Debug.Log("Angle: " + angle);
        //Debug.DrawRay(Constants.Player.transform.position, -healthEventParam.ProjectileDirection.Value * 10, Color.cyan, 5f);

        //-healthEventParam.ProjectileDirection.Value;
        HurtOverlayDamageDirection.rectTransform.rotation = Quaternion.Euler(0, 0, 90); // angle on z 
        // TODO

        HurtOverlay.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        HurtOverlay.SetActive(false);
    }

    void UpdateScore(int score)
    {
        Score.text = $"{score}<b>/</b>{GameManager.instance.PlayerScoreToReach.ToString()}";
    }

    void GameOver(GameOverEventParam eventParam)
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        StartCoroutine(_GameOver(eventParam));
    }

    IEnumerator _GameOver(GameOverEventParam eventParam)
    {
        GameOverScreen.SetActive(true);

        var color = GameOverBlackOverlay.color;
        
        if (eventParam != null && eventParam.FadeToBlackTime > 0)
        {
            float currentTime = 0, start = 0;
            while (currentTime < eventParam.FadeToBlackTime)
            {
                currentTime += Time.deltaTime;
                color.a = Mathf.Lerp(start, 1f, currentTime / eventParam.FadeToBlackTime);
                GameOverBlackOverlay.color = color;
                yield return null;
            }
        }
        else
        {
            color.a = 1;
            GameOverBlackOverlay.color = color;
        }
        GameOverText.SetActive(true);
    }

    void EndLevel()
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        LevelEndScreen.SetActive(true);
    }

    void CheckpointReached()
    {
        StartCoroutine(_CheckpointReached());
    }
    IEnumerator _CheckpointReached()
    {
        CheckpointReachedScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        CheckpointReachedScreen.SetActive(false);
    }

    void ExtractionShipOnTheWay()
    {
        StartCoroutine(_ExtractionShipOnTheWay());
    }
    IEnumerator _ExtractionShipOnTheWay()
    {
        ExtractionShipOnTheWayScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        ExtractionShipOnTheWayScreen.SetActive(false);
    }

}
