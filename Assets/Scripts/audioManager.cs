using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class audioManager : MonoBehaviour
{
    public static audioManager instance;

    public Audio[] musicAudio, sfxAudio;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { 
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Login":
                PlayMusic("LoginTheme");
                break;
            case "GameLobby":
                PlayMusic("GameLobbyTheme");
                break;
            case "ForestLevel":
                PlayMusic("ForestTheme");
                break;
            case "DesertLevel":
                PlayMusic("DesertTheme");
                break;
            case "GlitchLevel":
                PlayMusic("GlitchTheme");
                break;
        }
    }

    public void PlayMusic(string name)
    {
        Audio a = Array.Find(musicAudio, x => x.name == name);
        if (a == null)
        {
            Debug.Log("Music not found");
        }
        else
        {
            musicSource.clip = a.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Audio a = Array.Find(sfxAudio, x => x.name == name);
        if (a == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.PlayOneShot(a.clip);
        }
    }

    public void MusicVolume(float volume)
    {

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicSource.volume = PlayerPrefs.GetFloat("MusicVolume");
        else
            musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume");
        else
            sfxSource.volume = volume;
    }
}
