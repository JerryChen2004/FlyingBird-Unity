using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text totalScoreText;
    public TMP_Text BestScoreText;
    public TMP_Text gameText;

    public GameObject pauseButton;
    public GameObject pausePanel;
    public GameObject gameEndPanel;
    public GameObject gamePlay;
    public GameObject countDown;

    public TMP_Text countdownText;
    public float countdownTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartCountdown());

        ShowScore();
    }


    public void ShowScore()
    {
        scoreText.text = "Score: " + gameManager.instance.score;
        totalScoreText.text = "Totel Score: " + gameManager.instance.score;

        switch (SceneManager.GetActiveScene().name)
        {
            case "ForestLevel":
                BestScoreText.text = "Totel Score: " + PlayerPrefs.GetInt("ForestScore");
                break;
            case "DesertLevel":
                BestScoreText.text = "Totel Score: " + PlayerPrefs.GetInt("DesertScore");
                break;
            case "GlitchLevel":
                BestScoreText.text = "Totel Score: " + PlayerPrefs.GetInt("GlitchScore");
                break;
        }
    }

    public void Pause()
    {
        gameManager.instance.PauseGame();
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void Resume()
    {
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
        gamePlay.SetActive(false);

        StartCoroutine(StartCountdown());

    }
    public void Replay()
    {
        gameManager.instance.ReplayGame();
    }

    public void GameEnd()
    {
        gameManager.instance.PauseGame();
        gameEndPanel.SetActive(true);
        pauseButton.SetActive(false);
        pausePanel.SetActive(false);
        gamePlay.SetActive(false);

        if (gameManager.instance.gameEnd == true)
        {
            gameText.text = "Level Complete";
        }
        else 
        {
            gameText.text = "Game Over";
        }
    }

    IEnumerator StartCountdown()
    {
        countDown.SetActive(true);
        gameManager.instance.PauseGame();
        float counter = countdownTime;

        while (counter > 0)
        {
            countdownText.text = Mathf.Ceil(counter).ToString();
            audioManager.instance.PlaySFX("Countdown");
            yield return WaitForRealSeconds(1f);
            counter--;
        }
       
        countDown.SetActive(false);
        gameManager.instance.ResumeGame();

        gamePlay.SetActive(true);
    }

    IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
