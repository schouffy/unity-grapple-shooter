using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject PauseScreen;
    public Slider MouseSensitivityX;
    public Slider MouseSensitivityY;
    public GameObject EndScreen;
    public Image BlackOverlay;
    public GameObject GameOverText;
    public GameObject LevelCompleteText;
    public GameObject CheckpointReachedScreen;
    public GameObject ExtractionShipOnTheWayScreen;
    public GameObject HurtOverlay;
    public GameObject CollectOverlay;
    public GameObject Loading;
    public Image HurtOverlayDamageDirection;
    private int? _currentHealth;
    public Text Health;
    public Text Score;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        ReloadPlayerPrefs();
        UpdateScore(0);
    }

    void OnEnable()
    {
        EventManager.StartListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth((HealthEventParam)p));
        EventManager.StartListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StartListening(EventType.GameOver, (p) => this.GameOver((GameOverEventParam)p));
        EventManager.StartListening(EventType.LevelEnd, (p) => this.EndLevel((EndLevelEventParam)p));
        EventManager.StartListening(EventType.CheckpointReached, (p) => this.CheckpointReached());
        EventManager.StartListening(EventType.SummonExtractionShip, (p) => this.ExtractionShipOnTheWay());
        EventManager.StartListening(EventType.TogglePause, (p) => this.TogglePause());
        EventManager.StartListening(EventType.LoadScene, (p) => this.LoadSceneTriggered());
    }

    void OnDisable()
    {
        EventManager.StopListening(EventType.PlayerHealthUpdated, (p) => this.UpdateHealth((HealthEventParam)p));
        EventManager.StopListening(EventType.PlayerScoreUpdated, (p) => this.UpdateScore(((IntegerEventParam)p).Value));
        EventManager.StopListening(EventType.GameOver, (p) => this.GameOver((GameOverEventParam)p));
        EventManager.StopListening(EventType.LevelEnd, (p) => this.EndLevel((EndLevelEventParam)p));
        EventManager.StopListening(EventType.CheckpointReached, (p) => this.CheckpointReached());
        EventManager.StopListening(EventType.SummonExtractionShip, (p) => this.ExtractionShipOnTheWay());
        EventManager.StopListening(EventType.TogglePause, (p) => this.TogglePause());
        EventManager.StopListening(EventType.LoadScene, (p) => this.LoadSceneTriggered());
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
        StartCoroutine(_ShowUpdateScoreOverlay());
    }

    IEnumerator _ShowUpdateScoreOverlay()
    {
        CollectOverlay.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        CollectOverlay.SetActive(false);
    }

    void LoadSceneTriggered()
    {
        Loading.SetActive(true);
    }

    void GameOver(GameOverEventParam eventParam)
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        StartCoroutine(_GameOver(eventParam));
    }

    IEnumerator _GameOver(GameOverEventParam eventParam)
    {
        EndScreen.SetActive(true);
        yield return FadeToBlack(eventParam?.FadeToBlackTime);
        GameOverText.SetActive(true);
    }

    void EndLevel(EndLevelEventParam eventParam)
    {
        Health.gameObject.SetActive(false);
        Score.gameObject.SetActive(false);
        StartCoroutine(_EndLevel(eventParam));
    }

    IEnumerator FadeToBlack(float? fadeToBlackTime)
    {
        var color = BlackOverlay.color;

        if (fadeToBlackTime.HasValue && fadeToBlackTime.Value > 0)
        {
            float currentTime = 0, start = 0;
            while (currentTime < fadeToBlackTime.Value)
            {
                currentTime += Time.deltaTime;
                color.a = Mathf.Lerp(start, 1f, currentTime / fadeToBlackTime.Value);
                BlackOverlay.color = color;
                yield return null;
            }
        }
        else
        {
            color.a = 1;
            BlackOverlay.color = color;
        }
    }

    IEnumerator _EndLevel(EndLevelEventParam eventParam)
    {
        EndScreen.SetActive(true);
        yield return FadeToBlack(eventParam?.FadeToBlackTime);
        LevelCompleteText.SetActive(true);
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

    private bool _paused = false;
    void TogglePause()
    {
        _paused = !_paused;

        if (_paused)
        {
            ReloadPlayerPrefs();
            PauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            PauseScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void ReloadPlayerPrefs()
    {
        MouseSensitivityX.value = PlayerPrefs.GetFloat(Constants.PlayerPrefs.MouseSensitivityX, Constants.Player.GetComponent<PlayerController>().MouseSensitivityX);
        MouseSensitivityY.value = PlayerPrefs.GetFloat(Constants.PlayerPrefs.MouseSensitivityY, Constants.Player.GetComponent<PlayerController>().MouseSensitivityY);
    }

    public void UpdateMouseSensitivity()
    {
        if (!_paused)
            return; // initial change at startup

        PlayerPrefs.SetFloat(Constants.PlayerPrefs.MouseSensitivityX, MouseSensitivityX.value);
        PlayerPrefs.SetFloat(Constants.PlayerPrefs.MouseSensitivityY, MouseSensitivityY.value);

        EventManager.TriggerEvent(EventType.PlayerPrefsUpdated, null);
    }

}
