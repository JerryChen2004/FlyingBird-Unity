using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;

using TMPro;

public class MainGame : MonoBehaviour
{
    public TMP_Text profileText;
    public TMP_Text usernameText;

    public TMP_Text levelListText;

    void Start()
    {
        gameManager.instance.ResumeGame();

        profileText.text = PlayerPrefs.GetString("Username");
        usernameText.text = PlayerPrefs.GetString("Username");

        levelListText.text =
            "Forest\n" + "score: " + PlayerPrefs.GetInt("ForestScore")+
            "\n\n" +
            "Desert\n" + "score: " + PlayerPrefs.GetInt("DesertScore") +
            "\n\n" +
            "Glitch\n" + "score: " + PlayerPrefs.GetInt("GlitchScore")
            ;

    }

    public void ButtonPress()
    {
        audioManager.instance.PlaySFX("Button");
    }

    public void Play()
    {
        gameManager.instance.Reset();
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        gameManager.instance.Reset();
        Destroy(gameManager.instance.gameObject);
        gameManager.instance = null;
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("Login");
    }
}
