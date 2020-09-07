using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image BlackOverlay;
    public GameObject Loading;

    private void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        yield return FadeToBlack(1f);
        yield return LoadScene("level-1");
    }

    public void StartGame()
    {
        StartCoroutine(_StartGame());
    }

    IEnumerator _StartGame()
    {
        yield return FadeToBlack(1f);
        yield return LoadScene("level-2");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
