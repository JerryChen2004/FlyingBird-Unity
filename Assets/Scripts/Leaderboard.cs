using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Firebase.Auth;
using Firebase.Database;
using System;

public class Leaderboard : MonoBehaviour
{
    public TMP_Text level;
    public TMP_Text score;
    public TMP_Text rank;

    public TMP_Text leaderboardMap;

    private SwipeController swipeController;

    public GameObject ListPrefab;
    public Transform contentParent;

    void Start()
    {
        swipeController = FindObjectOfType<SwipeController>();
        DisplayLeaderboard();
    }

    public void DisplayLeaderboard()
    {

        switch (swipeController.currentPage)
        {
            case 1:
                leaderboardMap.text = "Forest Leaderboard";
                level.text = "Forest";

                if (PlayerPrefs.HasKey("ForestScore"))
                    score.text = "Score: " + PlayerPrefs.GetInt("ForestScore");
                else
                    score.text = "Score: " + 0;

                rank.text = "?";

                LoadLeaderboard("forestLevel", scores => {
                    DisplayLeaderboardList(scores);

                    scores.Sort((a, b) => b.score.CompareTo(a.score));
                    for (int i = 0; i < scores.Count; i++)
                    {
                        if (scores[i].username == PlayerPrefs.GetString("Username"))
                        {
                            rank.text = "#" + (i + 1);
                            break;
                        }
                    }
                });
                break;

            case 2:
                leaderboardMap.text = "Desert Leaderboard";
                level.text = "Desert";

                if (PlayerPrefs.HasKey("DesertScore"))
                    score.text = "Score: " + PlayerPrefs.GetInt("DesertScore");
                else
                    score.text = "Score: " + 0;

                rank.text = "?";

                LoadLeaderboard("desertLevel", scores => {
                    DisplayLeaderboardList(scores);

                    scores.Sort((a, b) => b.score.CompareTo(a.score));
                    for (int i = 0; i < scores.Count; i++)
                    {
                        if (scores[i].username == PlayerPrefs.GetString("Username"))
                        {
                            rank.text = "#" + (i + 1);
                            break;
                        }
                    }
                });
                break;

            case 3:
                leaderboardMap.text = "Glitch Leaderboard";
                level.text = "Glitch";

                if (PlayerPrefs.HasKey("GlitchScore"))
                    score.text = "Score: " + PlayerPrefs.GetInt("GlitchScore");
                else
                    score.text = "Score: " + 0;

                rank.text = "?";

                LoadLeaderboard("glitchLevel", scores => {
                    DisplayLeaderboardList(scores);

                    scores.Sort((a, b) => b.score.CompareTo(a.score));
                    for (int i = 0; i < scores.Count; i++)
                    {
                        if (scores[i].username == PlayerPrefs.GetString("Username"))
                        {
                            rank.text = "#" + (i + 1);
                            break;
                        }
                    }
                });
                break;
        }
    }

    public void DisplayLeaderboardList(List<(string username, int score)> scores)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        scores.Sort((a, b) => b.score.CompareTo(a.score));

        for (int i = 0; i < scores.Count; i++)
        {
            var entry = scores[i];
            GameObject row = Instantiate(ListPrefab, contentParent);
            row.SetActive(true);

            var leaderboardList = row.GetComponent<LeaderboardList>();
            leaderboardList.UserList(i + 1, entry.username, entry.score);
        }
    }

    public void LoadLeaderboard(string levelName, Action<List<(string username, int score)>> callback)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("leaderboard")
            .Child(levelName)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                var leaderboardScores = new List<(string, int)>();

                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    foreach (var child in task.Result.Children)
                    {
                        string user = child.Key;
                        int score = int.Parse(child.Value.ToString());
                        leaderboardScores.Add((user, score));
                    }
                }

                callback?.Invoke(leaderboardScores);
            });
    }

}
