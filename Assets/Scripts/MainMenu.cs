using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image BlackOverlay;
    public GameObject Loading;
    public GameObject IntroText;

    private static bool _hasSeenIntro = false;

    private void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (_hasSeenIntro)
        {
            StartCoroutine(FadeToBlack(1f, true));
        }
        else
        {
            Cursor.visible = false;
            IntroText.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(FadeToBlack(1f, true));
            IntroText.SetActive(false);
        }
    }

    IEnumerator FadeToBlack(float? fadeToBlackTime, bool reverse)
    {
        var color = BlackOverlay.color;
        float currentTime = 0, startAlpha = 0, endAlpha = 1f;

        if (reverse)
        {
            startAlpha = 1f;
            endAlpha = 0f;
        }

        if (fadeToBlackTime.HasValue && fadeToBlackTime.Value > 0)
        {
            while (currentTime < fadeToBlackTime.Value)
            {
                currentTime += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, endAlpha, currentTime / fadeToBlackTime.Value);
                BlackOverlay.color = color;
                yield return null;
            }
        }
        else
        {
            color.a = endAlpha;
            BlackOverlay.color = color;
        }

        BlackOverlay.gameObject.SetActive(!reverse);
    }

    IEnumerator LoadScene(string scene)
    {
        Loading.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void StartTutorial()
    {
        StartCoroutine(_StartTutorial());
    }

    IEnumerator _StartTutorial()
    {
        yield return FadeToBlack(1f, false);
        yield return LoadScene("level-1");
    }

    public void StartGame()
    {
        StartCoroutine(_StartGame());
    }

    IEnumerator _StartGame()
    {
        yield return FadeToBlack(1f, false);
        yield return LoadScene("level-2");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void FadeBlackOut()
    {
        _hasSeenIntro = true;
        StartCoroutine(FadeToBlack(1f, true));
        Cursor.visible = true;
    }
}
