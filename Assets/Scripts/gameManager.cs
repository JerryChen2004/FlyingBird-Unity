using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Auth;
using Firebase.Database;

using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public static bool isOnline = false;

    public int score = 0;
    public bool hasHit = false;
    public bool pause = false;
    public bool gameEnd = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Connecting...");
            if (task.Result == DependencyStatus.Available)
            {
                FetchAndActivateRemoteConfig();
            }
            else
            {
                Debug.LogError("Firebase dependencies not available: " + task.Result);
            }
        });
    }
    void FetchAndActivateRemoteConfig()
    {
        FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
        {
            if (fetchTask.IsCompletedSuccessfully)
            {
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(_ =>
                {
                    isOnline = true;
                    Debug.Log("Online");

                    string version = FirebaseRemoteConfig.DefaultInstance.GetValue("FlyingBirdGameVersion").StringValue;
                    Debug.Log("Online Game Version: " + version);

                    PlayerPrefs.SetString("GameVersion", version);
                    PlayerPrefs.Save();
                });
            }
            else
            {
                isOnline = false;
                Debug.LogWarning("Remote Config fetch failed: " + fetchTask.Exception);

                Debug.Log("Offline");
                Debug.Log("Offline Game Version " + PlayerPrefs.GetString("GameVersion"));
            }
        });
    }

    public void PauseGame()
    {
        audioManager.instance.musicSource.Pause();
        Time.timeScale = 0f;
        pause = true;

    }
    public void ResumeGame()
    {
        audioManager.instance.musicSource.UnPause();
        Time.timeScale = 1f;
        pause = false;
    }
    public void AddScore(int amount)
    {
        score += amount;
        audioManager.instance.PlaySFX("Score");
        HighScore();
    }
    public void Reset()
    {   
        score = 0;
        hasHit = false;
        pause = false;
        gameEnd = false;
        audioManager.instance.musicSource.time = 0f;
    }
    public void ReplayGame()
    {
        Reset();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void HighScore()
    {
        string key = "";

        switch (SceneManager.GetActiveScene().name)
        {
            case "ForestLevel":
                key = "ForestScore";
                break;
            case "DesertLevel":
                key = "DesertScore";
                break;
            case "GlitchLevel":
                key = "GlitchScore";
                break;
        }

        int saveScore = PlayerPrefs.GetInt(key);

        if (score > saveScore) 
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save();
            SavePlayerPrefsToDatabase();
        }
    }
    public void SavePlayerPrefsToDatabase()
    {
        if (isOnline)
        {
            string username = PlayerPrefs.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError("Username not found in PlayerPrefs.");
                return;
            }

            string currentUid = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
            if (string.IsNullOrEmpty(currentUid))
            {
                Debug.LogError("Not authenticated.");
                return;
            }

            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            dbRef.Child("usernames").Child(username).Child("uid").GetValueAsync().ContinueWithOnMainThread(uidTask =>
            {
                if (!uidTask.IsCompletedSuccessfully || !uidTask.Result.Exists)
                {
                    Debug.LogError("Username not found in database.");
                    return;
                }

                string storedUid = uidTask.Result.Value.ToString();
                if (storedUid != currentUid)
                {
                    Debug.LogError("Username does not match current user.");
                    return;
                }

                int forest = PlayerPrefs.GetInt("ForestScore");
                int desert = PlayerPrefs.GetInt("DesertScore");
                int glitch = PlayerPrefs.GetInt("GlitchScore");

                Dictionary<string, object> scores = new Dictionary<string, object>()
                {
                    { "forestLevel", forest },
                    { "desertLevel", desert },
                    { "glitchLevel", glitch }
                };

                Dictionary<string, object> userData = new Dictionary<string, object>()
                {
                    { "scores", scores }
                };

                Dictionary<string, int> levelScores = new Dictionary<string, int>
                {
                    { "forestLevel", forest },
                    { "desertLevel", desert },
                    { "glitchLevel", glitch }
                };

                foreach (var level in levelScores)
                {
                    dbRef.Child("leaderboard").Child(level.Key).Child(username).SetValueAsync(level.Value);
                }

                dbRef.Child("users").Child(username).SetValueAsync(userData).ContinueWithOnMainThread(saveTask =>
                {
                    if (saveTask.IsCompletedSuccessfully)
                    {
                        Debug.Log("PlayerPrefs synced to Firebase.");
                    }
                    else
                    {
                        Debug.LogError("Failed to sync PlayerPrefs: " + saveTask.Exception);
                    }
                });
            });
        }
    }


}
