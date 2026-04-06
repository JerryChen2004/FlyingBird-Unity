using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;




public class StartGame : MonoBehaviour
{
    public GameObject text;
    public GameObject loginPanel;
    public GameObject registerPanel;

    public GameObject StatusPanel;

    public GameObject infoPanel;
    public TMP_Text infoText;

    public TMP_Text gameVersionText;
    
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public TMP_Text loginText;

    public TMP_InputField resgisterUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerComfirmPasswordInput;
    public TMP_Text registerText;

    private bool hasPressed = false;

    private sceneManager sceneManager;

    void Awake()
    {
        sceneManager = FindObjectOfType<sceneManager>();

        if (PlayerPrefs.HasKey("GameVersion"))
            gameVersionText.text = "Ver " + PlayerPrefs.GetString("GameVersion");
        else
            gameVersionText.text = "Ver 1.0";
    }
    void Update()
    {
        if (hasPressed) return;
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            if (PlayerPrefs.HasKey("Username"))
            {
                StartCoroutine(ShowInfo("Welcome Back, " + PlayerPrefs.GetString("Username")));
                gameManager.instance.SavePlayerPrefsToDatabase();
                StartCoroutine(GoMainGame());
            }
            else 
            {
                if (gameManager.isOnline)
                    loginPanel.SetActive(true);
                else
                    StatusPanel.SetActive(true);
            }
            text.SetActive(false);
            hasPressed = true;
        }
    }
    bool HasNumber(string input)
    {
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                return true;
            }
        }
        return false;
    }

    public void Login()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        string username = loginUsernameInput.text.Trim().ToLower();
        string password = loginPasswordInput.text.Trim();

        string error = "";

        if (username.Length <= 0 && password.Length <= 0)
            error += "Please insert username and password.\n";
        else if (username.Length > 0 && password.Length <= 0)
            error += "Please insert password.\n";

        if (!string.IsNullOrEmpty(error))
        {
            loginText.text = error.Trim();
            return;
        }

        dbRef.Child("usernames").Child(username).Child("email")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if(task.IsFaulted)
                {
                    loginText.text = "Database error.";
                }
                else if (task.IsCanceled)
                {
                    loginText.text = "Request canceled.";
                }
                else if (!task.Result.Exists)
                {
                    loginText.text = "User not found.";
                }

                string email = task.Result.Value.ToString();

                auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCanceled || authTask.IsFaulted)
                    {
                        loginText.text = "Incorrect Password.";
                    }
                    else
                    {
                        loginPanel.SetActive(false);
                        PlayerPrefs.SetString("Username", username);
                        PlayerPrefs.Save();

                        FetchUserScoresToPlayerPrefs(username);

                        StartCoroutine(ShowInfo("Welcome, " + PlayerPrefs.GetString("Username")));
                        StartCoroutine(GoMainGame());
                    }
                });
            });
    }
    void FetchUserScoresToPlayerPrefs(string username)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(username).Child("scores")
            .GetValueAsync().ContinueWithOnMainThread(scoreTask =>
            {
                if (scoreTask.IsFaulted || scoreTask.IsCanceled)
                {
                    Debug.LogError("Failed to fetch scores.");
                    return;
                }

                DataSnapshot snapshot = scoreTask.Result;
                if (snapshot.Exists)
                {
                    int forest = int.Parse(snapshot.Child("forestLevel").Value.ToString());
                    int desert = int.Parse(snapshot.Child("desertLevel").Value.ToString());
                    int glitch = int.Parse(snapshot.Child("glitchLevel").Value.ToString());

                    PlayerPrefs.SetInt("ForestScore", forest);
                    PlayerPrefs.SetInt("DesertScore", desert);
                    PlayerPrefs.SetInt("GlitchScore", glitch);
                    PlayerPrefs.Save();

                    Debug.Log("Scores saved to PlayerPrefs.");
                }
            });
    }

    public void Register()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        string username = resgisterUsernameInput.text.Trim().ToLower();
        string password = registerPasswordInput.text.Trim();
        string confirmPassword = registerComfirmPasswordInput.text.Trim();

        string error = "";

        if (username.Length <= 0 && password.Length <= 0)
            error += "Please insert username and password.\n";
        if (username.Length > 0 && username.Length < 5)
            error += "Username must have at least 5 characters.\n";
        if (password.Length > 0 && password.Length < 6)
            error += "Password must have at least 6 characters.\n";
        if (password.Length > 0 && !HasNumber(password))
            error += "Password must include at least one number.\n";
        if (password != confirmPassword)
            error += "Passwords do not match.\n";

        if (!string.IsNullOrEmpty(error))
        {
            registerText.text = error.Trim();
            return;
        }

        string usernameEmail = username + "@game.com";

        dbRef.Child("usernames").Child(username).GetValueAsync().ContinueWithOnMainThread(checkTask =>
        {
            if (checkTask.Result.Exists)
            {
                registerText.text = "Username already taken.";
            }
            else
            {
                auth.CreateUserWithEmailAndPasswordAsync(usernameEmail, password).ContinueWithOnMainThread(authTask =>
                {
                    if (authTask.IsCanceled || authTask.IsFaulted)
                    {
                        registerText.text = "Registration failed.";
                        return;
                    }

                    FirebaseUser newUser = authTask.Result.User;
                    string uid = newUser.UserId;

                    dbRef.Child("usernames").Child(username).Child("email").SetValueAsync(usernameEmail);
                    dbRef.Child("usernames").Child(username).Child("uid").SetValueAsync(uid);

                    Dictionary<string, object> scores = new Dictionary<string, object>()
                    {
                        { "forestLevel", 0 },
                        { "desertLevel", 0 },
                        { "glitchLevel", 0 }
                    };

                    Dictionary<string, object> userData = new Dictionary<string, object>()
                    {
                        { "scores", scores }
                    };

                    dbRef.Child("users").Child(username).SetValueAsync(userData).ContinueWithOnMainThread(dataTask =>
                    {
                        if (dataTask.IsCompletedSuccessfully)
                        {
                            ShowLoginPanel();
                            StartCoroutine(ShowInfo("Account Created"));
                        }
                        else
                        {
                            StartCoroutine(ShowInfo("Create Account Error"));
                        }
                    });
                });
            }
        });
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }
    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    IEnumerator ShowInfo(string text)
    {
        infoPanel.SetActive(true);
        infoText.text = text;   

        yield return new WaitForSeconds(3f);
        infoPanel.SetActive(false);
    }

    public void ButtonPress()
    {
        audioManager.instance.PlaySFX("Button");
    }
    public void Retry()
    {
        Destroy(gameManager.instance.gameObject);
        gameManager.instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator GoMainGame()
    {
        yield return new WaitForSeconds(3f);
        sceneManager.ChangeScene("GameLobby");
    }
}
