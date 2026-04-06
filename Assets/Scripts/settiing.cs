using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingManager : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;

    void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusicValue();
        else
            UIMusicVolume();

        if (PlayerPrefs.HasKey("SFXVolume"))
            LoadSFXValue();
        else
            UISFXVolume();
    }

    public void UIMusicVolume()
    {
        audioManager.instance.MusicVolume(musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }
    public void LoadMusicValue()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        UIMusicVolume();
    }
    public void UISFXVolume()
    {
        audioManager.instance.SFXVolume(sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
    public void LoadSFXValue()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        UISFXVolume();
    }
}
